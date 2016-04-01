using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBCourse.POCO
{
    public class Student
    {
        [BsonId]
        public int ID { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("scores")]
        public List<Score> Scores { get; set; }
        
        /*
        {
        "_id" : 0,
        "name" : "aimee Zank",
        "scores" : [
                {
                        "type" : "exam",
                        "score" : 1.463179736705023
                },
                {
                        "type" : "quiz",
                        "score" : 11.78273309957772
                },
                {
                        "type" : "homework",
                        "score" : 6.676176060654615
                },
                {
                        "type" : "homework",
                        "score" : 35.8740349954354
                }
        ]
        }*/

    }
}
