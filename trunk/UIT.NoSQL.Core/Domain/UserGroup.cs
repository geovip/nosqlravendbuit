using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public class UserGroupObject
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UserObject User { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsApprove { get; set; }

        public List<GroupRoleObject> ListGroupRole { get; set; }

        public UserGroupObject()
        {
            ListGroupRole = new List<GroupRoleObject>();
        }
    }
}
