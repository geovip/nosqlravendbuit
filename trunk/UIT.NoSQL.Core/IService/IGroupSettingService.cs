using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface IGroupSettingService
    {
        GroupSettingObject Load(string id);
        List<GroupSettingObject> GetAll();
        void Save(GroupSettingObject groupSetting);
        void Delete(string id);
    }
}
