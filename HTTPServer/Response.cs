using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
           
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;

            string statusLine = GetStatusLine(code);
            string fullContentType = "Content-Type: " + contentType + "\r\n";
            string contentLength = "Content-Length: " + content.Length + "\r\n";
            string date = "Date: " + DateTime.Now + "\r\n";

            headerLines.Add(fullContentType);
            headerLines.Add(contentLength);
            headerLines.Add(date);

            // TODO: Create the request string
            if (code == StatusCode.Redirect)
            {
                string location = "Location: " + redirectoinPath + "\r\n";
                headerLines.Add(location);
            }

            responseString = statusLine;
            foreach (var line in headerLines)
            {
                responseString += line;
            }
            responseString += "\r\n";
            responseString += content;

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = "HTTP/1.1 " + ((int)code).ToString() + " " + code.ToString() + "\r\n";
            return statusLine;
        }
    }
}
