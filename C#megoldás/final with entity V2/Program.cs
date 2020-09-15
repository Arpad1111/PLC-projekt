using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using TwinCAT.Ads;
using System.Collections;

namespace final_with_entity
{
    class Program
    {
        static int RecordmodellProperties;
        static TC_Communicator TCC;
        //static MySQL mysql;
        //static SQLite sqlite;
        static IDataBase currDB;
        static void Main(string[] args)
        {
            Program p = new Program();

            RecordmodellProperties = (p.get_recordmodell_property_array(new RecordModell())).Length;
            string[] RecordModellProperties = new string[RecordmodellProperties];
            RecordModellProperties = p.get_recordmodell_property_array(new RecordModell());
            ArrayList RecordmodellPropertiesArrayList = new ArrayList();
            RecordmodellPropertiesArrayList.AddRange(RecordModellProperties);

            TCC = new TC_Communicator(RecordmodellPropertiesArrayList);
            TCC.Connect();
            TCC.CreateHandles();
            TCC.CreateNotificationHandles();
            
            //TCC.ads.AdsNotification += new TC_Communicator.AdsNotificationEventHandler(p.OnNotification);
            TCC.ads.AdsNotification += new AdsNotificationEventHandler(p.OnNotification);
            while (true)
            {
                //loop()
            }
        }
        public void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            e.DataStream.Position = e.Offset;
            if (e.NotificationHandle == TCC.hConnect)
            {
                TCC.connect = TCC.binread.ReadBoolean();
                if (TCC.connect == true && TCC.prevconnect == false)
                {///rising edge on ConnectDB.bconnect
                    TCC.ReadVariables();
                    currDB = GetDBInstance();
                    currDB.Connect(TCC.ads);
                    TCC.connected = true;
                    TCC.WriteVariables();
                }
                if (TCC.connect == false && TCC.prevconnect == true)
                {/// falling edge on ConnectDB.bconnect

                    currDB.Disconnect();
                    TCC.connected = false;
                    TCC.WriteVariables();

                }
            }
            if (e.NotificationHandle == TCC.hExecuteDataReturn)
            {
                TCC.ExecuteDataReturn = TCC.binread.ReadBoolean();
                if (TCC.ExecuteDataReturn == true )//&& TCC.PrevExecuteDataReturn == false)
                {///rising edge on DataReturn.bexecute
                    while (Convert.ToBoolean(TCC.ads.ReadAny(TCC.hReturnReady, typeof(Boolean)))) { 
                       // addig nem kezdi amig b ready igaz
                    }
                        Console.WriteLine("rising edge on bexecute");
                    TCC.ReadVariables();
                    TCC.recordarray_to_TC(currDB.Select(TCC.tableread, TCC.kezdo, TCC.veg));
                    TCC.ReturnReady = true;
                      while (!Convert.ToBoolean( TCC.ads.ReadAny(TCC.hReturnReady, typeof(Boolean))))
                      { // addig csinálja amíg BReady hamis
                            TCC.WriteReadyFlags();
                    
                      }
                        
                   
                    
                    Console.WriteLine("ready flag elküldve "+TCC.ReturnReady+"");
                    TCC.ReturnReady = false;
                    

                }
            }

            if (e.NotificationHandle == TCC.hWriteExecute)
            {
                TCC.WriteExecute = TCC.binread.ReadBoolean();
                if (TCC.WriteExecute == true && TCC.PrevWriteExecute == false)
                {///rising edge on DBWrite.bExecute

                    TCC.ReadVariables();
                    TCC.getRecordFromTC(RecordmodellProperties);
                    RecordModell record = new RecordModell(TCC.WriteArray);
                    record.Dátum = DateTime.Now;
                    currDB.Insert(TCC.tablewrite, record);
                    TCC.WriteReady = true;
                    TCC.WriteReadyFlags();
                    TCC.WriteReady = false;
                }
            }

            if (e.NotificationHandle == TCC.hExecuteTableCreate)
            {
                TCC.ExecuteTableCreate = TCC.binread.ReadBoolean();
                if (TCC.ExecuteTableCreate == true && TCC.PrevExecuteTableCreate == false)
                {/// rising edge on CreateTable.bExecute
                    TCC.ReadVariables();
                    currDB.CreateTable(TCC.tablecreate);
                    TCC.TableReady = true;
                    TCC.WriteReadyFlags();
                    TCC.TableReady = false;

                }
            }
            TCC.PrevExecuteTableCreate = TCC.ExecuteTableCreate;
            TCC.PrevWriteExecute = TCC.WriteExecute;
            TCC.PrevExecuteDataReturn = TCC.ExecuteDataReturn;
            TCC.prevconnect = TCC.connect;
        }
    

        public  string[] get_recordmodell_property_array(RecordModell record)
        {
            PropertyInfo[] mypropertyinfo;
            mypropertyinfo = record.GetType().GetProperties();
            string[] recordmodell_properties = new string[mypropertyinfo.Length];
            for (int i = 0; i < mypropertyinfo.Length; i++)
            {
                recordmodell_properties[i] = mypropertyinfo[i].Name;
            }
            return recordmodell_properties;

        }
        

        public IDataBase GetDBInstance()
        {
            if (TCC.DBMS=="MySQL")
            {
                MySQL currDB =new MySQL();
                return currDB;
            }
            if (TCC.DBMS=="SQLite")
            {
                SQLite currDB = new SQLite();
                return currDB;
            }
            if (TCC.DBMS=="Access")
            {
                Access currDB = new Access();
                return currDB;
            }
            if (TCC.DBMS=="MongoDB")
            {
                MongoDB currDB = new MongoDB();
                return currDB;
            }
            else
            {
                Console.WriteLine("DBMS not specified");
                return null;

            }
        }
    }
}
