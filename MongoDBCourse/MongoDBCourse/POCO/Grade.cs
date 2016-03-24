using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBCourse.POCO
{
    public class Grade
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("student_id")]
        public int StudentID { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("score")]
        public double Score { get; set; }
    }
}
