using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class UserGroupService : IUserGroupService
    {
        private IDocumentSession session;

        public UserGroupService(IDocumentSession session)
        {
            this.session = session;
        }

        public void Save(UserGroupObject userGroup)
        {
            session.Store(userGroup);
            session.SaveChanges();
        }

        public List<UserGroupObject> GetByUser(string userId)
        {
            var userGroups = session.Query<UserGroupObject>().Where(u => u.UserId == userId);
            return userGroups.ToList();
        }
    }
}
