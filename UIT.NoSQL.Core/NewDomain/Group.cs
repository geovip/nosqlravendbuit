using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.NewDomain
{
    public class Group
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }

        public List<DenormalizedTopic> ListTopic { get; set; }
        public List<UserGroup> ListUserGroup { get; set; }
    }

    public class DenormalizedGroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}
