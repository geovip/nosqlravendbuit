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
        
        public UserGroupObject Load(string id)
        {
            var userGroup = session.Load<UserGroupObject>(id);
            return userGroup;
        }

        public List<UserGroupObject> GetUnapprove(string groupId)
        {
            var userGroups = session.Query<UserGroupObject>().Where(u => u.GroupId == groupId && u.IsApprove == false);

            foreach (var userGroup in userGroups)
            {
                userGroup.User = session.Load<UserObject>(userGroup.UserId);
            }

            return userGroups.ToList();
        }
    }
}
