using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public enum GroupRoleEnum
    {
        Owner = 0,
        Manager = 1,
        Member = 2
    }

    public class GroupRoleObject
    {
        public string Id { get; set; }
        public string GroupName { get; set; }
        public string IsGeneral { get; set; }

        public GroupRoleObject()
        {
        }
    }
}
