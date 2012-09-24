using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class UserService : IUserService
    {
        private IDocumentSession session;

        public UserService(IDocumentSession session)
        {
            this.session = session;
        }

        //Load User based on Id
        public UserObject Load(string id)
        {
            return session.Load<UserObject>(id);
        }

        // Get all users
        public List<UserObject> GetAll()
        {
            var users = session.Query<UserObject>();
            return users.ToList();
        }

        public void Save(UserObject user)
        {
            session.Store(user);
            session.SaveChanges();
        }

        //Delete a user
        public void Delete(string id)
        {
            var user = Load(id);
            session.Delete<UserObject>(user);
            session.SaveChanges();
        }
    }
}
