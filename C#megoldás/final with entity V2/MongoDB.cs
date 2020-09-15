using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TwinCAT.Ads;

namespace final_with_entity
{
    class MongoDB :DBabstract, IDataBase
    {
        public string ConnectionString { get; set; }
        public bool Connected { get; set; }
        private IMongoDatabase db;
        private string DB;
        private string MongoDBDatabase;
        private int hMongoDBDatabase;

        public MongoDB()
        {
            Connected = false;
            //MongoClient Client = new MongoClient();
            //TCDataBase
            
        }
        public void Connect(TcAdsClient ads)
        {
            CreateHandles(ads);
            ReadConnectionInfo(ads);
            MongoClient Client = new MongoClient();
            db = Client.GetDatabase(MongoDBDatabase);
            Connected = true;
            Console.WriteLine("connected to MongoDB");
            
        }

        private void CreateHandles(TcAdsClient ads)
        {
            hMongoDBDatabase = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.MongoDBConnectionInfo.MongoDBDatabase");
        }

        public void CreateTable(string tablecreate)
        {
            try
            {
                db.CreateCollection(tablecreate);
            }
            catch (Exception)
            {
                Console.WriteLine("table exists");
                throw;
            }
            
            Console.WriteLine("Table created: "+tablecreate);
        }

        public void Disconnect()
        {
            Connected = false;
            
        }

        public void Insert(string tablewrite, RecordModell record)
        {
            var collection = db.GetCollection<RecordModell>(tablewrite);
            collection.InsertOne(record);
            Console.WriteLine("record inserted to MongoDB DB");
        }

        private void ReadConnectionInfo(TcAdsClient ads)
        {
            MongoDBDatabase = ads.ReadAny(hMongoDBDatabase, typeof(string), new int[] { 255 }).ToString();
        }

        public string[] Select(string tableread, string kezdo, string veg)
        {
            DateTime Dkezdo = DateTime.ParseExact(kezdo, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime Dveg = DateTime.ParseExact(veg, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var collection = db.GetCollection<RecordModell>(tableread);
            var returneddata = collection.Find<RecordModell>(r => r.Dátum > Dkezdo && r.Dátum < Dveg).ToList();
            foreach (var record in returneddata)
            {
                record.Dátum = record.Dátum.AddHours(2);
            }
            var returneddata2 = returneddata.ToArray();
            return recordmodell_arr_to_str_arr(returneddata2);
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
    }
}
