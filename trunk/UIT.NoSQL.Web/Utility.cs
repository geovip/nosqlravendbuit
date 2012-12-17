using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIT.NoSQL.Core.Domain;
using Raven.Client;
using System.Security.Cryptography;
using System.Text;

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
            //role
            GroupRoleObject groupRole = null;

            groupRole = new GroupRoleObject();
            groupRole.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRole.GroupName = "Manager";
            session.Store(groupRole);

            groupRole = new GroupRoleObject();
            groupRole.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRole.GroupName = "Owner";
            session.Store(groupRole);

            groupRole = new GroupRoleObject();
            groupRole.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRole.GroupName = "Member";
            session.Store(groupRole);

            //user
            UserObject userObject = null;

            userObject = new UserObject();
            userObject.Id = "D035A3B8-961D-4DA0-827A-D58E8FCE3832";
            userObject.FullName = "Duong Than Dan";
            userObject.UserName = "sa";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "duongthandan@gmail.com";
            session.Store(userObject);

            userObject = new UserObject();
            userObject.Id = "F4D45AD1-D581-425C-A058-799AFA51FE01";
            userObject.FullName = "Bui Ngoc Huy";
            userObject.UserName = "aa";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "huyuit@gmail.com";
            session.Store(userObject);

            userObject = new UserObject();
            userObject.Id = "{3FDA3031-C5D0-4A7C-87EE-F0AF91EAC76E}";
            userObject.FullName = "qq";
            userObject.UserName = "qq";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "qq@gmail.com";
            session.Store(userObject);

            userObject = new UserObject();
            userObject.Id = "{592D9EDD-A3C9-4ACC-A3EF-5C88823A2474}";
            userObject.FullName = "ww";
            userObject.UserName = "ww";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "ww@gmail.com";
            session.Store(userObject);

            userObject = new UserObject();
            userObject.Id = "{E0548546-17A0-418D-9832-4D5887536268}";
            userObject.FullName = "ww";
            userObject.UserName = "ww";
            userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
            userObject.Email = "ee@gmail.com";
            session.Store(userObject);

            ////group
            //GroupObject groupObject = null;

            //groupObject = new GroupObject();
            //groupObject.Id = "";
            //groupObject.GroupName = "group 1";
            //groupObject.Description = "this is group 1";
            //groupObject.IsPublic = true;
            //groupObject.CreateDate = DateTime.Now;
            //session.Store(groupObject);

            session.SaveChanges();
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
}