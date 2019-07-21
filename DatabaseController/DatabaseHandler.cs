using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RokonoDbManager.Models;

namespace RokonoDbManager.DatabaseController
{
    public class DatabaseHandler
    {
        public static void AddRecordToSavedConnections(IncomingLoginAttempt loginAttempt)
        {

            if(!File.Exists("LoginConnection"))
            {
                File.Create("LoginConnection").Close();
            
                WriteFiles(loginAttempt);
            }
            else
                WriteFiles(loginAttempt);
        }

        public static List<BoundConnected> GetSavedConnections()
        {
            var result = new List<IncomingLoginAttempt>();
            if(File.Exists("LoginConnection"))
              result = JsonConvert.DeserializeObject<List<IncomingLoginAttempt>>(File.ReadAllText("LoginConnection"));
            var bindingData = new List<BoundConnected>();
            result.ForEach(x=>{
                bindingData.Add(new BoundConnected{
                    text = $"{x.Host} {x.Username} {x.Database}",
                    ConnectionId = x.ConnectionId
                });
            });

            return bindingData;
        }

        private static void WriteFiles(IncomingLoginAttempt loginAttempt)
        {
            FileStream fileStream = new FileStream("LoginConnection", FileMode.Open);
            if(fileStream != null)
            {
                using (StreamWriter StreamWriter = new StreamWriter(fileStream))
                { 

                    StreamWriter.Flush();
                    var data = JsonConvert.DeserializeObject<List<IncomingLoginAttempt>>(File.ReadAllText("LoginConnection"));
                    if(data == null)
                        data = new List<IncomingLoginAttempt>();
                    var connectionId = data.Count+1;
                    loginAttempt.ConnectionId = connectionId;
                    data.Add(loginAttempt);
                    StreamWriter.WriteLine(JsonConvert.SerializeObject(data));
                }
            }
        }

        internal static string GetFilePath(string databaseName, string connectionString)
        {
            if(!File.Exists("PlantUMLData"))
                File.Create("PlantUMLData").Close();

            var data = JsonConvert.DeserializeObject<List<DatabasePlantUMLPath>>(File.ReadAllText("PlantUMLData"));
            if(data == null)
                return string.Empty; 
            var selectedDatabase =data.FirstOrDefault(x=>x.ConnectionId == connectionString && x.DatabaseName == databaseName);
            if(selectedDatabase != null)
                return selectedDatabase.FilePath;
                
            else return string.Empty;
        }

        internal static List<SavedConnection> LoadConnections()
        {
            var result = new List<SavedConnection>();
            var data = JsonConvert.DeserializeObject<List<IncomingLoginAttempt>>(File.ReadAllText("LoginConnection"));
            data.ForEach(x=>{
                result.Add(new SavedConnection{
                        ConnectionId = x.ConnectionId,
                        ConnectionString = $"Server={x.Host};Database={x.Database};User ID={x.Username};Password='{x.Password}'",
                        Host = x.Host,
                        Username =x.Username,
                        Password =x.Password
                });
            });
            return result;
        }

        public static string GenerateDatabaseRelations(List<UmlBindingData> umlData,string fileName)
        {

            if(!string.IsNullOrEmpty(fileName))
            {
                if(!File.Exists(fileName))
                {
                    File.Create(fileName).Close();
                    GenerateSchema(umlData,fileName);
                }
                else
                    GenerateSchema(umlData,fileName);
            }

            return GetPlantUMLText(umlData); 
        }

        private static void GenerateSchema(List<UmlBindingData> umlData, string fileName)
        {     
            using (StreamWriter StreamWriter = new StreamWriter(fileName))
            { 
                umlData.ForEach(x=>{
                    x.Tables.ForEach(y=>{
                        StreamWriter.WriteLine($"class {y.Shape.Name}");
                        StreamWriter.WriteLine("{");
                        y.Shape.Attribute.ForEach(z=>{


                            StreamWriter.WriteLine($"    {z.Name} {z.Column}");
                        });

                        StreamWriter.WriteLine("}");
                    });
                    x.Connections.ForEach(y => {
                        StreamWriter.WriteLine($"{y.TableName} \"1\" *-- \"many\" {y.ConnectionName}");   
                    });
                });
               
            }
        }

        private static string GetPlantUMLText(List<UmlBindingData> umlData)
        {
            var res = new StringBuilder();
            umlData.ForEach(x=>{
                x.Tables.ForEach(y=>{
                    res.AppendLine($"class {y.Shape.Name}");
                    res.AppendLine("{");

                    y.Shape.Attribute.ForEach(z=>{

                        res.AppendLine($"    {z.Name} {z.Column}");

                    });
                    res.AppendLine("}");
                });
                x.Connections.ForEach(y => {
                    res.AppendLine($"{y.TableName} \"1\" *-- \"many\" {y.ConnectionName}");   
                });
            });
            return res.ToString();
        }
    }
}

  