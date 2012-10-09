using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public class GroupObject
    {
        public string Id { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }

        public List<DenormalizedTopic> ListTopic { get; set; }
        public List<UserGroupObject> ListUserGroup { get; set; }

        public GroupObject()
        {
            ListTopic = new List<DenormalizedTopic>();
            ListUserGroup = new List<UserGroupObject>();
        }
    }

    public class DenormalizedGroup
    {
        public string Id { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}
