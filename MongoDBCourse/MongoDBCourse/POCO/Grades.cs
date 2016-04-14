using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MongoDBCourse.POCO
{
    public class Grades
    {
        [BsonId]
        public ObjectId ID { get; set; }
        [BsonElement("student_id")]
        public int StudentID { get; set; }
        [BsonElement("class_id")]
        public int ClassID { get; set; }
        [BsonElement("scores")]
        public List<Score> Scores { get; private set; }
    }
}
