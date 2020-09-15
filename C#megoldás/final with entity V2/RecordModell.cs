using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

//using System.Data.Linq;

namespace final_with_entity
{
    [BsonIgnoreExtraElements]
    public class RecordModell
    {
        
        public DateTime Dátum { get; set; }

        public int temp_set { get; set; }

        public int temp_act { get; set; }

        public int hum_set { get; set; }

        public int hum_act { get; set; }
        

        public RecordModell()
        {
                
        }
        public RecordModell(ArrayList WriteArray)
        {
            temp_set = (int)WriteArray[0];
            temp_act =(int) WriteArray[1];
            hum_set = (int)WriteArray[2];
            hum_act = (int)WriteArray[3];
            
        }
    }
}
