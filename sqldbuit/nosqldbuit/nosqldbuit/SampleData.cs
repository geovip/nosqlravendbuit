using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIT.NoSQL.Core.Domain;

namespace nosqldbuit
{
    public class SampleData
    {
        public List<UserObject> CreateUser(int count)
        {
            RandomData randomData = new RandomData();
            List<UserObject> listUser = new List<UserObject>();
            UserObject user = null;

            for (int i = 0; i < count; i++)
            {
                user = new UserObject();
                user.Id = Guid.NewGuid().ToString();
                user.UserName = randomData.RandomString();
                user.Password = randomData.RandomString();
                user.Email = randomData.RandomString();
                user.FullName = randomData.RandomString();
                user.CreateDate = DateTime.Now;

                listUser.Add(user);
            }

            return listUser;
        }
    }
}
