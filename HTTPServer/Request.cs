using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;
        string[] FullRequest;
        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {


            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] delimeters = { "\r\n" };
            FullRequest = requestString.Split(delimeters, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (FullRequest.Length < 3)
            {
                return false;
            }
            // Parse Request line
            requestLines = FullRequest[0].Split(' ');
            bool isValidFormat = ParseRequestLine();
            // Validate blank line exists
            isValidFormat &= ValidateBlankLine();
            // Load header lines into HeaderLines dictionary
            isValidFormat &= LoadHeaderLines();
            return isValidFormat;
        }

        //returns true if the requestLines is in a valid format.
        private bool ParseRequestLine()
        {
            if (requestLines.Length < 2)
            {
                return false;
            }

            if (requestLines.Length == 2)
            {
                httpVersion = HTTPVersion.HTTP09;
            }
            else
            {
                if (requestLines[2] == "HTTP/1.0")
                {
                    httpVersion = HTTPVersion.HTTP10;
                }
                else if (requestLines[2] == "HTTP/1.1") 
                {
                    httpVersion = HTTPVersion.HTTP11;
                }

                else 
                    { return false; }
            }
            switch (requestLines[0].ToUpper())
            {
                case "GET":
                    method = RequestMethod.GET;
                    break;
                case "POST":
                    method = RequestMethod.POST;
                    break;
                case "HEAD":
                    method = RequestMethod.HEAD;
                    break;
                default:
                    return false;
            }

            relativeURI = requestLines[1];
            return ValidateIsURI(relativeURI);
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            bool result = true;
            headerLines = new Dictionary<string, string>();
            for(int i = 1; i < FullRequest.Length - 2; i++)
            {
                if (FullRequest[i].Contains(":"))
                {
                    string[] splitChar = { ": " };
                    string[] request = FullRequest[i].Split(splitChar, StringSplitOptions.None);
                    headerLines.Add(request[0], request[1]);

                }
                else result = false;
            }
            return result;
        }

        private bool ValidateBlankLine()
        {
            if (FullRequest[(FullRequest.Length - 2)] == string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
