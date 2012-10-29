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
            string id = GetIdByType(type);
            var groupRole = session.Load<GroupRoleObject>(id);

            return groupRole;
        }

        private string GetIdByType(GroupRoleType type)
        {
            switch (type)
            {
                case GroupRoleType.Manager:
                    return "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
                case GroupRoleType.Owner:
                    return "79C6B725-F787-4FDF-B820-42A21174449D";
                case GroupRoleType.Member:
                    return "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            }

            return string.Empty;
        }
    }
}
