using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.Reflection;

namespace final_with_entity
{
    public abstract class DBabstract
    {
        //public string ConnectionString { get; set; }
        //public bool Connected { get; set; }
        //public abstract void Connect();
        //public abstract void Disconnect();
        //public abstract void CreateHandles();
        //public abstract void ReadConnectionInfo();
        //public abstract void CreateTable(string tablecreate);
        //public abstract string[] Select(string tableread, string kezdo, string veg);
        //public abstract void Insert(string tablewrite, RecordModell record);
        public string[] recordmodell_arr_to_str_arr(RecordModell[] records)
        {
            PropertyInfo[] myPropertyInfo;

            myPropertyInfo = records[1].GetType().GetProperties();

            string[] returneddata = new string[records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                //PropertyInfo[] myPropertyInfo;
                
                //myPropertyInfo = records[i].GetType().GetProperties();
                
                returneddata[i] = "(";
                for (int j = 0; j < myPropertyInfo.Length; j++)
                {

                    returneddata[i] = returneddata[i] + myPropertyInfo[j].GetValue(records[i]).ToString() + ", ";
                }
                returneddata[i] = returneddata[i] + ")";
                //Console.WriteLine(i + ".  " + returneddata[i]);

            }
            Console.WriteLine(returneddata.Length + " records returned");
            return returneddata;

        }


    }
}
