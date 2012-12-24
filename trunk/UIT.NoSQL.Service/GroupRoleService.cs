using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using Raven.Client;

namespace UIT.NoSQL.Service
{
    public class GroupRoleService : IGroupRoleService
    {
        private IDocumentSession session;

        public GroupRoleService(IDocumentSession session)
        {
            this.session = session;
        }

        public GroupRoleObject LoadByName(GroupRoleType type)
        {
            var groupRole = session.Query<GroupRoleObject>().Where(r => r.GroupName == type.ToString()).FirstOrDefault();

            return groupRole;
        }
    }
}
