using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using RokonoDbManager.Models;

namespace RokonoDbManager.DatabaseController
{
    public class DbManager : IDisposable
    {
        SqlConnection SqlConnection; 
        public DbManager(string connectionString)
        {
            SqlConnection = new SqlConnection(connectionString);
        }

        public List<OutboundTableConnection> GetDbUmlData()
        {
            var result = new List<OutboundTableConnection>();
            var query = "SELECT tp.name 'Parent table',tr.name 'Refrenced table' FROM  sys.foreign_keys fk INNER JOIN  sys.tables tp ON fk.parent_object_id = tp.object_id INNER JOIN  sys.tables tr ON fk.referenced_object_id = tr.object_id INNER JOIN  sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id ORDER BY tp.name, cp.column_id";
            SqlCommand command = new SqlCommand(query, SqlConnection);

            // Open the connection in a try/catch block. 
            // Create and execute the DataReader, writing the result
            // set to the console window.
            try
            {
                SqlConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new OutboundTableConnection{
                            TableName = reader.GetString(0),
                            ConnectionName = reader.GetString(1)

                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        internal List<string> GetTables()
        {
            var result = new List<string>();
            var query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;
        }

        internal List<string> GetTableKeys(string name)
        {
            var query = "SELECT tp.name 'Parent table',tr.name 'Refrenced table' FROM  sys.foreign_keys fk INNER JOIN  sys.tables tp ON fk.parent_object_id = tp.object_id INNER JOIN  sys.tables tr ON fk.referenced_object_id = tr.object_id INNER JOIN  sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id ORDER BY tp.name, cp.column_id";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;
        }

        internal List<string> GetDatabaseRoles(string x)
        {
            var query = "Select [name] From sysusers Where issqlrole = 1";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;
        }

        internal List<string> GetDatabaseSchemas(string x)
        {
            var query = "select s.name as schema_name, s.schema_id, u.name as schema_owner from sys.schemas s inner join sys.sysusers u on u.uid = s.principal_id order by s.name";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;
        }

        internal List<string> GetDatabaseUsers(string name)
        {
            var query = $"select * from {name}.sys.server_principals";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;
        }

        internal List<string> GetDatabaseTriggers(string x)
        {
            var query = "select name from sys.triggers";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;
        }

      
        internal List<string> GetDatabaseStoredProcedures(string name)
        {
            var query = $"select * from {name}.information_schema.routines where routine_type = 'PROCEDURE'";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
            result.Add(reader.GetString(2));
            reader.Close();
            SqlConnection.Close();
            return result;        
        }

        internal List<string> GetDatabaseViews(string x)
        {
            var query = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE ='VIEW'";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
            result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;        
        }

        internal List<string> GetTableStatistics(string name)
        {
            var query = $"SELECT sp.stats_id, name, filter_definition, last_updated, rows, rows_sampled, steps, unfiltered_rows, modification_counter FROM sys.stats AS stat CROSS APPLY sys.dm_db_stats_properties(stat.object_id, stat.stats_id) AS sp  WHERE stat.object_id = object_id('{name}');";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
            result.Add(reader.GetString(1));
            reader.Close();
            SqlConnection.Close();
            return result;        
        }

        internal List<string> GetTableIndexes(string name)
        {
            var query = "col.* FROM sys.indexes ind INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id INNER JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id INNER JOIN sys.tables t ON ind.object_id = t.object_id WHERE ind.is_primary_key = 0 AND ind.is_unique = 0 AND ind.is_unique_constraint = 0 AND t.is_ms_shipped = 0 ORDER BY  t.name, ind.name, ind.index_id, ic.index_column_id;";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;        
        }

        internal List<string> GetTabeTriggers(string name)
        {
            var query = $"Select [tgr].[name] as [trigger name]from sysobjects tgr join sysobjects tbl on tgr.parent_obj = tbl.id WHERE tgr.xtype = 'TR' and [tbl].[name] = '{name}'";
            var result = new List<string>();
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;        
        }

        internal List<string> GetDatabases()
        {
            var result = new List<string>();
            var query = "EXEC sp_databases";
            var reader = ExecuteQuery(query);
            while (reader.Read())    
                result.Add(reader.GetString(0));
            reader.Close();
            SqlConnection.Close();
            return result;
        }

        public OutboundTable GetTableData(string tableName)
        {
            var result = new OutboundTable();
            result.Shape = new Shape();
            result.Id = tableName;
            result.Shape.Name = tableName;
            result.Shape.Attribute = new List<BindingRow>();
            result.Shape.Name = tableName;
            var query =$"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}'";
            var reader = ExecuteQuery(query);
            while (reader.Read())    
            {
                var lenght = reader.GetSqlValue(2);
                if(lenght  != null)
                    lenght = lenght as string;
                result.Shape.Attribute.Add( new BindingRow{
                    Column = $"{reader.GetString(1)} | {lenght} | {reader.GetString(3)}",
                    Name = reader.GetString(0)
                });
            }
            reader.Close();
            SqlConnection.Close();

            return result;
        }
        public SqlDataReader ExecuteQuery(string query)
        {
            
            SqlCommand command = new SqlCommand(query, SqlConnection);
            try
            {
                SqlConnection.Open();
                return command.ExecuteReader();
              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DbManager()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
