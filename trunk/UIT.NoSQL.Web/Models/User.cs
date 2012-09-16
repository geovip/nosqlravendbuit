using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Web.Models
{
    public class User
    {
        public string ID {get;set;}
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }

        public List<GroupRole> ListGroupRole { get; set; }
    }
}