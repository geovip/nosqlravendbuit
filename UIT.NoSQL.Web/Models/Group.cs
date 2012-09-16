using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Web.Models
{
    public class Group
    {
        public string GroupID  { get; set; }
        public string GroupName { get; set; }

        public List<GroupRole> ListGroupRole { get; set; }
    }
}