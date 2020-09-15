using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.Reflection;

namespace final_with_entity
{
    public class MySQL : DBabstract, IDataBase
    {
        private int hDB;
        private int hServer;
        private int hUser;
        private int hPassword;
        private string server;
        private string user;
        private string password;
        private string DB;
        //public string DBMS { get; set; }

        public string ConnectionString { get; set; }
        public bool Connected { get; set; }
        

        public MySQL()
        {
            Connected = false;
        }
        public void Connect(TcAdsClient ads)
        {
            //context = new tc_dataCollectionContext();
            //context.DBMS = "MySQL";
            CreateHandles(ads);
            ReadConnectionInfo(ads);
            ConnectionString = CreateConnectionString();
            //context.Database.OpenConnection();
            Connected = true;
            Console.WriteLine("connected to MySQL DB");
        }

        private void CreateHandles(TcAdsClient ads)
        {
            hDB = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.MYSQLConnectionInfo.DB");
            hServer = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.MYSQLConnectionInfo.server");
            hUser = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.MYSQLConnectionInfo.user");
            hPassword = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.MYSQLConnectionInfo.password");
        }

        public void CreateTable(string tablecreate)
        {
            using (var context = new tc_dataCollectionContext())
            {
                context.DBMS = "MySQL";
                context.ConnectionString = ConnectionString;
                context.Tablename = tablecreate;
                RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
                databaseCreator.CreateTables();
            }
                Console.WriteLine("table created: " + tablecreate);


        }

        public void Disconnect()
        {
            //context.Dispose();
            Connected = false;
            Console.WriteLine("disconnected from MySQL DB");
        }

        public void Insert( string tablewrite, RecordModell record)
        {
            using (var context = new tc_dataCollectionContext())
            {

                // context.ConnectionString = ConnectionString;
                context.DBMS = "MySQL";
                context.ConnectionString = ConnectionString;
                context.Tablename = tablewrite;
                context.Chambers.Add(record);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    Console.WriteLine("error inserting to MySQL DB");
                    throw;
                }
                
            }
            Console.WriteLine("record successfully inserted to : " + tablewrite);


        }

        public void ReadConnectionInfo(TcAdsClient ads)
        {
            server = ads.ReadAny(hServer, typeof(string), new int[] { 255 }).ToString();
            user = ads.ReadAny(hUser, typeof(string), new int[] { 255 }).ToString();
            password = ads.ReadAny(hPassword, typeof(string), new int[] { 255 }).ToString();
            DB = ads.ReadAny(hDB, typeof(string), new int[] { 255 }).ToString();
        }

        public string[] Select(string tableread, string kezdo, string veg)
        {
            using (var context = new tc_dataCollectionContext())
            {
                DateTime Dkezdo = DateTime.ParseExact(kezdo, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime Dveg = DateTime.ParseExact(veg, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                context.DBMS = "MySQL";
                context.ConnectionString = ConnectionString;
                context.Tablename = tableread;
                var Records = context.Chambers.Where<RecordModell>(r => r.Dátum > Dkezdo && r.Dátum < Dveg);

                RecordModell[] dataarray = new RecordModell[Records.Count()];
                int i = 0;
                foreach (var record in Records)
                {
                    //var data = new StringBuilder();
                    //data.AppendLine($"Dátum: {record.Dátum}");
                    //data.AppendLine($"temp_set: {record.temp_set}");
                    //data.AppendLine($"temp_act: {record.temp_act}");
                    //data.AppendLine($"hum_set: {record.hum_set}");
                    //data.AppendLine($"hum_act: {record.hum_act}");
                    //Console.WriteLine(data.ToString());
                    dataarray[i] = record;
                    i++;
                }
                
                return recordmodell_arr_to_str_arr(dataarray);
            }

        }
        private string CreateConnectionString()
        {
            ConnectionString = "server=" + server + ";user id=" + user + ";database=" + DB + ";password=" + password + "";
            return ConnectionString;
        }
        //private string[] recordmodell_arr_to_str_arr(RecordModell[] records)
        //{
        //    string[] returneddata = new string[records.Length];
        //    for (int i = 0; i < records.Length; i++)
        //    {
        //        PropertyInfo[] myPropertyInfo;
        //        // Get the properties of 'Type' class object.
        //        myPropertyInfo = records[i].GetType().GetProperties();
        //        //Console.WriteLine("Properties of System.Type are:");
        //        //for (int i = 0; i < myPropertyInfo.Length; i++)
        //        //{
        //        //    Console.WriteLine(myPropertyInfo[i].ToString());
        //        //}
        //        returneddata[i] = "(";
        //        for (int j = 0; j < myPropertyInfo.Length; j++)
        //        {
                    
        //            returneddata[i] = returneddata[i] + myPropertyInfo[j].GetValue(records[i]).ToString() + ", ";
        //        }
        //        returneddata[i] = returneddata[i] + ")";
        //        Console.WriteLine(i+".  "+returneddata[i]);
                
        //    }
        //    return returneddata;
        //}
    }
}
