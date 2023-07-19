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
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);
            //TODO: initialize this.serverSocket
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
           serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
                //TODO: accept connections and start thread for each accepted connection.

            }
        }

        public void HandleConnection(object obj)
        {
            Socket clientSock = (Socket)obj;
            string welcome = "Welcome to my test server";
            byte[] data = Encoding.ASCII.GetBytes(welcome);
            clientSock.Send(data);
            int receivedLength;
            clientSock.ReceiveTimeout = 0;
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    data = new byte[1024 * 1024];
                    receivedLength = clientSock.Receive(data);
                    
                    if (receivedLength == 0)
                        break;
                    Request request = new Request(Encoding.ASCII.GetString(data, 0, receivedLength));
                    Response response = HandleRequest(request);
                 
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                    // TODO: Receive request

                    // TODO: break the while loop if receivedLen==0

                    // TODO: Create a Request object using received request string

                    // TODO: Call HandleRequest Method that returns the response

                    // TODO: Send Response back to client

                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    // TODO: log exception using Logger class
                }
            }
            clientSock.Close();
            // TODO: close client socket
        }

        Response HandleRequest(Request request)
        {
            int code = 500;
            string content;
            try
            {
                bool GoodRequest = request.ParseRequest();
                string file = request.relativeURI;
                string[] x = file.Split('/');
                string FileName = x[1];
                string Path = Configuration.RootPath + "\\" + FileName;
                string RedirectionalPath = GetRedirectionPagePathIFExist(FileName);

                if (GoodRequest == false)
                {
                    code = 400;
                    FileName = Configuration.BadRequestDefaultPageName;
                }
                else if (RedirectionalPath != null)
                {
                    code = 301;
                    request.relativeURI = "/" + RedirectionalPath;
                    FileName = RedirectionalPath;
                }
                else if (File.Exists(Path) == false)
                {
                    code = 404;
                    FileName = Configuration.NotFoundDefaultPageName;
                }
                else
                {
                    code = 200;

                }
                content = LoadDefaultPage(FileName);
                Path = Configuration.RootPath + "\\" + FileName;

                Response response = new Response((StatusCode)code, "text/html", content, Path);

                return response;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                Logger.LogException(ex);
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response((StatusCode)code, "html", content, "");
            }
        }
        

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string RedirectionPath;
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            else
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string FilePath = Configuration.RootPath + defaultPageName;
            if (File.Exists(FilePath))
                return File.ReadAllText(FilePath);
            else
                return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] RulesArr = File.ReadAllLines(filePath);

                Configuration.RedirectionRules = new Dictionary<string, string>();
                // then fill Configuration.RedirectionRules dictionary 
                for (int i = 0; i < RulesArr.Length; i++)
                {
                    string[] rule = RulesArr[i].Split(',');
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
