using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;
using Raven.Abstractions.Commands;

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
            var userGroups = session.Query<UserGroupObject>().Where(u => u.GroupId == groupId && u.IsApprove == UserGroupStatus.JoinRequest);

            foreach (var userGroup in userGroups)
            {
                userGroup.User = session.Load<UserObject>(userGroup.UserId);
            }

            return userGroups.ToList();
        }

        public void UpdateSettingByGroupID(GroupObject group)
        {
            var userGroups = session.Query<UserGroupObject>("ByGroupIdAndJoinDateSortByJoinDate")
                .Customize(x => x.Include<UserGroupObject>(o => o.UserId))
                .Where(x => x.GroupId.Equals(group.Id))
                .ToList();

            List<UserObject> listUsers = new List<UserObject>();
            foreach (var userGroup in userGroups)
            {
                userGroup.GroupName = group.GroupName;
                userGroup.Description = group.Description;

                session.Store(userGroup);
                listUsers.Add(session.Load<UserObject>(userGroup.UserId));
            }

            foreach (var user in listUsers)
            {
                var userGroup = user.ListUserGroup.Where<UserGroupObject>(x => x.GroupId.Equals(group.Id)).FirstOrDefault();
                if (userGroup != null)
                {
                    userGroup.GroupName = group.GroupName;
                    userGroup.Description = group.Description;

                    session.Store(user);
                    break;
                }
            }

            session.SaveChanges();
        }

        public void Delete(string id)
        {
            session.Advanced.Defer(new DeleteCommandData { Key = id });
        }
    }
}
