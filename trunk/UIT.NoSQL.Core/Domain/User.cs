using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public interface IUserObjectDocument
    {
        string Id { get; set; }
        string FullName { get; set; }
    }

    public class UserObject: IUserObjectDocument
    {
        public string Id { get; set; }
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

    public class DenormalizedUser<T> where T : IUserObjectDocument
    {
        public string Id { get; set; }
        public string FullName { get; set; }

        public static implicit operator DenormalizedUser<T>(T doc)
        {
            return new DenormalizedUser<T>
            {
                Id = doc.Id,
                FullName = doc.FullName
            };
        }
    }
}
