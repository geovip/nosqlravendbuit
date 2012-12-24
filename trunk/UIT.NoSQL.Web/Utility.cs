﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIT.NoSQL.Core.Domain;
using Raven.Client;
using System.Security.Cryptography;
using System.Text;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using System.IO;
using UIT.NoSQL.Service;

namespace UIT.NoSQL.Web
{
    public class Utility
    {
        private IDocumentSession session;

        public Utility(IDocumentSession session)
        {
            this.session = session;
        }

        public void Initialized()
        {
            List<GroupRoleObject> listRole = new List<GroupRoleObject>();
            List<UserObject> listUser = new List<UserObject>();
            List<UserGroupObject> listUserGroup = new List<UserGroupObject>();
            List<GroupObject> listGroup = new List<GroupObject>();
            
            //create data sample
            GroupRoleObject groupRole = null;
            groupRole = new GroupRoleObject();
            groupRole.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRole.GroupName = "Manager";
            groupRole.IsGeneral = MvcApplication.ServerGeneral;
            session.Store(groupRole);

            groupRole = new GroupRoleObject();
            groupRole.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRole.GroupName = "Member";
            groupRole.IsGeneral = MvcApplication.ServerGeneral;
            session.Store(groupRole);

            groupRole = new GroupRoleObject();
            groupRole.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRole.GroupName = "Owner";
            groupRole.IsGeneral = MvcApplication.ServerGeneral;
            session.Store(groupRole);

            //user
            UserObject userObject = null;

            userObject = new UserObject();
            userObject.Id = "D035A3B8-961D-4DA0-827A-D58E8FCE3832";
            userObject.FullName = "Duong Than Dan";
            userObject.UserName = "sa";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "duongthandan@gmail.com";
            userObject.Region = MvcApplication.ServerRegion[0];
            session.Store(userObject);

            //group
            GroupObject groupObject = null;
            groupObject = new GroupObject();
            groupObject.Id = "2B857081-5D44-4CDB-A5DD-D34D753D0A7A";
            groupObject.GroupName = "ASP.NET MVC 4";
            groupObject.Description = "ASP.NET MVC 4";
            groupObject.IsPublic = false;
            groupObject.CreateDate = DateTime.Now;
            groupObject.CreateBy = userObject;
            groupObject.NewEvent = new GroupEvent();
            groupObject.NewEvent.Title = "New group";
            groupObject.NewEvent.CreateDate = groupObject.CreateDate;
            groupObject.NewEvent.CreateBy = userObject.FullName;
            session.Store(groupObject);

            var userGroup = new UserGroupObject();
            userGroup.Id = "565C2563-0CA2-4993-B322-1D05C885A996";
            userGroup.UserId = userObject.Id;
            userGroup.GroupId = groupObject.Id;
            userGroup.GroupName = groupObject.GroupName;
            userGroup.Description = groupObject.Description;
            userGroup.IsApprove = UserGroupStatus.Approve;
            userGroup.JoinDate = DateTime.Now;
            userGroup.GroupRole = groupRole;

            groupObject.ListUserGroup.Add(userGroup);
            userObject.ListUserGroup.Add(userGroup);

            session.Store(userGroup);
            session.Store(groupObject);


            RandomData randomData = new RandomData();
            for (int i = 0; i < 7; i++)
            {
                groupObject = new GroupObject();
                groupObject.Id = Guid.NewGuid().ToString();
                groupObject.GroupName = randomData.RandomString();
                groupObject.Description = randomData.RandomString() + " " + randomData.RandomString();
                groupObject.IsPublic = false;
                groupObject.CreateDate = DateTime.Now;
                groupObject.CreateBy = userObject;
                groupObject.NewEvent = new GroupEvent();
                groupObject.NewEvent.Title = "New group";
                groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                groupObject.NewEvent.CreateBy = userObject.FullName;
                session.Store(groupObject);

                var userGroupRandom = new UserGroupObject();
                userGroupRandom.Id = Guid.NewGuid().ToString();
                userGroupRandom.UserId = userObject.Id;
                userGroupRandom.GroupId = groupObject.Id;
                userGroupRandom.GroupName = groupObject.GroupName;
                userGroupRandom.Description = groupObject.Description;
                userGroupRandom.IsApprove = UserGroupStatus.Approve;
                userGroupRandom.JoinDate = DateTime.Now;
                userGroupRandom.GroupRole = groupRole;

                groupObject.ListUserGroup.Add(userGroupRandom);
                userObject.ListUserGroup.Add(userGroupRandom);

                session.Store(userGroupRandom);
                session.Store(groupObject);
            }

            session.Store(userObject);
            session.SaveChanges();

            //user
            userObject = new UserObject();
            userObject.Id = "F4D45AD1-D581-425C-A058-799AFA51FE01";
            userObject.FullName = "Bui Ngoc Huy";
            userObject.UserName = "aa";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "huyuit@gmail.com";
            userObject.Region = MvcApplication.ServerRegion[1];

            session.Store(userObject);

            for (int i = 0; i < 12; i++)
            {
                groupObject = new GroupObject();
                groupObject.Id = Guid.NewGuid().ToString();
                groupObject.GroupName = randomData.RandomString();
                groupObject.Description = randomData.RandomString() + " " + randomData.RandomString();
                groupObject.IsPublic = false;
                groupObject.CreateDate = DateTime.Now;
                groupObject.CreateBy = userObject;
                groupObject.NewEvent = new GroupEvent();
                groupObject.NewEvent.Title = "New group";
                groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                groupObject.NewEvent.CreateBy = userObject.FullName;
                session.Store(groupObject);

                var userGroupRandom = new UserGroupObject();
                userGroupRandom.Id = Guid.NewGuid().ToString();
                userGroupRandom.UserId = userObject.Id;
                userGroupRandom.GroupId = groupObject.Id;
                userGroupRandom.GroupName = groupObject.GroupName;
                userGroupRandom.Description = groupObject.Description;
                userGroupRandom.IsApprove = UserGroupStatus.Approve;
                userGroupRandom.JoinDate = DateTime.Now;
                userGroupRandom.GroupRole = groupRole;

                groupObject.ListUserGroup.Add(userGroupRandom);
                userObject.ListUserGroup.Add(userGroupRandom);

                session.Store(userGroupRandom);
                session.Store(groupObject);
            }

            session.Store(userObject);



            //user
            userObject = new UserObject();
            userObject.Id = "3FDA3031-C5D0-4A7C-87EE-F0AF91EAC76E";
            userObject.FullName = "qq";
            userObject.UserName = "qq";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "qq@gmail.com";
            userObject.Region = MvcApplication.ServerRegion[2];
            session.Store(userObject);

            userObject = new UserObject();
            userObject.Id = "592D9EDD-A3C9-4ACC-A3EF-5C88823A2474";
            userObject.FullName = "ww";
            userObject.UserName = "ww";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "ww@gmail.com";
            userObject.Region = MvcApplication.ServerRegion[2];
            session.Store(userObject);

            userObject = new UserObject();
            userObject.Id = "E0548546-17A0-418D-9832-4D5887536268";
            userObject.FullName = "ww";
            userObject.UserName = "ww";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "ee@gmail.com";
            userObject.Region = MvcApplication.ServerRegion[2];
            session.Store(userObject);

            session.SaveChanges();

            ////save data
            //foreach (var item in listRole)
            //{
            //    session.Store(item);
            //}
            //session.SaveChanges();

            //foreach (var item in listUser)
            //{
            //    session.Store(item);
            //}
            //session.SaveChanges();

            //foreach (var item in listGroup)
            //{
            //    session.Store(item);
            //}
            //session.SaveChanges();

            //foreach (var item in listUserGroup)
            //{
            //    session.Store(item);
            //}
            //session.SaveChanges();

            //create index
            foreach (var store in MvcApplication.documentStores)
            {
                InitalIndex(store);
            }
            new GroupObject_Search().Execute(MvcApplication.documentStoreShard);
        }

        private void InitalIndex(IDocumentStore documentStore)
        {
            var session = documentStore.OpenSession();

            documentStore.DatabaseCommands.DeleteIndex("GroupName");
            documentStore.DatabaseCommands.PutIndex("GroupName", new IndexDefinitionBuilder<GroupObject>
            {
                Map = gr => from g in gr
                            select new { g.GroupName, g.Description },
                Indexes =
                {
                    { x => x.GroupName, FieldIndexing.Analyzed}
                }
            });
        }

        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }

    public class RandomData
    {
        private Random random;

        public RandomData()
        {
            random = new Random((int)DateTime.Now.Ticks);
        }

        public string RandomString()
        {
            string path = Path.GetRandomFileName() + " " + Path.GetRandomFileName();
            path = path.Replace(".", "");
            return path;
        }
    }
}