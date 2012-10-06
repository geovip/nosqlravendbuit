using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public class GroupObject
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }

        public List<DenormalizedTopic> ListTopic { get; set; }
        public List<UserGroupObject> ListUserGroup { get; set; }
    }

    public class DenormalizedGroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}
