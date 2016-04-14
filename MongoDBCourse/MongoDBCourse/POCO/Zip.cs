using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBCourse.POCO
{
    public class Zip
    {
        [BsonId]
        public string ID { get; set; }
        [BsonElement("city")]
        public string City { get; set; }
        [BsonElement("pop")]
        public int Population { get; set; }
        [BsonElement("state")]
        public string State { get; set; }
        [BsonElement("loc")]
        public List<double> Location { get; set; }
    }
}
