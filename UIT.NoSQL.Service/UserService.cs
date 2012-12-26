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

        //Load UserObject based on Id
        public UserObject Load(string id)
        {
            return session.Load<UserObject>(id);
        }

        //Load UserObject based on UserName
        public UserObject LoadByUserName(string username)
        {
            return session.Query<UserObject>().Where(u => u.UserName == username).SingleOrDefault();
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

        //Check Login success or not
        public bool CheckLoginSuccess(string username, string password)
        {
            var userLogin = (from user in session.Query<UserObject>("LoginIndex")
                             where user.UserName.Equals(username) && user.Password.Equals(password)
                            select user).SingleOrDefault();
            if (userLogin == null)
                return false;
            return true;
        }
    }
}
