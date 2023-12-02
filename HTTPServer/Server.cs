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
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            EndPoint endpoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endpoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            Console.WriteLine("Start Listening");
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientsocket = serverSocket.Accept();
                //remoteEndPoit بترحع الاي بي و البورت نمبر للكلاينت
                Console.WriteLine("New Client Accepted: {0} ", clientsocket.RemoteEndPoint);
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientsocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            byte[] receivedData = new byte[1024 * 1024];
            int receivedLength = 0;
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    receivedLength = clientSocket.Receive(receivedData);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("clinet {0} ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Console.WriteLine("request data ");
                    Console.WriteLine(Encoding.ASCII.GetString(receivedData, 0, receivedLength));
                    Request req = new Request(Encoding.ASCII.GetString(receivedData, 0, receivedLength));

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(req);
                    Console.WriteLine("response data");
                    Console.WriteLine(response.ResponseString);
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
            //Response response=new Response();
            string content;
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest() == false)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    if (content != string.Empty)


                    {


                        Response response = new Response(StatusCode.BadRequest, "text/html", content, "", request);
                        return response;
                    }
                    else
                    {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);


                        Response response = new Response(StatusCode.NotFound, "text/html", content, "", request);
                        return response;
                    }
                }
                else
                {
                    string[] uri = request.relativeURI.Split('/');

                    //"aboutus2.html"
                    if (uri[1] == (Configuration.RedirectionRules.Keys.ElementAt(0).ToString()))
                    {
                        uri[1] = Configuration.RedirectionRules.Values.ElementAt(0).ToString();

                        content = LoadDefaultPage(uri[1]);
                        if (content != string.Empty)
                        {



                            string location = GetRedirectionPagePathIFExist(uri[1]);

                            Response response = new Response(StatusCode.Redirect, "text/html", content, location, request);
                            return response;
                        }
                        else
                        {
                            content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);

                            Response response = new Response(StatusCode.NotFound, "text/html", content, "", request);
                            return response;
                        }
                    }
                    else {
                        string filePath = Configuration.RootPath + "\\" + uri[1];
                        if (!File.Exists(filePath))
                        {
                            content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                            Response response = new Response(StatusCode.NotFound, "text/html", content, "", request);
                            return response;
                        }
                        else {
                            content = LoadDefaultPage(uri[1]);
                            Response response = new Response(StatusCode.OK, "text/html", content, "", request);
                            return response;

                        }


                    }


                }
               
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class

                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                string path = Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName);
                content = File.ReadAllText(path);


                Response response = new Response(StatusCode.InternalServerError, "text/html", content, "", request);
                return response;
            }

        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            try
            {
                string filePath = "http://localhost:1000/" + relativePath;



                return filePath;



            }

            catch
            {
                return string.Empty;
            }
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string content = "";

            string filePath = Configuration.RootPath + "\\" + defaultPageName;
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            {
                string[] tmp = File.ReadAllLines(filePath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    content += tmp[i];
                }
                return content;
            }


            catch (Exception e)
            {
                Logger.LogException(e);
                return String.Empty;
            }

        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] lines = File.ReadAllLines(@filePath);
                string[] tmp = lines[0].Split(',');


                // then fill Configuration.RedirectionRules dictionary
                Configuration.RedirectionRules.Add(tmp[0], tmp[1]);
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
