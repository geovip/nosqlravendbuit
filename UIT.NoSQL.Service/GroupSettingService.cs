using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class GroupSettingService : IGroupSettingService
    {
        private IDocumentSession session;

        public GroupSettingService(IDocumentSession session)
        {
            this.session = session;
        }

        //Load GroupSetting based on Id
        public GroupSettingObject Load(string id)
        {
            return session.Load<GroupSettingObject>(id);
        }

        // Get all groupSettings
        public List<GroupSettingObject> GetAll()
        {
            var groupSettings = session.Query<GroupSettingObject>();
            return groupSettings.ToList();
        }

        public void Save(GroupSettingObject groupSetting)
        {
            session.Store(groupSetting);
            session.SaveChanges();
        }

        //Delete a groupSetting
        public void Delete(string id)
        {
            var groupSetting = Load(id);
            session.Delete<GroupSettingObject>(groupSetting);
            session.SaveChanges();
        }
    }
}
