using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Web.Models
{
    public class GroupObject
    {
        public string GroupID  { get; set; }
        public string GroupName { get; set; }

        public List<GroupRoleObject> ListGroupRole { get; set; }
    }
}