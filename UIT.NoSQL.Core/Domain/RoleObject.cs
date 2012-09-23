using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Core.Domain
{
    public class RoleObject
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        List<GroupRoleObject> ListGroupRole { get; set; }
    }
}