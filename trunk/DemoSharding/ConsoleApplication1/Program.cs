using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Shard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DemoSharding
{
    class Program
    {
        private IDocumentStore documentStore;

        public Program()
        {
            Initial();
        }

        public void Initial()
        {
            var shards = new Dictionary<string, IDocumentStore>
                {
                    {CompanyEnum.Asia.ToString(), new DocumentStore {Url = "http://localhost:8081"}},
                    {CompanyEnum.MiddleEast.ToString(), new DocumentStore {Url = "http://localhost:8082"}},
                    {CompanyEnum.America.ToString(), new DocumentStore {Url = "http://localhost:8083"}},
                };

            var shardStrategy = new ShardStrategy(shards);
                //.ShardingOn<Company>(company => company.Region);
                //.ShardingOn<User>(u => u.CompanyId);
            shardStrategy.ShardResolutionStrategy = new DefaultShardResolutionStrategy(shards.Keys, shardStrategy);

            documentStore = new ShardedDocumentStore(shardStrategy).Initialize();
        }

        public void InitialData(CompanyEnum region)
        {
            var session = documentStore.OpenSession();
            RandomData rd = new RandomData();
            //Company company = null;

            //for (int i = 0; i < 10000; i++)
            //{
            //    company = new Company();
            //    company.Id = Guid.NewGuid().ToString();
            //    company.Name = rd.RandomString() + " " + rd.RandomString();
            //    company.Region = region.ToString();

            //    session.Store(company);
            //}

            //session.SaveChanges();

            User user = null;
            for (int i = 0; i < 10000; i++)
            {
                user = new User();
                user.Id = Guid.NewGuid().ToString();
                user.Name = rd.RandomString() + " " + rd.RandomString();

                session.Store(user);
            }
            session.SaveChanges();
        }

        public void Run()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var session = documentStore.OpenSession();
            //get all, should automagically retrieve from each shard
            var allCompanies = session.Query<Company>()
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .Where(company => company.Region == CompanyEnum.Asia.ToString())
                .ToArray();

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine(allCompanies.Length);
            //foreach (var company in allCompanies)
            //    Console.WriteLine(company.Name);
        }

        public void Search()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var session = documentStore.OpenSession();
            //get all, should automagically retrieve from each shard
            var allCompanies = session.Query<Company>()
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .Where(company => company.Id == "MiddleEast/fafcc66e-4db5-4e2a-9832-5176a7fdee7d")
                .FirstOrDefault();

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine(allCompanies == null ? "null" : allCompanies.Name);
            //foreach (var company in allCompanies)
            //    Console.WriteLine(company.Name);
        }

        public void Insert()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var session = documentStore.OpenSession();
            
            var user = new User();
            user.Id = Guid.NewGuid().ToString();
            user.Name = new RandomData().RandomString();
            //user.CompanyId = "Asia/ba85bd70-d5b2-4693-a08e-d525e223ffde";

            session.Store(user);
            session.SaveChanges();
            session.Dispose();

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            //foreach (var company in allCompanies)
            //    Console.WriteLine(company.Name);
        }

        static void Main(string[] args)
        {
            Program p = new Program();

            p.InitialData(CompanyEnum.Asia);
            p.InitialData(CompanyEnum.MiddleEast);
            //p.InitialData(CompanyEnum.America);

            //p.Insert();
            p.documentStore.Dispose();

            Console.ReadLine();
        }
    }

    public enum CompanyEnum
    {
        Asia,
        MiddleEast,
        America
    }

    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
    }
}
