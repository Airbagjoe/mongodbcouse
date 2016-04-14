using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBCourse.POCO
{
    public class Post
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }

        [BsonElement("tags")]
        public string[] Tags { get; set; }

        [BsonElement("permalink")]
        public string Permalink { get; set; }

        [BsonElement("comments")]
        public List<Comment> Comments { get; set; }
    }
}