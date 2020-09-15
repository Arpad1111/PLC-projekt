using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.Reflection;
using System.Data;

namespace final_with_entity
{
    public class SQLite :DBabstract, IDataBase
    {
        public string ConnectionString { get; set; }
        public bool Connected { get; set; }
        
        private string SQLitePath;
        private int hSQLitePath;
        

        public SQLite()
        {
            Connected = false;
        }
        public void Connect(TcAdsClient ads)
        {
            CreateHandles(ads);
            ReadConnectionInfo(ads);
            ConnectionString = CreateConnectionString();
            
            Connected = true;
            Console.WriteLine("connected to SQLite DB");
        }
        public void Disconnect()
        {
            
            Connected = false;
            Console.WriteLine("disconnected from SQLite DB");
        }
        private void CreateHandles(TcAdsClient ads)
        {
            hSQLitePath = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.SQLiteConnectionInfo.Path");
        }
        private void ReadConnectionInfo(TcAdsClient ads)
        {
            SQLitePath = ads.ReadAny(hSQLitePath, typeof(string), new int[] { 255 }).ToString();
        }
        public void CreateTable(string tablecreate)
        {
            using (var context = new tc_dataCollectionContext())
            {
                context.ConnectionString = ConnectionString;
                context.DBMS = "SQLite";
                context.Tablename = tablecreate;
                RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
                databaseCreator.CreateTables();
            }
            Console.WriteLine("table created: "+ tablecreate);


        }
        public string[] Select(string tableread, string kezdo, string veg)
        {
            DateTime Dkezdo = DateTime.ParseExact(kezdo, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime Dveg = DateTime.ParseExact(veg, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            using (var context = new tc_dataCollectionContext())
            {
                context.ConnectionString = ConnectionString;
                context.DBMS = "SQLite";
                context.Tablename = tableread;
                var Records = context.Chambers.Where<RecordModell>(r => r.Dátum > Dkezdo && r.Dátum < Dveg);
                RecordModell[] dataarray = new RecordModell[Records.Count()];
                int i = 0;
                foreach (var record in Records)
                {
                    dataarray[i] = record;
                    i++;
                }
                Console.WriteLine("select lefutott" );
                return recordmodell_arr_to_str_arr(dataarray);
            }
            
        }

        public void Insert(string tablewrite, RecordModell record)
        {

            using (var context = new tc_dataCollectionContext())
            {
                context.ConnectionString = ConnectionString;
                context.DBMS = "SQLite";
                context.Tablename = tablewrite;
                context.Chambers.Add(record);
                context.SaveChanges();
            }
            Console.WriteLine("record successfully inserted to : "+ tablewrite);


        }
        //private string[] recordmodell_arr_to_str_arr(RecordModell[] records)
        //{
        //    string[] returneddata = new string[records.Length];
        //    for (int i = 0; i < records.Length; i++)
        //    {
        //        PropertyInfo[] myPropertyInfo;
        //        myPropertyInfo = records[i].GetType().GetProperties();
               
        //        returneddata[i] = "(";
        //        for (int j = 0; j < myPropertyInfo.Length; j++)
        //        {

        //            returneddata[i] = returneddata[i] + myPropertyInfo[j].GetValue(records[i]).ToString() + ", ";
        //        }
        //        returneddata[i] = returneddata[i] + ")";
        //        Console.WriteLine(i + ".  " + returneddata[i]);
                
        //    }
        //    return returneddata;
        //}
        private string CreateConnectionString()
        {
            ConnectionString= @"Data Source = " + SQLitePath + ";";
            return ConnectionString;
        }
    }
}
