using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface IGroupService
    {
        //GroupObject Load(string id);
        GroupObject LoadByUser(string userId);
        List<GroupObject> GetAll();
        List<GroupObject> GetByUser(string userId);
        void Save(GroupObject group);
        GroupObject Load(string id);
        GroupObject LoadWithUser(string groupID, out List<UserObject> listUser, out List<UserGroupObject> listUserGroup);
        GroupObject LoadWithUser(string id);
        List<GroupObject> Search(string searchStr);
        List<GroupObject> LoadList(string[] arrId);
    }
}
