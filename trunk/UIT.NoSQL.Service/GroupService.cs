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
        private IDocumentStore[] documentStores;

        public GroupService(IDocumentSession session, IDocumentStore[] documentStores)
        {
            this.session = session;
            this.documentStores = documentStores;
        }

        public GroupObject Load(string id)
        {
            return session.Load<GroupObject>(id);
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

        public List<GroupObject> GetTenGroupPublic()
        {
            var groups = session.Query<GroupObject>().Where(x => x.IsPublic == true).Take(10);
            return groups.ToList();
        }

        public GroupObject LoadWithUser(string groupID)
        {
            var group = session.Include<GroupObject>(u => u.ListUserGroup).Load(groupID);
            
            var userGroups = session.Query<UserGroupObject>()
                .Customize(x => x.Include<UserGroupObject>(o => o.UserId))
                .Where(x => x.GroupId.Equals(groupID))
                .ToList();

            foreach (var userGroup in userGroups)
            {
                userGroup.User = session.Load<UserObject>(userGroup.UserId);
            }

            group.ListUserGroup = userGroups;

            return group;
        }

        public List<GroupObject> Search(string searchStr, out int totalResult)
        {
            RavenQueryStatistics stats;
            totalResult = 0;

            List<GroupObject> listGroup;

            listGroup = session.Query<GroupObject_Search_NotAnalyed.ReduceResult, GroupObject_Search_NotAnalyed>()
                .Statistics(out stats)
                .Where(c => c.Query.Equals((object)searchStr))
                //.Search(x => x.GroupName, searchStr)
                .As<GroupObject>()
                .ToList();

            if (stats.TotalResults == 0)
            {
                listGroup = session.Query<GroupObject, GroupObject_Search>()
                 .Statistics(out stats)
                .Search(x => x.GroupName, searchStr)
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
                                    //g.Description,
                                    //g.CreateBy.FullName
                            };
            Indexes.Add(x => x.GroupName, FieldIndexing.Analyzed);
            //Indexes.Add(x => x.Description, FieldIndexing.Analyzed);
            //Indexes.Add(x => x.CreateBy.FullName, FieldIndexing.Analyzed);
        }
    }

    public class GroupObject_Search_NotAnalyed : AbstractIndexCreationTask<GroupObject>
    {
        public class ReduceResult
        {
            public object[] Query { get; set; }
        }

        public GroupObject_Search_NotAnalyed()
        {
            Map = groups => from g in groups
                            select new
                            {
                                Query = new[]
                                    {
                                        g.GroupName
                                    }
                            };
        }
    }
}
