using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Web.Models
{
    public class GroupRoleObject
    {
        public string GroupRoleID { get; set; }
        public string GroupName { get; set; }
        public GroupObject Group { get; set; }

        public List<RoleObject> ListRole { get; set; }
        public List<UserObject> ListUser { get; set; }
    }
}