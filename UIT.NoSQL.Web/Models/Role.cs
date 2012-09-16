using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Web.Models
{
    public class Role
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        List<GroupRole> ListGroupRole { get; set; }
    }
}