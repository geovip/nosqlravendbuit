using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Core.IService
{
    public interface IUserService
    {
        UserObject Load(string id);
        UserObject LoadByUserName(string username);
        List<UserObject> GetAll();
        void Save(UserObject user);
        void Delete(string id);
        bool CheckLoginSuccess(string username, string password);
    }
}
