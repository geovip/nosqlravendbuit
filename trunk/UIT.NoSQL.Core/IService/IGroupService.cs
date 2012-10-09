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
    }
}
