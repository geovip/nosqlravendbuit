using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface IGroupRoleService
    {
        GroupRoleObject Load(string id);
        List<GroupRoleObject> GetAll();
        void Save(GroupRoleObject groupRole);
        void Delete(string id);
    }
}
