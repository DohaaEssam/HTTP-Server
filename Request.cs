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
            string[] Parse_Pattern = { "\r\n" };
            string[] arr = requestString.Split(Parse_Pattern, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (arr.Length >= 3)
            {
                if (ParseRequestLine(arr) == true && ValidateBlankLine(arr) == true && LoadHeaderLines(arr) == true)
                    return true;
                return true;
            }
            else
            {
                return false;
            }
            // Parse Request line
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
        }

        private bool ParseRequestLine(string[] rl)
        {
            string[] requestline = rl[0].Split(' ');
            relativeURI = requestline[1];
            if (requestline.Length <= 2 && requestline[0] == "GET" || requestline[0] == "get" && requestLines[2] == "HTTP/1.1" && ValidateIsURI(relativeURI)==true)
            {
                method = RequestMethod.GET;
                httpVersion = HTTPVersion.HTTP11;
                return true;
            }
            else return false;
        }

        private bool ValidateIsURI(string uri)
        {

            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }


        private bool LoadHeaderLines(string[] arr)
        {
            string[] HL = arr[1].Split(':');
            headerLines.Add(HL[0], HL[1]);
            if (headerLines!= null)
                return true;
            else
                return false;
        }

        private bool ValidateBlankLine(string[] arr)
        {

            if (string.IsNullOrEmpty(arr[2]))
                return true;
            else
                return false;

        }
    }
}