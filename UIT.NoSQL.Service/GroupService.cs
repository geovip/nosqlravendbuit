using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class GroupService : IGroupService
    {
        private IDocumentSession session;

        public GroupService(IDocumentSession session)
        {
            this.session = session;
        }

        //Load Group based on Id
        public GroupObject Load(string id)
        {
            return session.Load<GroupObject>(id);
        }

        // Get all groups
        public List<GroupObject> GetAll()
        {
            var groups = session.Query<GroupObject>();
            return groups.ToList();
        }

        public void Save(GroupObject group)
        {
            session.Store(group);
            session.SaveChanges();
        }

        //Delete a group
        public void Delete(string id)
        {
            var group = Load(id);
            session.Delete<GroupObject>(group);
            session.SaveChanges();
        }
    }
}
