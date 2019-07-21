using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RokonoDbManager.Models;

namespace RokonoDbManager
{
    public class Program
    {
        public static List<ConnectionHandlers> Connections{get;set;}
        public static List<SavedConnection> SavedConnections { get; set; }
        public static void Main(string[] args)
        {
            Connections = new List<ConnectionHandlers>();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                
                .UseElectron(args)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
