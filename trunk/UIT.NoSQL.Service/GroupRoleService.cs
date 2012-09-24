using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class GroupRoleService : IGroupRoleService
    {
        private IDocumentSession session;

        public GroupRoleService(IDocumentSession session)
        {
            this.session = session;
        }

        //Load GroupRole based on Id
        public GroupRoleObject Load(string id)
        {
            return session.Load<GroupRoleObject>(id);
        }

        // Get all groupRoles
        public List<GroupRoleObject> GetAll()
        {
            var groupRoles = session.Query<GroupRoleObject>();
            return groupRoles.ToList();
        }

        public void Save(GroupRoleObject groupRole)
        {
            session.Store(groupRole);
            session.SaveChanges();
        }

        //Delete a groupRole
        public void Delete(string id)
        {
            var groupRole = Load(id);
            session.Delete<GroupRoleObject>(groupRole);
            session.SaveChanges();
        }
    }
}
