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
       // string[] lines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        public HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;
        string[] firstLine;
        public Request(string requestString)
        {
            this.requestString = requestString;
            /*this.requestLines[0] = this.requestString;
            this.requestLines[1] = this.headerLines.ToString();
            this.requestLines[2] = " ";
            this.requestLines[3] = this.contentLines.ToString() ;*/
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>

        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            requestLines = this.requestString.Split('\n');
            //خلي بالك لو هنهندل الكونتنت هتبقي post 
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3) { return false; }
            // Parse Request line

            if (ParseRequestLine())
            {
                //method
                if (firstLine[0] == "GET")
                    method = RequestMethod.GET;
                else if (firstLine[0] == "POST")
                    method = RequestMethod.POST;
                else if (firstLine[0] == "HEAD")
                    method = RequestMethod.HEAD;
                else
                    return false;
                //validate uri
                if (!ValidateIsURI(firstLine[1]))
                {
                    return false;
                }
                else {

                    relativeURI = firstLine[1];
                }
                //http version
                if (firstLine[2]== "HTTP/1.1")
                    httpVersion = HTTPVersion.HTTP11;
                else if (firstLine[2] == "HTTP/1.0")
                    httpVersion = HTTPVersion.HTTP10;
               else
                    httpVersion = HTTPVersion.HTTP09;
            }
            else { return false; }

            // Validate blank line exists
            if (!ValidateBlankLine()) {
                return false;
            }
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            try
            {
                firstLine = requestLines[0].Split(' ');
                return true;
            }
            catch {
                return false;
            }
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            try
            {
                int i = 1;
                while (!string.IsNullOrEmpty(requestLines[i]))
                {
                    string[] headerHost = requestLines[i].Split(':');
                    
                    headerLines.Add(headerHost[0], headerHost[1]);
                    Console.WriteLine("***************************************************************");
                    i++;

                }
            }
            //طالما دخل هنا يبقي الهيدرز فاضية
            catch
            {
                //لازم يكون في هيدرز مع الفيرجن دي
                if (httpVersion == HTTPVersion.HTTP11)
                    return false;
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            
            if (requestLines[requestLines.Length-1] != "")
            {
                Console.WriteLine("#################" + requestLines.Count());
                for (int i = 0; i < requestLines.Length; i++)
                {

                    Console.WriteLine(requestLines[i]);
                }
                return false;
            }
            return true;
        }

    }
}
