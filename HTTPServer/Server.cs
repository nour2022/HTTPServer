using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(iPEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            serverSocket.Listen(100);

            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            Console.WriteLine("Connection Started with Client: ", clientSocket.RemoteEndPoint);
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] receivedData = new byte[1024 * 1024];
                    int receivedLen = clientSocket.Receive(receivedData);
                    // TODO: break the while loop if receivedLen==0
                    if(receivedLen == 0)
                    {
                        Console.WriteLine("Connection Ended By Client : ",clientSocket.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(receivedData));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content, string.Empty);
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Configuration.RootPath + request.relativeURI;
                //TODO: check for redirect
                string redirectionPath =GetRedirectionPagePathIFExist(request.relativeURI);
                if (redirectionPath.Length > 0) 
                {
                    physicalPath = Configuration.RootPath + "/" + redirectionPath;
                    content = File.ReadAllText(physicalPath);
                    return new Response(StatusCode.Redirect, "text/html", content, redirectionPath);
                }
                //TODO: check file exists
                bool isExist = File.Exists(physicalPath);
                if (!isExist)
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", content, string.Empty);
                }
                //TODO: read the physical file
                content = File.ReadAllText(physicalPath);
                // Create OK response
                return new Response(StatusCode.OK, "text/html", content, string.Empty);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, string.Empty);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string RedirectionPath;
            if (relativePath[0] == '/')
            {
                relativePath = relativePath.Substring(1);
            }
            bool isExist = Configuration.RedirectionRules.TryGetValue(relativePath, out RedirectionPath);
            if (isExist)
            {
                return RedirectionPath;
            }
            else
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            bool isExist = File.Exists(filePath);
            if (!isExist)
            {
                Logger.LogException(new Exception(defaultPageName + " not Exist"));
                return string.Empty;
            }
            // else read file and return its content
            return File.ReadAllText(filePath);
           
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                string[] Rules = File.ReadAllLines(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach(var Rule in Rules)
                {
                    string[] rule = Rule.Split(',');
                    Configuration.RedirectionRules.Add(rule[0], rule[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
