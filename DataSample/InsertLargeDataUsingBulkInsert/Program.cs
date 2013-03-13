using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UIT.NoSQL.Core.Domain;

namespace InsertLargeDataUsingBulkInsert
{
    class Program
    {
        public static string STR_DATA_SERVER_GROUPS = "F:\\RavenServers-2230\\Data\\Groups\\";
        public static string databaseName = "UITNoSQLDB2";

        public static IDocumentStore documentStore1, documentStore2, documentStore3;
        public static GroupRoleObject groupRoleManager, groupRoleOwner, groupRoleMember;
        public static string[] regions = new string[] { "Asia", "MiddleEast", "America" };
        public static DataTable dt;

        static void Main(string[] args)
        {
            Console.WriteLine("Inserting...!");
            Stopwatch st = new Stopwatch();
            st.Start();

            documentStore1 = new DocumentStore { Url = "http://localhost:8081/", DefaultDatabase = databaseName};
            documentStore1.Conventions.IdentityPartsSeparator = "-";
            documentStore1.Initialize();
            documentStore2 = new DocumentStore { Url = "http://localhost:8082/", DefaultDatabase = databaseName };
            documentStore2.Conventions.IdentityPartsSeparator = "-";
            documentStore2.Initialize();
            documentStore3 = new DocumentStore { Url = "http://localhost:8083/", DefaultDatabase = databaseName };
            documentStore3.Conventions.IdentityPartsSeparator = "-";
            documentStore3.Initialize();

            

            //var session3 = documentStore3.OpenSession();
            //groupRoleManager = new GroupRoleObject();
            //groupRoleManager.Id = "America-7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            //groupRoleManager.GroupName = "Manager";
            //groupRoleManager.IsGeneral = "America";
            //session3.Store(groupRoleManager);

            //groupRoleMember = new GroupRoleObject();
            //groupRoleMember.Id = "America-9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            //groupRoleMember.GroupName = "Member";
            //groupRoleMember.IsGeneral = "America";
            //session3.Store(groupRoleMember);

            //groupRoleOwner = new GroupRoleObject();
            //groupRoleOwner.Id = "America-79C6B725-F787-4FDF-B820-42A21174449D";
            //groupRoleOwner.GroupName = "Owner";
            //groupRoleOwner.IsGeneral = "America";
            //session3.Store(groupRoleOwner);
            //session3.Dispose();

            //dt = GetListFullName(300);

            //// Asia
            //InsertDataToParticular(regions[0], documentStore1);
            // Middle East
            //InsertDataToParticular(regions[1], documentStore2);
            // America
            //InsertDataToParticular(regions[2], documentStore3);

            InitIndexes();

            #region doc danh sach group len tu file xml
            // doc du lieu danh sách Groups tu ListGroups.xml
            //string groupsInfoXmlFilePath = STR_DATA_SERVER_GROUPS + "ListGroups.xml";
            //if (File.Exists(groupsInfoXmlFilePath))
            //{
            //    XElement xmlGroupRSS = XElement.Load(groupsInfoXmlFilePath);
            //    List<Group> listGroups = (from u in xmlGroupRSS.Elements("Group")
            //                              select new Group
            //                              {
            //                                  GroupName = u.Element("GroupName").Value,
            //                              }).ToList();

            //}
            #endregion

            st.Stop();
            Console.WriteLine("Insert Data Sample Success!");
            Console.WriteLine("Time elapsed: {0}", st.Elapsed);
        }

        public static void InitIndexes()
        {
            IndexCreation.CreateIndexes(typeof(LoginIndex).Assembly, documentStore1);
            IndexCreation.CreateIndexes(typeof(GroupIndex).Assembly, documentStore1);
            IndexCreation.CreateIndexes(typeof(GroupRoleIndex).Assembly, documentStore1);
            IndexCreation.CreateIndexes(typeof(ByGroupIdAndJoinDateSortByJoinDate).Assembly, documentStore1);
            IndexCreation.CreateIndexes(typeof(GetUserGroupByUserIdGroupId).Assembly, documentStore1);
            IndexCreation.CreateIndexes(typeof(GroupObject_ByIsPublic).Assembly, documentStore1);
            IndexCreation.CreateIndexes(typeof(GroupObject_Search).Assembly, documentStore1);
            IndexCreation.CreateIndexes(typeof(GroupObject_Search_NotAnalyed).Assembly, documentStore1);

            IndexCreation.CreateIndexes(typeof(LoginIndex).Assembly, documentStore2);
            IndexCreation.CreateIndexes(typeof(GroupIndex).Assembly, documentStore2);
            IndexCreation.CreateIndexes(typeof(GroupRoleIndex).Assembly, documentStore2);
            IndexCreation.CreateIndexes(typeof(ByGroupIdAndJoinDateSortByJoinDate).Assembly, documentStore2);
            IndexCreation.CreateIndexes(typeof(GetUserGroupByUserIdGroupId).Assembly, documentStore2);
            IndexCreation.CreateIndexes(typeof(GroupObject_ByIsPublic).Assembly, documentStore2);
            IndexCreation.CreateIndexes(typeof(GroupObject_Search).Assembly, documentStore2);
            IndexCreation.CreateIndexes(typeof(GroupObject_Search_NotAnalyed).Assembly, documentStore2);

            IndexCreation.CreateIndexes(typeof(LoginIndex).Assembly, documentStore3);
            IndexCreation.CreateIndexes(typeof(GroupIndex).Assembly, documentStore3);
            IndexCreation.CreateIndexes(typeof(GroupRoleIndex).Assembly, documentStore3);
            IndexCreation.CreateIndexes(typeof(ByGroupIdAndJoinDateSortByJoinDate).Assembly, documentStore3);
            IndexCreation.CreateIndexes(typeof(GetUserGroupByUserIdGroupId).Assembly, documentStore3);
            IndexCreation.CreateIndexes(typeof(GroupObject_ByIsPublic).Assembly, documentStore3);
            IndexCreation.CreateIndexes(typeof(GroupObject_Search).Assembly, documentStore3);
            IndexCreation.CreateIndexes(typeof(GroupObject_Search_NotAnalyed).Assembly, documentStore3);
        }

        public class LoginIndex : AbstractIndexCreationTask<UserObject>
        {
            public LoginIndex()
            {
                Map = users => from user in users
                               select new { user.Id, user.UserName, user.Password };
            }
        }

        public class GroupIndex : AbstractIndexCreationTask<GroupObject>
        {
            public GroupIndex()
            {
                Map = groups => from g in groups
                                select new { g.Id };
            }
        }

        public class GroupRoleIndex : AbstractIndexCreationTask<GroupRoleObject>
        {
            public GroupRoleIndex()
            {
                Map = groupRoles => from gr in groupRoles
                                    select new { gr.GroupName };
            }
        }

        public class GroupObject_ByIsPublic : AbstractIndexCreationTask<GroupObject>
        {
            public GroupObject_ByIsPublic()
            {
                Map = groupObjects => from g in groupObjects
                                      select new { g.IsPublic };
            }
        }

        public class ByGroupIdAndJoinDateSortByJoinDate : AbstractIndexCreationTask<UserGroupObject>
        {
            public ByGroupIdAndJoinDateSortByJoinDate()
            {
                {
                    Map = groups => from g in groups
                                    select new { g.GroupId, g.JoinDate };
                }
            }
        }

        public class GetUserGroupByUserIdGroupId : AbstractIndexCreationTask<UserGroupObject>
        {
            public GetUserGroupByUserIdGroupId()
            {
                {
                    Map = userGroups => from ug in userGroups
                                        select new { ug.UserId, ug.GroupId };
                }
            }
        }

        public class GroupObject_Search : AbstractIndexCreationTask<GroupObject>
        {
            public GroupObject_Search()
            {
                Map = groups => from g in groups
                                select new
                                {
                                    g.GroupName,
                                    g.Description,
                                    g.CreateBy.FullName
                                };
                Indexes.Add(x => x.GroupName, FieldIndexing.Analyzed);
                Indexes.Add(x => x.Description, FieldIndexing.Analyzed);
            }
        }

        public class GroupObject_Search_NotAnalyed : AbstractIndexCreationTask<GroupObject>
        {
            public class ReduceResult
            {
                public object[] Query { get; set; }
            }

            public GroupObject_Search_NotAnalyed()
            {
                Map = groups => from g in groups
                                select new
                                {
                                    Query = new[]
                                    {
                                        g.GroupName
                                    }
                                };
            }
        }

        // luu du lieu xuong tung server
        public static void InsertDataToParticular(string region, IDocumentStore documentStore)
        {
            int numberOfGroups = 100, numberOfTopicsForEachGroup = 50, numberOfCommentsForEachTopic = 100;
            int numberOfUsers = 300;

            List<UserObject> listUserObject = new List<UserObject>();
            List<UserObject> list50MemberFirst = new List<UserObject>();
            List<GroupObject> listGroupObject = new List<GroupObject>();
            List<UserGroupObject> listUserGroupObject = new List<UserGroupObject>();
            List<TopicObject> listTopicObject = new List<TopicObject>();
            List<TopicObject> listTopicObjectForEachGroup = new List<TopicObject>();
            int i, j, k, l;
            Random random = new Random();
            int indexRandomUser;
            UserObject userObjectRandom;
            UserObject userObject;

            // them user
            if (region == regions[0]) // Asia
            {
                for (i = 1; i <= numberOfUsers/3; i++)
                {
                    userObject = new UserObject();
                    userObject.Id = region + "-" + Guid.NewGuid().ToString();
                    userObject.FullName = dt.Rows[i - 1]["FullName"].ToString();
                    userObject.UserName = "username" + i;
                    userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                    userObject.Email = "username" + i + "@gmail.com";
                    userObject.Region = region;
                    listUserObject.Add(userObject);
                }
            }
            else
            {
                if (region == regions[1]) // Middle East
                {
                    for (i = numberOfUsers / 3 + 1; i <= numberOfUsers/3 *2; i++)
                    {
                        userObject = new UserObject();
                        userObject.Id = region + "-" + Guid.NewGuid().ToString();
                        userObject.FullName = dt.Rows[i - 1]["FullName"].ToString();
                        userObject.UserName = "username" + i;
                        userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                        userObject.Email = "username" + i + "@gmail.com";
                        userObject.Region = region;
                        listUserObject.Add(userObject);
                    }
                }
                else // America
                {
                    for (i = numberOfUsers / 3 * 2 + 1; i <= numberOfUsers; i++)
                    {
                        userObject = new UserObject();
                        userObject.Id = region + "-" + Guid.NewGuid().ToString();
                        userObject.FullName = dt.Rows[i-1]["FullName"].ToString();
                        userObject.UserName = "username" + i;
                        userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                        userObject.Email = "username" + i + "@gmail.com";
                        userObject.Region = region;
                        listUserObject.Add(userObject);
                    }
                }
            }

            GroupObject groupObject;
            for (i = 1; i <= numberOfGroups; i++)
            {
                // tao group
                groupObject = new GroupObject();
                groupObject.Id = region + "-" + Guid.NewGuid().ToString();
                groupObject.GroupName = "Group " + i;
                groupObject.Description = "This is new group";
                groupObject.IsPublic = false;
                groupObject.CreateDate = DateTime.Now;

                // tao ngau nhien user trong 5.000 Users o Asia
                indexRandomUser = random.Next(listUserObject.Count());
                userObjectRandom = listUserObject[indexRandomUser];

                groupObject.CreateBy = userObjectRandom;
                groupObject.NewEvent = new GroupEvent();
                groupObject.NewEvent.Title = "New post";
                groupObject.NewEvent.CreateDate = DateTime.Now;
                groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                groupObject.NewEvent.UserId = userObjectRandom.Id;

                // tao user group cho group vua tao
                var userGroupObject = new UserGroupObject();
                userGroupObject.Id = region + "-" + Guid.NewGuid().ToString();
                userGroupObject.UserId = userObjectRandom.Id;
                userGroupObject.GroupId = groupObject.Id;
                userGroupObject.GroupName = groupObject.GroupName;
                userGroupObject.Description = groupObject.Description;
                userGroupObject.IsApprove = UserGroupStatus.Approve;
                userGroupObject.IsReceiveEmail = true;
                userGroupObject.JoinDate = DateTime.Now;
                userGroupObject.GroupRole = groupRoleOwner;
                userGroupObject.IsReceiveEmail = true;

                // them usergroup vao listUserGroupObject
                listUserGroupObject.Add(userGroupObject);

                // them usergroup vap group
                groupObject.ListUserGroup.Add(userGroupObject);
                // them user group vao user
                listUserObject.Find(u => u.UserName == userObjectRandom.UserName).ListUserGroup.Add(userGroupObject);

                // lay 50 member dau tien
                list50MemberFirst = listUserObject.GetRange(new Random().Next(0,numberOfUsers/3 -51), 50);
                if (list50MemberFirst.Exists(u => u.UserName == userObjectRandom.UserName))
                {
                    list50MemberFirst.Remove(userObjectRandom);
                }

                UserGroupObject userGroupObjectMember;
                foreach (var user in list50MemberFirst) // duyet qua danh sach 50 member de luu xuong db
                {
                    userGroupObjectMember = new UserGroupObject();
                    userGroupObjectMember.Id = region + "-" + Guid.NewGuid().ToString();
                    userGroupObjectMember.UserId = user.Id;
                    userGroupObjectMember.GroupId = groupObject.Id;
                    userGroupObjectMember.GroupName = groupObject.GroupName;
                    userGroupObjectMember.Description = groupObject.Description;
                    userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                    userGroupObjectMember.JoinDate = DateTime.Now;
                    userGroupObjectMember.GroupRole = groupRoleMember;
                    userGroupObjectMember.IsReceiveEmail = true;

                    // them userGroupObjectMember vao listUserGroupObject
                    listUserGroupObject.Add(userGroupObjectMember);
                    // them member vao group
                    groupObject.ListUserGroup.Add(userGroupObjectMember);
                    // cap nhat danh sach listUserGroup
                    listUserObject.Find(u => u.UserName == user.UserName).ListUserGroup.Add(userGroupObjectMember);
                }

                // tao 100 topics
                TopicObject topicObject;
                int indexRandomIn50Members;
                UserObject userObjectRandomIn50Members;
                listTopicObjectForEachGroup = new List<TopicObject>();
                for (j = 1; j <= numberOfTopicsForEachGroup; j++)
                {
                    topicObject = new TopicObject();
                    topicObject.Id = region + "-" + Guid.NewGuid().ToString();
                    topicObject.TopicName = "Topic " + j + " group " + groupObject.GroupName;
                    topicObject.Content = "Content topic here.";

                    // tao ngau nhien user
                    indexRandomIn50Members = random.Next(list50MemberFirst.Count());
                    userObjectRandomIn50Members = list50MemberFirst[indexRandomIn50Members];
                    topicObject.CreateBy = userObjectRandomIn50Members;
                    topicObject.GroupId = groupObject.Id;
                    topicObject.CreateDate = DateTime.Now;
                    topicObject.LastModified = topicObject.CreateDate;
                    topicObject.NumberOfView = 0;
                    topicObject.isDeleted = false;

                    // tao 100 comments
                    CommentObject commentObject;
                    List<CommentObject> listCommentObject = new List<CommentObject>();
                    for (k = 1; k <= numberOfCommentsForEachTopic; k++)
                    {
                        commentObject = new CommentObject();
                        commentObject.Id = Guid.NewGuid().ToString();
                        commentObject.Content = "This is comment " + k;
                        commentObject.ParentContent = "This is parent content.";
                        // tao user ngau nhien trong 50 thanh vien cua nhom
                        indexRandomIn50Members = random.Next(list50MemberFirst.Count());
                        userObjectRandomIn50Members = list50MemberFirst[indexRandomIn50Members];
                        commentObject.CreateBy = userObjectRandomIn50Members;
                        commentObject.CreateDate = DateTime.Now;
                        commentObject.isDeleted = false;
                        listCommentObject.Add(commentObject);
                    }
                    topicObject.ListComment = listCommentObject;
                    topicObject.NumberOfComment = Convert.ToUInt32(listCommentObject.Count);

                    listTopicObjectForEachGroup.Add(topicObject);
                    listTopicObject.Add(topicObject);
                }

                // them denormalize topic vao group
                foreach (TopicObject topic in listTopicObjectForEachGroup)
                {
                    groupObject.ListTopic.Add(topic);
                }

                listGroupObject.Add(groupObject);

            }

            #region RavenDB 2.0 using Bulk Insert
            try
            {
                using (var bulkInsert = documentStore.BulkInsert())
                {
                    bulkInsert.OnBeforeEntityInsert += bulkInsert_OnBeforeEntityInsert;
                    foreach (UserObject u in listUserObject)
                    {
                        bulkInsert.Store(u);
                    }
                    foreach (GroupObject g in listGroupObject)
                    {
                        bulkInsert.Store(g);
                    }
                    foreach (UserGroupObject u in listUserGroupObject)
                    {
                        bulkInsert.Store(u);
                    }
                    foreach (TopicObject t in listTopicObject)
                    {
                        bulkInsert.Store(t);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Insert Error!");
            }
            #endregion

        }

        static void bulkInsert_OnBeforeEntityInsert(string id, Raven.Json.Linq.RavenJObject data, Raven.Json.Linq.RavenJObject metadata)
        {
            metadata["Raven-Shard-Id"] = "Asia";
            //throw new NotImplementedException();
        }

        // tạo ngẫu nhiên tên user
        public static DataTable GetListFullName(int length)
        {
            string[] listHo1 = {"Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý",
                               "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh","Phan","Vũ","Võ","Đặng","Bùi","Đỗ","Hồ","Ngô","Dương","Lý"};

            string[] listHo2 = {"Âu","Dương","Bá","Bạch","Bảo","Bửu","Ca","Cao",
                                   "Châu","Chung","Chương","Chử","Cổ","Cù","Cung",
                                   "Cự","Dã","Danh","Doãn","Dư","Đàm","Đan","Đào",
                                   "Đinh","Đoàn","Hà","Hạ","Hứa","Kha","Kiều","Kim",
                                   "La","Lạc","Lại","Lâm","Liễu","Lục","Lương","Lưu",
                                   "Mã","Mai","Mộc","Ninh","Nhâm","Nghiêm","Nhữ","Ông",
                                   "Phi","Phùng","Quách","Tạ","Tào","Thạch","Thái","Tiêu",
                                   "Tô","Tôn","Tông","Tống","Trà","Trác","Triệu","Trịnh",
                                   "Trình","Trưng","Trương","Từ","Ung","Vi","Viên","Vương"};
            string[] listTenDem1 = { "Văn", "Thị" };
            string[] listTenDem2 = { "Xuân", "Thu", "Cẩm", "Châu", "Hồng", "Hoàng", "Đức", "Hạnh", "Đình", "Đại", "Ngọc" };
            string[] listTenNu = { "Mai", "Lan", "Cúc", "Hoa", "Hương", "Yến", "Anh", "Oanh", "Bích", "Ngọc", "Trân", "Nhung", "Gấm", "Là", "Lụa", "Hạnh", "Thảo", "Hiền", "Dung", "Vân", "Thúy", "Diễm", "Lệ", "Nguyệt", "Trang", "Huyền" };
            string[] listTenNam = { "Cương", "Cường", "Hùng", "Tráng", "Dũng", "Thông", "Minh", "Trí", "Tuệ", "Sáng", "Hoài", "Nhân", "Trung", "Tín", "Lễ", "Nghĩa", "Công", "Hiệp", "Phú", "Quý", "Kim", "Tài", "Danh", "Sơn", "Giang", "Lâm", "Hải", "Dương" };

            Random random = new Random();
            int xacsuat;
            int index = 0;

            DataTable dtb = new DataTable();
            //Tạo các Columns
            dtb.Columns.Add("FullName");

            //Thêm các bản ghi

            int numHo1 = listHo1.Length;
            int numHo2 = listHo2.Length;
            int numTenDem2 = listTenDem2.Length;
            int numTenNam = listTenNam.Length;
            int numTenNu = listTenNu.Length;
            string ho, tendem, tenchinh, tendaydu;
            bool isMan = true;
            for (int i = 0; i < length; i++)
            {
                //lay ho
                xacsuat = random.Next(10); // xac suat ho 
                if (xacsuat == 1) // truong hop bang 1 chiem 10%, con lai la 90%
                {
                    index = random.Next(numHo2);
                    ho = listHo2[index];
                }
                else
                {
                    index = random.Next(numHo1);
                    ho = listHo1[index];
                }

                // lay ten dem 1
                xacsuat = random.Next(2); // xac suat ten dem 50-50, nam-nu
                if (xacsuat == 1) // nam
                {
                    tendem = "Văn";
                    isMan = true;
                }
                else
                { // nu
                    tendem = "Thị";
                    isMan = false;
                }

                // lay ten dem 2
                xacsuat = random.Next(2); // xac suat ten dem thu 2 50-50, co hay ko co
                if (xacsuat == 1) // co ten dem thu 2
                {
                    index = random.Next(numTenDem2);
                    tendem += " " + listTenDem2[index];
                }

                // lay ten chinh
                if (isMan)
                {
                    index = random.Next(numTenNam);
                    tenchinh = listTenNam[index];
                }
                else
                {
                    index = random.Next(numTenNu);
                    tenchinh = listTenNu[index];
                }

                tendaydu = ho + " " + tendem + " " + tenchinh;

                // dua du lieu vao table
                dtb.Rows.Add(tendaydu);
            }

            return dtb;
        }

    }

    public class Group
    {
        public string GroupName { get; set; }
    }
}
