using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDBCourse.POCO;

namespace MongoDBCourse
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
            Console.WriteLine();
            Console.ReadLine();
        }

        
        static async Task MainAsync(string[] args)
        {
            var client = new MongoClient(new MongoClientSettings()
            {
                Server = new MongoServerAddress("localhost", 27017),
            });

            await HomeWork22(client);
        }

        private static async Task HomeWork21(MongoClient client)
        {
            var db = client.GetDatabase("students");
            var grades = db.GetCollection<Grade>("grades");

            var foundGrade = await grades.Find(g => g.Score > 65 && g.Type == "exam")
                .Sort(Builders<Grade>.Sort.Ascending(g => g.Score)).FirstAsync();

            Console.WriteLine("Student ID: " + foundGrade.StudentID);
        }

        private static async Task HomeWork22(MongoClient client)
        {
            var db = client.GetDatabase("students");
            var grades = db.GetCollection<Grade>("grades");

            var lowestGrades = new Dictionary<int, Grade>();

            var foundGrades = await grades.Find(g => true).ToListAsync();

            foreach (var grade in foundGrades)
            {
                Grade curLowest;
                if (!lowestGrades.TryGetValue(grade.StudentID, out curLowest) || grade.Score < curLowest.Score)
                {
                    lowestGrades[grade.StudentID] = grade;
                }
            }

            var result = await grades.BulkWriteAsync(lowestGrades.Values.Select(g => new DeleteOneModel<Grade>(new BsonDocument("_id", g.Id))));

            if (result.DeletedCount != 200)
            {
                Console.WriteLine("Delete request failed!");
                return;
            }

            if (grades.Count(new BsonDocument()) != 600)
            {
                Console.WriteLine("Invalid number of documents in database after request");
                return;
            }

            //Test queries
            var test1Res = await grades.Find(new BsonDocument()).Sort("{ 'student_id' : 1, 'score' : 1, }").Limit(5).ToListAsync();

            if (test1Res.Count != 5)
            {
                Console.WriteLine("Invalid number of results result!");
                return;
            }
        }


    }
}
