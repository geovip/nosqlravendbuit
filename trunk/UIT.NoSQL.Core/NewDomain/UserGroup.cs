using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.NewDomain
{
    public class UserGroup
    {
        public string UserGroupId { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }

        public List<GroupRole> ListGroupRole { get; set; }

        public UserGroup()
        {
            ListGroupRole = new List<GroupRole>();
        }
    }
}
