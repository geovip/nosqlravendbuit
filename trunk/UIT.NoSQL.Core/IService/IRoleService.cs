using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface IRoleService
    {
        RoleObject Load(string id);
        List<RoleObject> GetAll();
        void Save(RoleObject role);
        void Delete(string id);
    }
}
