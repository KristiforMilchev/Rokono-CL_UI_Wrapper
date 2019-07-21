using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RokonoDbManager.DatabaseController;
using RokonoDbManager.Models;

namespace RokonoDbManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            
            if(HybridSupport.IsElectronActive)
            {

                Electron.IpcMain.On("async-LoginAttempt", (args) =>
                {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var attempt = JsonConvert.DeserializeObject<IncomingLoginAttempt>(args.ToString());
                    var databasesNames = new List<string>();
                    var dbConString = $"Server={attempt.Host};Database=master;User ID={attempt.Username};Password='{attempt.Password}';";
                    using(var context = new DbManager(dbConString))
                    {
                        databasesNames = context.GetDatabases();
                      
                    }
                    
                 
                    if(attempt.Remember)
                        SaveConnectionForLater(attempt);
                    var connect = new ConnectionHandlers{
                        ConnectionId = Program.Connections.Count + 1,
                        ConnectionString = dbConString
                    };
                    Program.Connections.Add(connect);
                   

                    var result = new List<MainMenuBindingData>();
                    var jsonData = JsonConvert.SerializeObject(result);
                    Electron.IpcMain.Send(mainWindow, "MainMenuData", jsonData);
                   // Electron.IpcMain.Send(mainWindow, "DatabaseConnected", connect.ConnectionId.ToString(), databaseName);


                });
                Electron.IpcMain.On("async-LoadSavedLogins", (args) =>
                {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var result = DatabaseHandler.GetSavedConnections();
                    var jsonData = JsonConvert.SerializeObject(result);
                    Electron.IpcMain.Send(mainWindow, "LoginUsersRecived", jsonData);
                   // Electron.IpcMain.Send(mainWindow, "DatabaseConnected", connect.ConnectionId.ToString(), databaseName);


                });



            }
        
            return View();
        }

        private void SaveConnectionForLater(IncomingLoginAttempt args)
        {
            DatabaseHandler.AddRecordToSavedConnections(args);
        }

        internal string CastToString(object obj)
        {
            return obj as string;
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
