using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBCourse.POCO
{
    public class Score
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("score")]
        public double Value { get; set; }

        /*
        {
                        "type" : "exam",
                        "score" : 1.463179736705023
        }*/
    }
}
