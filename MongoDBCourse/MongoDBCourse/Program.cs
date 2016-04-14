using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDBCourse.POCO;
using MoreLinq;

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

            await HomeWork54(client);
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

        private static async Task HomeWork31(MongoClient client)
        {
            var db = client.GetDatabase("school");
            var collection = db.GetCollection<Student>("students");

            var students = await collection.Find(new BsonDocument()).ToListAsync();

            var updateMods = new List<UpdateOneModel<Student>>();
            foreach (var student in students)
            {
                var minScore = student.Scores.Where(s => s.Type == "homework").MinBy(s => s.Value); //
                var updateOperation = Builders<Student>.Update.PullFilter(s => s.Scores, s => s.Value == minScore.Value && s.Type == minScore.Type);
                updateMods.Add(new UpdateOneModel<Student>(Builders<Student>.Filter.Where(s => s.ID == student.ID), updateOperation));
            }

            var result = await collection.BulkWriteAsync(updateMods);

            if (result.ModifiedCount != 200)
            {
                Console.WriteLine("Invalid modified count");
                return;
            }

            if (collection.Count(s => true) != 200)
            {
                Console.WriteLine("Invalid number of documents in database after request");
                return;
            }

            var testElement = await collection.Find<Student>(s => s.ID == 137).FirstOrDefaultAsync();
            if (testElement == null || testElement.Scores.Count != 3)
            {
                Console.WriteLine("Invalid test element");
                Console.WriteLine(testElement?.ToJson<Student>(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true }));
                return;
            }
        }

        private static async Task HomeWork44(MongoClient client)
        {
            var db = client.GetDatabase("m101");
            var collection = db.GetCollection<BsonDocument>("profile");

            var longesterQuery = await collection.Find(p => p["ns"] == "school2.students" && p["op"] == "query").SortByDescending(p => p["millis"]).FirstAsync();

            Console.WriteLine(longesterQuery);
        }

        private static async Task HomeWork51(MongoClient client)
        {
            var db = client.GetDatabase("blog");
            var collection = db.GetCollection<Post>("posts");

            var posts = await collection.Find(p => true).ToListAsync();
            var dictionary = new Dictionary<string, int>();
            foreach (var post in posts)
            {
                foreach (var comment in post.Comments)
                {
                    int curValue;
                    dictionary.TryGetValue(comment.Author, out curValue);
                    dictionary[comment.Author] = curValue + 1;
                }
            }

            foreach (var entry in dictionary.OrderByDescending(kvpair => kvpair.Value))
            {
                Console.WriteLine($"Comments: {entry.Value}, Author {entry.Key}");
            }

            Console.WriteLine("\n\n------------\n\n");

            var groups = await collection.Aggregate().Unwind<Post, BsonDocument>(p => p.Comments).Group(p => p["comments"]["author"], g => new { Author = g.Key, NRComments = g.Sum(c => 1) }).SortByDescending(g => g.NRComments).ToListAsync();

            foreach (var group in groups)
            {
                Console.WriteLine($"Comments: {group.NRComments}, Author {group.Author}");
            }
        }

        private static async Task HomeWork52(MongoClient client)
        {
            var db = client.GetDatabase("test");
            var collection = db.GetCollection<Zip>("zips");

            var zips = await collection.Find(zip => zip.State == "NY" || zip.State == "CA").ToListAsync();

            var store = new List<Zip>();
            foreach (var zip in zips)
            {
                var key = zip.State + "-" + zip.City;
                var stored = store.FirstOrDefault(z => z.State == zip.State && z.City == zip.City);
                if (stored != null)
                {
                    stored.Population += zip.Population;
                }
                else
                {
                    store.Add(zip);
                }
            }
            Console.WriteLine("Average: " + store.Where(zip => zip.Population > 25000).Average(zip => zip.Population));
           

            var groups = await collection.Aggregate().Match(z => (z.State == "CA" || z.State == "NY"))
                .Group(z => new { State = z.State, City = z.City }, g => new { ID = g.Key, Pop = g.Sum(z =>z.Population) } )
                .Match(g => g.Pop > 25000)
                .ToListAsync();

            Console.WriteLine("Average: "+groups.Average(z => z.Pop));
        }

        private static async Task HomeWork53(MongoClient client)
        {
            var db = client.GetDatabase("test");
            var collection = db.GetCollection<Grades>("grades");

            var grades = await collection.Find(g => true).ToListAsync();

            var groups = await collection.Aggregate().Unwind<Grades, BsonDocument>(grade => grade.Scores)
                .Match(Builders<BsonDocument>.Filter.Ne(grade => grade["scores.type"], "quiz"))
                .Group(grade => new { Student = grade["student_id"], Class = grade["class_id"] }, g => new { ID = g.Key, Avg = g.Average(grade => (long)grade["scores.score"]) })
                .Group(grade => grade.ID.Class, g => new { Class = g.Key, Avg = g.Average(grade => grade.Avg) })
                .SortBy(grade => grade.Avg)
                .ToListAsync();

            foreach (var group in groups)
            {
                Console.WriteLine($"Class: {group.Class} Avg: {group.Avg}");
            }
        }

        private static async Task HomeWork54(MongoClient client)
        {
            var db = client.GetDatabase("test");
            var collection = db.GetCollection<Zip>("zips");

            var zips = await collection.Find(z => true).ToListAsync();
            Console.WriteLine("Total: "+ zips.Where(zip => char.IsNumber(zip.City[0])).Sum(zip => zip.Population));
            string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            var groups = await collection.Aggregate().Project(zip => new { FirstChar = zip.City.Substring(0, 1), Pop = zip.Population })
                .Match(zip => numbers.Contains(zip.FirstChar))
                .ToListAsync();
            Console.WriteLine("Total: " + groups.Sum(zip => zip.Pop));
        }
    }
}
