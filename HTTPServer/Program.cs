﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            int port = 1000;
            Server server = new Server(port,"redirectionRules.txt");
            // 2) Start Server
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            string[] Rules = new string[]
            {
               "aboutus.html,aboutus2.html"
            };
            StreamWriter sw = new StreamWriter("redirectionRules.txt");
            foreach(var rule in Rules)
            {
                sw.WriteLine(rule);
            }
            sw.Close();
        }
         
    }
}
