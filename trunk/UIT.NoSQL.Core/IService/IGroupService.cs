using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface IGroupService
    {
        GroupObject Load(string id);
        List<GroupObject> GetAll();
        void Save(GroupObject group);
        void Delete(string id);
    }
}
