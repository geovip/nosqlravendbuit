using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Web.Models
{
    public class GroupRole
    {
        public string GroupRoleID { get; set; }
        public string GroupName { get; set; }
        public Group Group { get; set; }

        public List<Role> ListRole { get; set; }
        public List<User> ListUser { get; set; }
    }
}