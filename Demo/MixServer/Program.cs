﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket;
using NetworkSocket.Http;
using System.Reflection;
using NetworkSocket.Fast;
using NetworkSocket.WebSocket;
using MixServer.GlobalFilters;
using System.IO;
using System.Diagnostics;
using NetworkSocket.Flex;
using MixServer.AppStart;
namespace MixServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new TcpServer();
            Config.ConfigMiddleware(server);
            Config.ConfigValidation();

            var listener = new TcpListener();
            listener.Start(server, 1212);

            if (Directory.Exists("js") == false)
            {
                Directory.SetCurrentDirectory("../../");
            }

            Process.Start("http://localhost:1212/home/index");
            Console.ReadLine();
        }
    }
}