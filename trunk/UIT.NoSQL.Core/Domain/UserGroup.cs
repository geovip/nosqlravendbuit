using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public enum UserGroupStatus
    {
        JoinRequest = 0,
        Approve = 1,
        Reject = 2
    }

    public class UserGroupObject
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UserObject User { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool IsReceiveEmail { get; set; }
        public DateTime JoinDate { get; set; }
        public UserGroupStatus IsApprove { get; set; }

        public GroupRoleObject GroupRole { get; set; }

        public UserGroupObject()
        {
        }
    }
}
