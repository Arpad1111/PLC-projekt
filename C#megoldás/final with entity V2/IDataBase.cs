using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.Data;
using TwinCAT.Ads;

namespace final_with_entity
{
    interface IDataBase
    {
        string ConnectionString { get; set; }
         bool Connected { get; set; }
        void Connect(TcAdsClient ads);
        void Disconnect();
        void Insert(string tablewrite, RecordModell record);
        void CreateTable(string tablecreate);
        string[] Select(string tableread, string kezdo, string veg);
        
    }
}
