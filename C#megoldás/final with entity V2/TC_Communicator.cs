using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace final_with_entity
{
    class TC_Communicator
    {
        public TcAdsClient ads;
        //public datastream =new AdsStream(31);
        public AdsStream datastream;
        public BinaryReader binread;
        //public delegate void AdsNotificationEventHandler(object sender, AdsNotificationEventArgs e);
        //public event AdsNotificationEventHandler ADSNotification;

        public int hConnected { get; private set; }
        public int hDBMS { get; private set; }
        public int hKezdo { get; private set; }
        public int hVeg { get; private set; }
        public int hTableRead { get; private set; }
        public int hReturnReady { get; private set; }
        public int hTableWrite { get; private set; }
        public int hWriteReady { get; private set; }
        public int hTableCreate { get; private set; }
        public int hTableReady { get; private set; }
        
        public  int hConnect { get; private set; }
        public  int hExecuteDataReturn { get; private set; }
        public  int hWriteExecute { get; private set; }
        public  int hExecuteTableCreate { get; private set; }
        public  bool connect { get; set; }
        public  bool ExecuteDataReturn { get; set; }
        public  bool WriteExecute { get; set; }
        public  bool ExecuteTableCreate { get; set; }
        public string DBMS { get; private set; }
        public string kezdo { get; private set; }
        public string veg { get; private set; }
        public string tableread { get; private set; }
        public string tablewrite { get; private set; }
        public string tablecreate { get; private set; }
        public  bool connected { get;  set; }
        public  bool WriteReady { get;  set; }
        public  bool ReturnReady { get;  set; }
        public  bool TableReady { get;  set; }

        public bool PrevExecuteTableCreate { get; set; }
        public bool PrevWriteExecute { get; set; }
        //public bool PrevDisconnect { get; set; }
        public bool PrevExecuteDataReturn { get; set; }
        public bool prevconnect { get; set; }


        public ArrayList hWriteArray = new ArrayList();
        public ArrayList WriteArray = new ArrayList();
        public ArrayList TCWriteStructVariables = new ArrayList();

        public void Connect()
        {
            
            ads.Connect(851);
            Console.WriteLine("connected to TC");
        }

        public void CreateHandles()
        {
            try
            {
                hConnected = ads.CreateVariableHandle("MAIN.ConnectDB.bConnected");
            }
            catch (Exception)
            {
                Console.WriteLine("not connected to tc");
                throw;
            }
            
            hDBMS = ads.CreateVariableHandle("MAIN.ConnectDB.DBInfo.DBMS");
            hKezdo = ads.CreateVariableHandle("MAIN.DataReturn.ReturnBetweenDates.kezdo");
            hVeg = ads.CreateVariableHandle("MAIN.DataReturn.ReturnBetweenDates.veg");
            hTableRead = ads.CreateVariableHandle("MAIN.DataReturn.ReturnBetweenDates.table");
            hReturnReady = ads.CreateVariableHandle("MAIN.DataReturn.bReady");
            hTableWrite = ads.CreateVariableHandle("MAIN.DBWrite.Table");
            hWriteReady = ads.CreateVariableHandle("MAIN.DBWrite.bReady");
            hTableCreate = ads.CreateVariableHandle("MAIN.CreateTable.tablename");
            hTableReady = ads.CreateVariableHandle("MAIN.CreateTable.bReady");
            for (int i = 0; i < TCWriteStructVariables.Count-1; i++)
            {
                hWriteArray.Add( ads.CreateVariableHandle("MAIN.DBWrite.DataStruct." + TCWriteStructVariables[i+1] + ""));

            }

        }
        public void CreateNotificationHandles()
        {
            try
            {
                hConnect = ads.AddDeviceNotification("MAIN.ConnectDB.bConnect", datastream, 0, 1, AdsTransMode.OnChange, 100, 0, connect);
            }
            catch (Exception)
            {
                Console.WriteLine("not connected to tc");
                throw;
            }
            
            hExecuteDataReturn = ads.AddDeviceNotification("MAIN.DataReturn.bExecute", datastream, 0, 1, AdsTransMode.OnChange, 100, 0, ExecuteDataReturn);
            hWriteExecute = ads.AddDeviceNotification("MAIN.DBWrite.bExecute", datastream, 0, 1, AdsTransMode.OnChange, 100, 0, WriteExecute);
            hExecuteTableCreate = ads.AddDeviceNotification("MAIN.CreateTable.bExecute", datastream, 0, 1, AdsTransMode.OnChange, 100, 0, ExecuteTableCreate);

            //AdsNotificationEventHandler handler = OnNotification;
            //ads.AdsNotification += ADSNotification;
            //mainbe: 
            //ads.AdsNotification += ADSNotification;
            //ads.AdsNotification += new AdsNotificationEventHandler(Onnotification);
        }
        public void Onnotification(object sender, AdsNotificationErrorEventArgs e)
        {

        }
        //protected virtual void OnNotification(AdsNotificationEventArgs e)
        //{
        //    ADSNotification?.Invoke(this, e);
        //}

        public TC_Communicator(ArrayList tcwritestructvariables)
        {
            this.TCWriteStructVariables = tcwritestructvariables;
            ads = new TcAdsClient();
            datastream = new AdsStream(31);
            binread = new BinaryReader(datastream, System.Text.Encoding.ASCII);

        }
        public void ReadVariables()
        {
            DBMS = ads.ReadAny(hDBMS, typeof(string), new int[] { 50 }).ToString();
            kezdo = ads.ReadAny(hKezdo, typeof(string), new int[] { 50 }).ToString();
            veg = ads.ReadAny(hVeg, typeof(string), new int[] { 50 }).ToString();
            tableread = ads.ReadAny(hTableRead, typeof(string), new int[] { 50 }).ToString();
            tablewrite = ads.ReadAny(hTableWrite, typeof(string), new int[] { 50 }).ToString();
            tablecreate = ads.ReadAny(hTableCreate, typeof(string), new int[] { 50 }).ToString();
        }
        public void WriteVariables()
        {
            ads.WriteAny(hConnected, connected);

        }
        public void WriteReadyFlags()
        {
            ads.WriteAny(hWriteReady, WriteReady);
            ads.WriteAny(hReturnReady, ReturnReady);
            ads.WriteAny(hTableReady, TableReady);
        }
        public void recordarray_to_TC( string[] recordarray)
        {
            int hrecordarray;
            for (int i = 0; i < recordarray.Length; i++)
            {
                hrecordarray = ads.CreateVariableHandle("MAIN.DataReturn.recordarray[" + i + "]");
                ads.WriteAny(hrecordarray, recordarray[i], new int[] { 99 });
            }
            
        }
        public void getRecordFromTC(int FieldCount)
        {
            
            for (int i = 0; i < FieldCount-1; i++)
            {
                WriteArray.Add( Convert.ToInt32(ads.ReadAny((int)hWriteArray[i], typeof(int))));
            }
        }

        public TC_Communicator()
        {
            connect = false;
            connected = false;
            prevconnect = false;
            ReturnReady = false;
            TableReady = false;
            WriteReady = false;
            ExecuteDataReturn = false;
            WriteExecute = false;
            ExecuteTableCreate = false;

        }



    }
}
