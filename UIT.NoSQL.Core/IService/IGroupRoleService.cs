using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public enum GroupRoleType
    {
        Manager,
        Member,
        Owner
    }

    public interface IGroupRoleService
    {
        GroupRoleObject LoadByName(GroupRoleType type);
    }
}
