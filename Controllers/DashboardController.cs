using System.Collections.Generic;
using System.Linq;
using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RokonoDbManager.DatabaseController;
using RokonoDbManager.Models;

namespace RokonoDbManager.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index(string id)
        {
            if(HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("async-DatabseGenerate", (args) =>{
                    var res = new List<UmlBindingData>();
                    using(var context = new DbManager($"Server=192.168.1.3;Database={args.ToString()};User ID=Kristifor;Password=';;@Hanjolite';"))
                    {
                        var tableData = new UmlBindingData();

                        var outboundData = new List<OutboundTable>();
                        var tables = context.GetTables();
                        tables.ForEach(x=>{
                            outboundData.Add(context.GetTableData(x));
                        });
                        tableData.Tables = outboundData;
                        tableData.Connections = context.GetDbUmlData();
                        res.Add(tableData);
                    }
                    DatabaseHandler.GenerateDatabaseRelations(res,"");
                });
                Electron.IpcMain.On("async-LoadSchemaDefaultPath", (args) =>{
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var databsePath = JsonConvert.DeserializeObject<IncomingDatabaseFilePathRequest>(args.ToString());
                    var connection = Program.SavedConnections.FirstOrDefault(x=>x.ConnectionId == databsePath.ConnectionId);
                    var getDatabaseSavedPath = DatabaseHandler.GetFilePath(databsePath.DatabaseName, connection.ConnectionString);
                    if(string.IsNullOrEmpty(getDatabaseSavedPath))
                        Electron.IpcMain.Send(mainWindow, "DatabasePathNull");                    
                    Electron.IpcMain.Send(mainWindow, "DatabaseBoundFile", getDatabaseSavedPath);

                });
                Electron.IpcMain.On("async-GenerateSchemaSavePath", (args) =>{
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var databaseInfo = JsonConvert.DeserializeObject<DatabasePlantUMLPath>(args.ToString());
                    var connection = Program.SavedConnections.FirstOrDefault(x=>x.ConnectionId == int.Parse(databaseInfo.ConnectionId));
                        
                    var res = new List<UmlBindingData>();
                    using(var context = new DbManager($"Server={connection.Host};Database={databaseInfo.DatabaseName};User ID={connection.Username};Password='{connection.Password}';"))
                    {
                        var tableData = new UmlBindingData();
                 
                        var outboundData = new List<OutboundTable>();
                        var tables = context.GetTables();
                        tables.ForEach(x=>{
                            outboundData.Add(context.GetTableData(x));
                        });
                        tableData.Tables = outboundData;
                        tableData.Connections = context.GetDbUmlData();
                        res.Add(tableData);
                        
                    }
                    var generatedPlantUML = DatabaseHandler.GenerateDatabaseRelations(res,databaseInfo.FilePath);


                    Electron.IpcMain.Send(mainWindow, "SchemaGenerated", generatedPlantUML);

                });
                
            }
            var databasesNames = new List<string>();
            if(Program.SavedConnections == null)
                Program.SavedConnections = DatabaseHandler.LoadConnections();
            
            var connectionData = Program.SavedConnections.FirstOrDefault(x=> x.ConnectionId == int.Parse(id));
            using(var context = new DbManager(connectionData.ConnectionString))
            {
                ViewData["Connections"] = context.GetDatabases();
                ViewData["ConnectionId"] = id;
                
            }
            return View();
        }
            
    }
}