using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using TwinCAT.Ads;
//using System.Data.Linq;
using Dapper;
using System.Data;


namespace final_with_entity
{
    public class Access : IDataBase
    {
        public string ConnectionString { get ; set ; }
        
        private string Path;
         private OleDbConnection Connection { get; set; }

        public int hPath { get; set; }
        public bool Connected { get; set; }



        public void Connect(TcAdsClient ads)
        {
            CreateHandles(ads);
            ReadConnectionInfo(ads);
            if (Connected == false)
            {
                Connection = new OleDbConnection(CreateConnectionString());
                try
                {
                    Connection.Open();
                    Connected = true;
                    Console.WriteLine("DB Access connection open");

                }
                catch (Exception)
                {
                    Console.WriteLine("error connecting to Access DB");
                    throw;
                }
                
            }
           
        }

        public void CreateTable(string tablecreate)
        {
            string strSQL=  "CREATE TABLE " + tablecreate + " (Dátum DATETIME , temp_set INTEGER, temp_act INTEGER, hum_set INTEGER, hum_act INTEGER)";

            if (Connected == true)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandText = strSQL;
                //cmd.Parameters.AddWithValue("?tablecreate", tablecreate);
                cmd.Connection = Connection;
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Table: " + tablecreate + " created in Access DB");
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed createing " + tablecreate + " in Access DB");
                    throw;
                }



            }
            else
            {
                Console.WriteLine("Not connected to Access DB.. connect first to write");
            }
        }

        public void Disconnect()
        {
            if (Connected==true)
            {
                try
                {
                    Connection.Close();
                    Connected = false;
                    Console.WriteLine("db Access connection closed");
                }
                catch (Exception)
                {
                    Console.WriteLine("error while closing Access connection");
                    throw;
                }
            }
        }



        public string[] Select(string tableread, string kezdo, string veg)
        {
            DataTable dt = new DataTable();
            string strSQL = "SELECT Dátum, temp_set, temp_act, hum_set, hum_act FROM " + tableread + " WHERE Dátum BETWEEN @kezdo AND @veg";
            
            if (Connected == true)
            {
                if (dt.Rows.Count > 0)
                {
                    dt.Clear();
                }
               OleDbCommand cmd = new OleDbCommand();
                cmd.CommandText = strSQL;
                DateTime Dkezdo = DateTime.ParseExact(kezdo, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
               DateTime Dveg = DateTime.ParseExact(veg, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
               cmd.Parameters.AddWithValue("@kezdo", Dkezdo);
               cmd.Parameters.AddWithValue("@veg", Dveg);
                cmd.Connection = Connection;
               OleDbDataReader rd = cmd.ExecuteReader();
               dt.Load(rd);
                return DataTable_to_string_arr(dt);
             

            }
            else
            {
                Console.WriteLine("DB Access not connected");
                return null;
                
            }
        }
        private void CreateHandles(TcAdsClient ads)
        {
            hPath = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.AccessConnectionInfo.Path");
        }
        private void ReadConnectionInfo(TcAdsClient ads)
        {
            Path = ads.ReadAny(hPath, typeof(string), new int[] { 255 }).ToString();
        }
        private string CreateConnectionString()
        {
            
            string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path + "";
            return ConnectionString;
        }

        public void Insert(string tablewrite, RecordModell record)
        {
            string strSQL = "INSERT INTO " + tablewrite + "(Dátum, temp_set, temp_act, hum_set, hum_act) VALUES('" + DateTime.Now.ToString() + "', @temp_set, @temp_act, @hum_set, @hum_act)";
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandText = strSQL;
            cmd.Parameters.AddWithValue("@temp_set", record.temp_set);
            cmd.Parameters.AddWithValue("@temp_act", record.temp_act);
            cmd.Parameters.AddWithValue("@hum_set", record.hum_set);
            cmd.Parameters.AddWithValue("@hum_act", record.hum_act);
            cmd.Connection = Connection;
            if (Connected==true)
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("struct written to access database");
                }
                catch (Exception)
                {
                    Console.WriteLine("failed writing to access db");
                    throw;
                }
            }
            

            else
            {
                Console.WriteLine("Not connected to Access DB.. connect first to write");
            }
        }
                //Connection.Execute(cmd, new {temp_set=parameters[0] }, new { temp_act= parameters[1]}, new { hum_set=parameters[2]}, new {hum_act=parameters[3] })


        public Access()
        {
            Connected = false;
        }
        private string [] DataTable_to_string_arr(DataTable dt)
        {
            string[] results = new string[dt.Rows.Count];
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    results[j] = results[j] + dt.Rows[j].ItemArray[i];
                    results[j] = results[j] + ", ";
                }
                Console.WriteLine("" + j + "      ");
                Console.WriteLine(results[j]);
            }
            return results;
        }
        

    }
}
