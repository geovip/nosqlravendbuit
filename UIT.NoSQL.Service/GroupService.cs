using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using Raven.Client;
using Raven.Client.Linq;

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
            var listGroup = session.Advanced.LuceneQuery<GroupObject>("GroupName")
                .Statistics(out stats)
                .Skip(skip)
                .Take(take)
                .Where(string.Format("GroupName:*{0}* OR Description:*{0}*", searchStr)).ToList();
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
    }
}
