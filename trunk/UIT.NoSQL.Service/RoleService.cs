using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class RoleService : IRoleService
    {
        private IDocumentSession session;

        public RoleService(IDocumentSession session)
        {
            this.session = session;
        }

        //Load Role based on Id
        public RoleObject Load(string id)
        {
            return session.Load<RoleObject>(id);
        }

        // Get all roles
        public List<RoleObject> GetAll()
        {
            var roles = session.Query<RoleObject>();
            return roles.ToList();
        }

        public void Save(RoleObject role)
        {
            session.Store(role);
            session.SaveChanges();
        }

        //Delete a role
        public void Delete(string id)
        {
            var role = Load(id);
            session.Delete<RoleObject>(role);
            session.SaveChanges();
        }
    }
}
