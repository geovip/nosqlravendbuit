using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public class UserObject
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime CreateDate { get; set; }

        public List<UserGroupObject> ListUserGroup { get; set; }

        public UserObject()
        {
            ListUserGroup = new List<UserGroupObject>();
        }

    }

    public class DenormalizedUser
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
    }
}
