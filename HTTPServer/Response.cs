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
        Request request;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath,Request req)
        {
            //throw new NotImplementedException();
            request = req;
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("contentType " + contentType);
            headerLines.Add("content " + content);
            headerLines.Add("content Length " + content.Length);

            headerLines.Add("Date " + DateTime.Now);
            if (redirectoinPath != "") {
                headerLines.Add("Location " + redirectoinPath);

            }
            // TODO: Create the response string
            string tmp="";
            for (int i = 0; i < headerLines.Count; i++) {
               
                tmp += headerLines[i] + "\n";

            }
            responseString = GetStatusLine(code) + "\n" +tmp+ "\n" +content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            if (code == StatusCode.OK) {
                 statusLine = request.httpVersion+" " + code +" OK";
            }
            else if (code == StatusCode.BadRequest)
            {
                statusLine = request.httpVersion + " " + code + " BAD REQUEST";
            }
            else if (code == StatusCode.Redirect)
            {
                statusLine = request.httpVersion + " " + code + " REDIRECT";
            }
            else if (code == StatusCode.InternalServerError)
            {
                statusLine = request.httpVersion + " " + code + " InternalServerError";
            }
            else
            {
                statusLine = request.httpVersion + " " + code + " Not Found";
            }

            return statusLine;
        }
    }
}
