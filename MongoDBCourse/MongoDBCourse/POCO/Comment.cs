using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDBCourse.POCO
{
    public class Comment
    {
        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }
    }
}