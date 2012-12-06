using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace nosqldbuit
{
    public class Content
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
    public class Tag
    {
        public string Name { get; set; }
    }
    public class TagResult
    {
        public int Count { get; set; }
        public string Name { get; set; }
    }
    public class All_Tags : AbstractMultiMapIndexCreationTask<TagResult>
    {
        public All_Tags()
        {
            AddMap<Content>(contents => from content in contents
                                        from tag in content.Tags
                                        select new
                                        {
                                            Name = tag.Name,
                                            Count = 1
                                        });

            Reduce = results => from result in results
                                group result by result.Name into tag
                                select new
                                {
                                    Count = tag.Sum(x => x.Count),
                                    Name = tag.Key,
                                };

        }
    }

    class Program
    {
        private const string connect = "http://localhost:8080";
        private const string databaseName = "TestMapReduce";
        private IDocumentStore store;

        public Program()
        {
            Console.WriteLine("initializing...");

            store = new DocumentStore { Url = connect, DefaultDatabase = databaseName };
            store.Initialize();

            //store.DatabaseCommands.DeleteIndex("FullName");

            //store.DatabaseCommands.PutIndex("FullName", new IndexDefinitionBuilder<UserObject>
            //{
            //    Map = users => from user in users
            //                   select new { user.FullName },
            //    Indexes =
            //    {
            //        { x => x.FullName, FieldIndexing.Analyzed}
            //    }
            //});

            Console.Clear();


            

        }

        private void Menu()
        {
            Console.WriteLine("1. Insert data");
            Console.WriteLine("2. Search data");
            Console.WriteLine("3. Test MapReduce");
            Console.WriteLine("4. Exit");
        }

        private void Run()
        {
            int chose;

            do
            {
                //Console.Clear();
                Menu();
                chose = Console.Read();

                switch (chose)
                {
                    case 49:
                        InsertUser();
                        break;
                    case 50:
                        SearchUser();
                        break;
                    case 51:
                        TestMapReduce();
                        break;
                }
            } while (chose != 52);

            store.Dispose();
        }

        private void TestMapReduce()
        {
            using (var session = store.OpenSession())
            {
                session.Store(new Content
                {
                    Title = "Test Title for a Video",
                    Tags = new List<Tag>
                {
                    new Tag() {Name = "c#"},
                    new Tag() {Name = "autofac"},
                    new Tag() {Name = "asp.net"},
                }
                });
                session.Store(new Content
                {
                    Title = "Test Title for an Article",
                    Tags = new List<Tag>
                {
                    new Tag() {Name = "c#"},
                    new Tag() {Name = "nhibernate"},
                    new Tag() {Name = "fluent-nhibernate"},
                    new Tag() {Name = "mvc"}
                }
                });
                session.Store(new Content
                {
                    Title = "Test Title for an Article",
                    Tags = new List<Tag>
                {
                    new Tag() {Name = "ravendb"},
                    new Tag() {Name = "asp.net"},
                    new Tag() {Name = "autofac"},
                    new Tag() {Name = "c#"}
                }
                });
                session.SaveChanges();
            }


            using (var session = store.OpenSession())
            {
                var result = session.Query<TagResult, All_Tags>()
                                    .ToList();
                foreach (var tag in result)
                {
                    Console.WriteLine(tag.Count + " x " + tag.Name);
                }
                session.SaveChanges();
            }

        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        private void InsertUser()
        {
            Console.Write("How many user do you want to insert into SQL database? ");
            int count;
            Console.ReadLine();
            string line = Console.ReadLine();
            if (!int.TryParse(line, out count))
            {
                return;
            }

            SampleData sampleDate = new SampleData();
            List<UserObject> listUser = sampleDate.CreateUser(count);            
            IDocumentSession session = store.OpenSession();

            //int n = 0;
            DateTime start = DateTime.Now;

            foreach (var user in listUser)
            {
                session.Store(user);
                //++n;
                //if (n == 50000)
                //{
                //    n = 0;
                //    session.SaveChanges();
                //}
            }
            session.SaveChanges();
            DateTime end = DateTime.Now;

            session.Dispose();
            
            Console.WriteLine("Start: {0}:{1}:{2}:{3}", start.Hour, start.Minute, start.Second, start.Millisecond);
            Console.WriteLine("End: {0}:{1}:{2}:{3}", end.Hour, end.Minute, end.Second, end.Millisecond);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sub: {0}", end - start);
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
        }

        private void DeleteAllUser()
        {
            DateTime start = DateTime.Now;

            IDocumentSession session = store.OpenSession(databaseName);
            
            //session.Advanced.DatabaseCommands.DeleteByIndex(

            session.Dispose();

            DateTime end = DateTime.Now;

            Console.WriteLine("Start: {0}:{1}:{2}:{3}", start.Hour, start.Minute, start.Second, start.Millisecond);
            Console.WriteLine("End: {0}:{1}:{2}:{3}", end.Hour, end.Minute, end.Second, end.Millisecond);
            Console.WriteLine("Sub: {0}", end - start);
            Console.ReadLine();
        }

        private void SearchUser()
        {
            Console.Write("Enter your search: ");
            Console.ReadLine();
            string searchStr = Console.ReadLine();

            IDocumentSession session = store.OpenSession();
            //chi search dc 128 result, do gioi han request j j do....
            DateTime start = DateTime.Now;
            var user = session.Advanced.LuceneQuery<UserObject>("FullName").Where(string.Format("FullName: *{0}*", searchStr)).ToList();
            //var user = session.Query<UserObject>("FullName").Where(x => x.FullName.StartsWith("*" + searchStr)).ToList();
            
            DateTime end = DateTime.Now;
            
            session.Dispose();

            if (user.Count <= 0)
            {
                Console.WriteLine("Don't any user");
            }
            else
            {
                foreach (var item in user)
                {
                    Console.WriteLine(item.FullName);
                }
                Console.WriteLine("Found: {0} item", user.Count);
            }

            Console.WriteLine("Start: {0}:{1}:{2}:{3}", start.Hour, start.Minute, start.Second, start.Millisecond);
            Console.WriteLine("End: {0}:{1}:{2}:{3}", end.Hour, end.Minute, end.Second, end.Millisecond);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sub: {0}", end - start);
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
        }
    }
}
