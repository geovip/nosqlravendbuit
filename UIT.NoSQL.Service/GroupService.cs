using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using Raven.Client;
//using Raven.Client.Linq;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace UIT.NoSQL.Service
{
    public class GroupService : IGroupService
    {
        private IDocumentSession session;

        public GroupService(IDocumentSession session)
        {
            this.session = session;
        }

        public GroupObject Load(string id)
        {
            return session.Load<GroupObject>(id);
        }
        
        public GroupObject LoadWithUser(string id)
        {
            var groupObject = session.Include<GroupObject>(u => u.Id).Load(id);

            foreach (var userGroup in groupObject.ListUserGroup)
            {
                var user = session.Load<UserObject>(userGroup.UserId);
                userGroup.User = user;
            }

            return groupObject;
        }

        public GroupObject LoadByUser(string userId)
        {
            //var userGroup = session.Include<UserGroupObject>(u => u.GroupId).Where(u => u.UserId == userId);
            //return session.Query<GroupObject>().Where(g => g.);
            return null;
        }

        public void Save(GroupObject group)
        {
            session.Store(group);
            session.SaveChanges();
        }

        public List<GroupObject> GetAll()
        {
            var groups = session.Query<GroupObject>();
            return groups.ToList();
        }

        public List<GroupObject> GetByUser(string userId)
        {
            //var groups = session.Query<GroupObject>().Where;
            //return groups.ToList();
            return null;
        }


        public GroupObject LoadWithUser(string groupID, out List<UserObject> listUser, out List<UserGroupObject> listUserGroup)
        {
            listUser = new List<UserObject>();
            listUserGroup = new List<UserGroupObject>();
            var group = session.Include<GroupObject>(u => u.Id).Load(groupID);
            if (group == null)
            {
                return null;
            }

            listUserGroup = group.ListUserGroup;
            foreach (var userGroup in group.ListUserGroup)
	        {
                var user = session.Load<UserObject>(userGroup.UserId);
                listUser.Add(user);
	        }            

            return group;
        }

        public List<GroupObject> Search(string searchStr, int skip, int take, out int totalResult)
        {
            //string str = string.Format("GroupName: *\"{0}\"* OR Description: *\"{0}\"*", searchStr);
            //str = str.Replace("\\","");// OR Description: \"{0}\"
            //"GroupName"
            RavenQueryStatistics stats;
            //var listGroup = session.Advanced.LuceneQuery<GroupObject>("GroupName")
            //    .Statistics(out stats)
            //    .Skip(skip)
            //    .Take(take)
            //    .Where(string.Format("GroupName:*{0}* OR Description:*{0}*", searchStr)).ToList();

            //var listGroup = session.Query<GroupObject>("Groupname")
            //    .Statistics(out stats)
            //    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
            //    .Skip(skip)
            //    .Take(take)
            //    .Where(g => g.GroupName.StartsWith(searchStr) || g.GroupName.StartsWith("*" + searchStr))
            //    .ToList();

            /* doan nay thay thanh doan duoi fulltext search
            var listGroup = session.Query<GroupObject_Search.ReduceResult, GroupObject_Search>()
                .Statistics(out stats)
                .Skip(skip)
                .Take(take)
                .Where(c => c.Query.In((object)searchStr))
                .As<GroupObject>()
                .ToList();

            if (listGroup.Count == 0)
            {
                searchStr = string.Format("*{0}*", searchStr);

                listGroup = session.Query<GroupObject_Search.ReduceResult, GroupObject_Search>()
                .Statistics(out stats)
                .Skip(skip)
                .Take(take)
                .Where(c => c.Query.In((object)searchStr))
                .As<GroupObject>()
                .ToList();
            }
            */
            //Raven.Client.Linq.IRavenQueryable<GroupObject> listGroup;

            List<GroupObject> listGroup = session.Query<GroupObject, GroupObject_Search>()
                .Statistics(out stats)
                .Skip(skip)
                .Take(take)
                .Search(x => x.GroupName, searchStr)
                .ToList();

            if (listGroup.Count == 0)
            {
                searchStr = string.Format("*{0}*", searchStr);

                listGroup = session.Query<GroupObject, GroupObject_Search>()
                .Statistics(out stats)
                .Skip(skip)
                .Take(take)
                .Search(x=>x.GroupName, searchStr)
                .ToList();
            }

            totalResult = stats.TotalResults;

            return listGroup;
        }

        public List<GroupObject> LoadList(string[] arrId)
        {
            //var listGroup = session.Query<GroupObject>().Where(g => arrId.All(s => g.Id.Equals(s))).ToList();
            string str = "Id:" + arrId[0];
            for (int i = 1; i < arrId.Length; i++)
			{
                str += " OR Id:" + arrId[i];
            }
            var listGroup = session.Advanced.LuceneQuery<GroupObject>().Where(str).ToList();
            
            return listGroup;
        }

        public bool SendEmail(List<UserGroupObject> listUser, string subject, string body)
        {
            bool result = false;
            string from = System.Configuration.ConfigurationManager.AppSettings["email.username"];
            string pass = System.Configuration.ConfigurationManager.AppSettings["email.password"];

            var threadSendEmail = new ThreadSendEmail(from, pass, listUser, subject, body, session);

            Thread sendEmail = new Thread(new ThreadStart(threadSendEmail.Run));
            sendEmail.Start();

            return result;
        }
    }

    public class ThreadSendEmail
    {
        private string from, pass, subject, body;
        private List<UserGroupObject> listUser;
        private IDocumentSession session;

        public ThreadSendEmail(string from, string pass, List<UserGroupObject> listUser, string subject, string body, IDocumentSession session)
        {
            this.from = from;
            this.pass = pass;
            this.listUser = listUser;
            this.subject = subject;
            this.body = body;
            this.session = session;
        }

        public void Run()
        {
            try
            {
                SmtpClient host = new SmtpClient("smtp.gmail.com", 587);
                host.EnableSsl = true;
                host.DeliveryMethod = SmtpDeliveryMethod.Network;
                host.UseDefaultCredentials = false;
                host.Credentials = new NetworkCredential(from, pass);

                MailMessage mail = null;
                UserObject userObject = null;
                foreach (var userGroup in listUser)
                {
                    if (userGroup.IsReceiveEmail)
                    {
                        userObject = session.Load<UserObject>(userGroup.UserId);
                        mail = new MailMessage(from, userObject.Email, subject, body);
                        mail.IsBodyHtml = true;
                        host.Send(mail);
                    }
                }

            }
            catch (Exception)
            {
            }
        }
    }
    
    public class GroupObject_Search : AbstractIndexCreationTask<GroupObject>
        {
            public GroupObject_Search()
            {
                Map = groups => from g in groups
                                select new
                                {
                                    g.GroupName,
                                    g.Description,
                                    g.CreateBy.FullName
                                };
                Indexes.Add(x => x.GroupName, FieldIndexing.Analyzed);
                Indexes.Add(x=> x.Description, FieldIndexing.Analyzed);
            }
        }

    //public class GroupObject_Search : AbstractIndexCreationTask<GroupObject, GroupObject_Search.ReduceResult>
    //{
    //    public class ReduceResult
    //    {
    //        public object[] Query { get; set; }
    //    }

    //    public GroupObject_Search()
    //    {
    //        Map = groups => from g in groups
    //                        select new
    //                        {
    //                            Query = new[]
    //                                {
    //                                    g.Id,
    //                                    g.GroupName,
    //                                    g.Description,
    //                                    g.CreateBy.FullName
    //                                }
    //                        };
    //    }
    //}
}
