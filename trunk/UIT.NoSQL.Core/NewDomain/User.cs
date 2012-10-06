using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.NewDomain
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime CreateDate { get; set; }

        public List<UserGroup> ListUserGroup { get; set; }

        public User()
        {
            ListUserGroup = new List<UserGroup>();
        }

    }

    public class DenormalizedUser
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
    }
}
