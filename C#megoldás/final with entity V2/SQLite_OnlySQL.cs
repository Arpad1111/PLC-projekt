using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.Data.SQLite;

namespace final_with_entity
{
    class SQLite_OnlySQL : IDataBase
    {
        private int hSQLitePath;
        private string SQLitePath;
        private SQLiteConnection con;

        public string ConnectionString { get; set; }
        public bool Connected { get; set; }

        public SQLite_OnlySQL()
        {
            Connected= false;
        }
        public void Connect(TcAdsClient ads)
        {
            CreateHandles(ads);
            ReadConnectionInfo(ads);
            con.ConnectionString = CreateConnectionString();
            try
            {
                con.Open();
                Console.WriteLine("connected to sqlite db");
            }
            catch (Exception)
            {
                Console.WriteLine("sqlite connection failed");
                throw;
            }

            
        }

        public void CreateTable(string tablecreate)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = "CREATE TABLE '" + tablecreate + "'('Dátum' TEXT, 'temp_set'  INTEGER, 'temp_act'  INTEGER, 'hum_set'   INTEGER, 'hum_act'   INTEGER)";
            cmd.Connection = con;
            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Table " + tablecreate + " created in SQLite DB");
            }
            catch (Exception)
            {
                Console.WriteLine("failed creating " + tablecreate + " in SQLite DB");
                throw;
            }


        }

        public void Disconnect()
        {
            Connected = false;
            con.Close();
            Console.WriteLine("disconnected from sqlite db");
        }

        public void Insert(string tablewrite, RecordModell record)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = "INSERT INTO '" + tablewrite + "' ('Dátum', 'temp_set', 'temp_act', 'hum_set', 'hum_act') Values(datetime('now','localtime'), @temp_set, @temp_act, @hum_set, @hum_act);";
            cmd.Parameters.AddWithValue("@temp_set", record.temp_set);
            cmd.Parameters.AddWithValue("@temp_act", record.temp_act);
            cmd.Parameters.AddWithValue("@hum_set", record.hum_set);
            cmd.Parameters.AddWithValue("@hum_act", record.hum_act);
            cmd.Connection = con;
            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("record written to SQLite database");
            }
            catch (Exception)
            {
                Console.WriteLine("failed writing to SQLite database");
                throw;
            }
        }

        public string[] Select(string tableread, string kezdo, string veg)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = "SELECT * FROM '" + tableread + "' WHERE Dátum BETWEEN @kezdo AND @veg ";
            DateTime Dkezdo = DateTime.ParseExact(kezdo, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime Dveg = DateTime.ParseExact(veg, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            cmd.Parameters.AddWithValue("@kezdo", kezdo);
            cmd.Parameters.AddWithValue("@veg", veg);
            cmd.Connection = con;
            SQLiteDataReader rd = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rd);
            return DataTable_to_string_arr(dt);

        }
        private string CreateConnectionString()
        {
            string ConnectionString = @"Data Source = " + SQLitePath + ";";
            return ConnectionString;
        }

        private void CreateHandles(TcAdsClient ads)
        {
            hSQLitePath = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.SQLiteConnectionInfo.Path");
        }
        private void ReadConnectionInfo(TcAdsClient ads)
        {
            SQLitePath = ads.ReadAny(hSQLitePath, typeof(string), new int[] { 255 }).ToString();
        }

        private string[] DataTable_to_string_arr(DataTable dt)
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
