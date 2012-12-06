using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sqldbuit
{
    class Program
    {
        private UITSQLDBDataContext db;

        public Program()
        {
            db = new UITSQLDBDataContext();
        }

        private void Menu()
        {
            Console.WriteLine("1. Insert data");
            Console.WriteLine("2. Search data");
            Console.WriteLine("3. Delete all data");
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
                        DeleteAllUser();
                        break;
                }
            } while (chose != 52);
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
            List<User> listUser = sampleDate.CreateUser(count);
            
            DateTime start = DateTime.Now;

            db.Users.InsertAllOnSubmit(listUser);
            db.SubmitChanges();

            DateTime end = DateTime.Now;

            Console.WriteLine("Start: {0}:{1}:{2}:{3}", start.Hour, start.Minute, start.Second, start.Millisecond);
            Console.WriteLine("End: {0}:{1}:{2}:{3}", end.Hour, end.Minute, end.Second, end.Millisecond);
            Console.WriteLine("Sub: {0}", end - start);
            Console.ReadLine();
        }

        private void DeleteAllUser()
        {
            DateTime start = DateTime.Now;

            db.Users.DeleteAllOnSubmit(db.Users);
            db.SubmitChanges();
            
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

            DateTime start = DateTime.Now;
            var user = (from u in db.Users where u.FullName.Contains(searchStr) select u).ToList();
            DateTime end = DateTime.Now;

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