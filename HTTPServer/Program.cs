﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static String path = @"D:\RedirectionRules.txt";
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server server = new Server(1000, path);
            // 2) Start Server
            server.StartServer();
           
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
           
            StreamWriter writer = new StreamWriter(path);
            writer.Write("aboutus.html,aboutus2.html");
            writer.Close();
        }
         
    }
}
