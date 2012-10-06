using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.NewDomain
{
    public class GroupRole
    {
        public string GroupRoleID { get; set; }
        public string GroupName { get; set; }

        public List<Role> ListRole { get; set; }

        public GroupRole()
        {
            ListRole = new List<Role>();
        }
    }
}
