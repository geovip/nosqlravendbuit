using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sqldbuit
{
    public class SampleData
    {
        public List<User> CreateUser(int count)
        {
            RandomData randomData = new RandomData();
            List<User> listUser = new List<User>();
            User user = null;

            for (int i = 0; i < count; i++)
            {
                user = new User();
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

        public List<UserGroup> CreateUserGroup(int count)
        {
            RandomData randomData = new RandomData();
            List<UserGroup> listUserGroup = new List<UserGroup>();
            UserGroup userGroup = null;

            for (int i = 0; i < count; i++)
            {
                userGroup = new UserGroup();
                userGroup.Id = Guid.NewGuid().ToString();
                //user.UserName = randomData.RandomString();
                //user.Password = randomData.RandomString();
                //user.Email = randomData.RandomString();
                //user.FullName = randomData.RandomString();
                //user.CreateDate = DateTime.Now;

                listUserGroup.Add(userGroup);
            }

            return listUserGroup;
        }
    }
}
