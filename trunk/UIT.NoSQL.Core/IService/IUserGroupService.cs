using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface IUserGroupService
    {
        void Save(UserGroupObject userGroup);
        List<UserGroupObject> GetByUser(string userId);
        UserGroupObject Load(string id);
        List<UserGroupObject> GetUnapprove(string groupId);
        void Delete(string id);
    }
}
