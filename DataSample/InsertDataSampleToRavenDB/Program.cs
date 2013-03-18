using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Shard;
using Raven.Json.Linq;
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

namespace InsertDataSampleToRavenDB
{
    class Program
    {
        public static string STR_DATA_SERVER = "F:\\RavenServers-2230\\Data\\";
        public static string STR_DATA_SERVER_USERS = "F:\\RavenServers-2230\\Data\\Users\\";
        public static string STR_DATA_SERVER_GROUPS = "F:\\RavenServers-2230\\Data\\Groups\\";
        public static string STR_DATA_SERVER_GROUPRSS = "F:\\RavenServers-2230\\Data\\GroupRSS\\";
        public static string STR_DATA_SERVER_TOPICS = "F:\\RavenServers-2230\\Data\\Topics\\";
        
        public static string  databaseName;
        public static IDocumentStore documentStoreShard;
        public static IDocumentStore[] documentStores;
        public static List<string> ServerRegion;
        public static string ServerGeneral;

        
        public static string ConnectRavenDB = "http://localhost:8081,Asia;http://localhost:8082,MiddleEast;http://localhost:8083,America";


        public static Dictionary<string, IDocumentStore> shards = new Dictionary<string, IDocumentStore>();

        public static string shardName = string.Empty;
        public static string shardUrl = string.Empty;
        public static int i = 0;

        public static GroupRoleObject groupRoleManager, groupRoleOwner, groupRoleMember;

        static void Main(string[] args)
        {
            Console.WriteLine("Inserting...!");
            Stopwatch st = new Stopwatch();
            st.Start();
            
            Init();
            CreateGroupRoles();
            InsertDataSampleToServers();
            // ~ insert 1.000.000 records


            //InsertLargeDataNewestTest(); // ham nay hien tai la chinh thuc
            //InsertLargeDataUsingPatchingAPI();
            //InitIndexes();

            st.Stop();
            Console.WriteLine("Insert Data Sample Success!");
            Console.WriteLine("Time elapsed: {0}", st.Elapsed);
        }

        public static void Init()
        {
            databaseName = "UITNoSQLDB2";
            string[] connects = ConnectRavenDB.Split(';');
            documentStores = new DocumentStore[connects.Length];
            ServerRegion = new List<string>();

            foreach (var connect in connects)
            {
                var co = connect.Split(',');
                shardUrl = co[0];
                shardName = co[1];
                shards.Add(shardName, new DocumentStore { Url = shardUrl, DefaultDatabase = databaseName });

                documentStores[i] = new DocumentStore { Url = shardUrl, DefaultDatabase = databaseName };//.Initialize();
                documentStores[i].Conventions.MaxNumberOfRequestsPerSession = 100000;
                documentStores[i].Initialize();
                ServerRegion.Add(shardName);
                i++;
            }

            DocumentConvention Conventions;
            Conventions = shards.First().Value.Conventions.Clone();
            var shardStrategy = new ShardStrategy(shards)
                .ShardingOn<UserObject>(u => u.Region)
                .ShardingOn<GroupObject>(g => g.CreateBy.Id)
                .ShardingOn<TopicObject>(t => t.CreateBy.Id)
                .ShardingOn<UserGroupObject>(u => u.UserId)
                .ShardingOn<GroupRoleObject>(r => r.IsGeneral);
            shardStrategy.Conventions.IdentityPartsSeparator = "-";
            shardStrategy.ModifyDocumentId = (convention, shardId, documentId) => shardId + convention.IdentityPartsSeparator + documentId;

            documentStoreShard = new ShardedDocumentStore(shardStrategy).Initialize();
            documentStoreShard.Conventions.MaxNumberOfRequestsPerSession = 100000;

            //set server general
            ServerGeneral = shardName;
        }

        public static void InitIndexes()
        {
            foreach (var doc in documentStores)
            {
                IndexCreation.CreateIndexes(typeof(LoginIndex).Assembly, doc);
                IndexCreation.CreateIndexes(typeof(GroupIndex).Assembly, doc);
                IndexCreation.CreateIndexes(typeof(GroupRoleIndex).Assembly, doc);
                IndexCreation.CreateIndexes(typeof(ByGroupIdAndJoinDateSortByJoinDate).Assembly, doc);
                IndexCreation.CreateIndexes(typeof(GetUserGroupByUserIdGroupId).Assembly, doc);
                IndexCreation.CreateIndexes(typeof(GroupObject_ByIsPublic).Assembly, doc);
                IndexCreation.CreateIndexes(typeof(GroupObject_Search).Assembly, doc);
                IndexCreation.CreateIndexes(typeof(GroupObject_Search_NotAnalyed).Assembly, doc);
            }     
        }

        public static void CreateGroupRoles()
        {
            using (var session = documentStoreShard.OpenSession())
            {
                groupRoleManager = new GroupRoleObject();
                groupRoleManager.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
                groupRoleManager.GroupName = "Manager";
                groupRoleManager.IsGeneral = ServerRegion[0];
                session.Store(groupRoleManager);

                groupRoleMember = new GroupRoleObject();
                groupRoleMember.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
                groupRoleMember.GroupName = "Member";
                groupRoleMember.IsGeneral = ServerRegion[0];
                session.Store(groupRoleMember);

                groupRoleOwner = new GroupRoleObject();
                groupRoleOwner.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
                groupRoleOwner.GroupName = "Owner";
                groupRoleOwner.IsGeneral = ServerRegion[0];
                session.Store(groupRoleOwner);
                session.SaveChanges();
                session.Dispose();
            }
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
                Indexes.Add(x=> x.Description, FieldIndexing.Analyzed);
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
        //public class GroupObject_Search_NotAnalyed : AbstractIndexCreationTask<GroupObject>
        //{
        //    public GroupObject_Search_NotAnalyed()
        //    {
        //        Map = groups => from g in groups
        //                        select new
        //                        {
        //                            g.GroupName
        //                        };
        //        Indexes.Add(x => x.GroupName, FieldIndexing.NotAnalyzed);                
        //    }
        //}

        /// <summary>
        ///  ham insert du lieu chinh
        /// </summary>
        public static void InsertDataSampleToServers()
        {    

            // co bao hieu luu user vao db
            bool flagSaveUser = true;

            var session = documentStoreShard.OpenSession();
            GroupRoleObject groupRoleManager, groupRoleOwner, groupRoleMember;
            groupRoleManager = new GroupRoleObject();
            groupRoleManager.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRoleManager.GroupName = "Manager";
            groupRoleManager.IsGeneral = ServerGeneral;
            session.Store(groupRoleManager);

            groupRoleMember = new GroupRoleObject();
            groupRoleMember.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRoleMember.GroupName = "Member";
            groupRoleMember.IsGeneral = ServerGeneral;
            session.Store(groupRoleMember);

            groupRoleOwner = new GroupRoleObject();
            groupRoleOwner.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRoleOwner.GroupName = "Owner";
            groupRoleOwner.IsGeneral = ServerGeneral;
            session.Store(groupRoleOwner);

            session.SaveChanges();
            session.Dispose();

            // 2 danh sách lưu dữ liệu cục bộ để sử dụng cho lưu group và lưu Topic, Comment
            List<UserObject> ListUserObject = new List<UserObject>();
            List<GroupObject> ListGroupObject = new List<GroupObject>();
            List<UserGroupObject> ListUserGroupObject = new List<UserGroupObject>();
            List<UserGroupObject> ListUserGroupObjectMember = new List<UserGroupObject>();
            List<TopicObject> ListTopicObject = new List<TopicObject>();

            UserObject userObject = null;
            GroupObject groupObject = null;

            // đọc dữ liệu 15.000 Users từ file ListUsers.xml
            string usersInfroXmlFilePath = STR_DATA_SERVER_USERS + "ListUsers.xml";
            XElement xmlUser = XElement.Load(usersInfroXmlFilePath);
            ListUserObject = (from u in xmlUser.Elements("UserObject")
                              select new UserObject
                              {
                                  Id = u.Element("Id").Value,
                                  FullName = u.Element("FullName").Value,
                                  UserName = u.Element("UserName").Value,
                                  Email = u.Element("Email").Value,
                                  Password = u.Element("Password").Value,
                                  Region = u.Element("Region").Value,
                                  CreateDate = DateTime.Now,
                              }).ToList();

            // đọc dữ liệu 15.000 Users từ file ListUsersReference.xml
            string path = STR_DATA_SERVER_USERS + "ListUsersReference.xml";
            XElement xmlUserReference = XElement.Load(path);
            // danh sach nay de lay user khi tao group, usergroup, topic, comment ma khong can gui yeu cau toi server
            List<UserObject> listUserReference = (from u in xmlUserReference.Elements("UserObject")
                                                  select new UserObject
                                                  {
                                                      Id = u.Element("Id").Value,
                                                      FullName = u.Element("FullName").Value,
                                                      UserName = u.Element("UserName").Value,
                                                      Email = u.Element("Email").Value,
                                                      Password = u.Element("Password").Value,
                                                      Region = u.Element("Region").Value,
                                                  }).ToList();

            // doc du lieu danh sách Groups tu ListGroups.xml
            string groupsInfoXmlFilePath = STR_DATA_SERVER_GROUPS + "ListGroups.xml";
            if (File.Exists(groupsInfoXmlFilePath))
            {
                XElement xmlGroupRSS = XElement.Load(groupsInfoXmlFilePath);
                List<GroupRSS> listGroup = (from u in xmlGroupRSS.Elements("Group")
                                            select new GroupRSS
                                            {
                                                GroupName = u.Element("GroupName").Value,
                                            }).ToList();

                Random random = new Random();
                int indexRandom;
                int lengthListUserReferecen = listUserReference.Count;
                UserObject userObjectRandom;
                UserObject userObjectReal;
                string region;
                foreach (var group in listGroup)
                {
                    // tao group
                    groupObject = new GroupObject();
                    groupObject.Id = Guid.NewGuid().ToString();
                    groupObject.GroupName = group.GroupName;
                    groupObject.Description = "This is new group";
                    groupObject.IsPublic = false;
                    groupObject.CreateDate = DateTime.Now;

                    // tao ngau nhien user trong 15.000 Users
                    indexRandom = random.Next(lengthListUserReferecen);
                    userObjectRandom = listUserReference[indexRandom];
                    region = userObjectRandom.Region;

                    groupObject.CreateBy = userObjectRandom;
                    groupObject.NewEvent = new GroupEvent();
                    groupObject.NewEvent.Title = "New group";
                    groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                    groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                    groupObject.NewEvent.UserId = userObjectRandom.Id;

                    // tao user group cho group vua tao
                    var userGroupObject = new UserGroupObject();
                    userGroupObject.Id = Guid.NewGuid().ToString();
                    userGroupObject.UserId = userObjectRandom.Id;
                    userGroupObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                    userGroupObject.GroupName = groupObject.GroupName;
                    userGroupObject.Description = groupObject.Description;
                    userGroupObject.IsApprove = UserGroupStatus.Approve;
                    userGroupObject.IsReceiveEmail = true;
                    userGroupObject.JoinDate = DateTime.Now;
                    userGroupObject.GroupRole = groupRoleOwner;
                    userGroupObject.IsReceiveEmail = true;

                    var userGroupObjectTemp = new UserGroupObject();
                    userGroupObjectTemp.Id = region + "-" + userGroupObject.Id;
                    userGroupObjectTemp.UserId = userGroupObject.UserId;
                    userGroupObjectTemp.GroupId = userGroupObject.GroupId;
                    userGroupObjectTemp.GroupName = userGroupObject.GroupName;
                    userGroupObjectTemp.Description = userGroupObject.Description;
                    userGroupObjectTemp.IsApprove = userGroupObject.IsApprove;
                    userGroupObjectTemp.IsReceiveEmail = true;
                    userGroupObjectTemp.JoinDate = userGroupObject.JoinDate;
                    userGroupObjectTemp.GroupRole = userGroupObject.GroupRole;
                    userGroupObjectTemp.IsReceiveEmail = true;

                    // them usergroup vap group
                    groupObject.ListUserGroup.Add(userGroupObjectTemp);
            
                    // them user group vao user
                    ListUserObject.Find(u => u.UserName == userObjectRandom.UserName).ListUserGroup.Add(userGroupObjectTemp);

                    

                    // tao 50 member cho moi group
                    int indexRandomMember;
                    UserObject userObjectMember;
                    List<UserObject> listUserObjectMember = new List<UserObject>();
                    int numberOfMember = 0;
                    while (numberOfMember < 50)
                    {
                        indexRandomMember = random.Next(lengthListUserReferecen);
                        if (indexRandomMember != indexRandom)
                        {
                            userObjectMember = listUserReference[indexRandomMember];
                            listUserObjectMember.Add(userObjectMember);
                            numberOfMember += 1;
                        }
                    }
                    int lengthList50Members = listUserObjectMember.Count;

                    UserGroupObject userGroupObjectMember;
                    UserGroupObject ugTemp;
                    foreach (var user in listUserObjectMember) // duyet qua danh sach 50 member de luu xuong db
                    {
                        userGroupObjectMember = new UserGroupObject();
                        userGroupObjectMember.Id = Guid.NewGuid().ToString();
                        userGroupObjectMember.UserId = user.Id;
                        userGroupObjectMember.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                        userGroupObjectMember.GroupName = groupObject.GroupName;
                        userGroupObjectMember.Description = groupObject.Description;
                        userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                        userGroupObjectMember.JoinDate = DateTime.Now;
                        userGroupObjectMember.GroupRole = groupRoleMember;
                        userGroupObjectMember.IsReceiveEmail = true;

                        ugTemp = new UserGroupObject();
                        ugTemp.Id = user.Region + "-" + userGroupObjectMember.Id;
                        ugTemp.UserId = userGroupObjectMember.UserId;
                        ugTemp.GroupId = userGroupObjectMember.GroupId;
                        ugTemp.GroupName = userGroupObjectMember.GroupName;
                        ugTemp.Description = userGroupObjectMember.Description;
                        ugTemp.IsApprove = userGroupObjectMember.IsApprove;
                        ugTemp.JoinDate = userGroupObjectMember.JoinDate;
                        ugTemp.GroupRole = userGroupObjectMember.GroupRole;
                        ugTemp.IsReceiveEmail = true;

                        // them member vao group
                        groupObject.ListUserGroup.Add(ugTemp);

                        // lay user ListUsersReference len de cap nhat danh sach ListUserGroup, ko lay tu db
                        ListUserObject.Find(u => u.UserName == user.UserName).ListUserGroup.Add(ugTemp);                      

                        // luu user group member xuong db
                        //session.Store(userGroupObjectMember);
                        ListUserGroupObjectMember.Add(userGroupObjectMember);
                    }

                    // doc danh sach topic tu xml de luu vao group
                    string strSubFolderTopic;
                    List<TopicObject> listTopicObject = new List<TopicObject>();
                    foreach (string path1 in Directory.GetDirectories(STR_DATA_SERVER_TOPICS)) // path1 = "91538b3e-3f95-4525-9c81-512ea6abe16c"
                    {
                        //string path2 = Directory.GetDirectories(path1).FirstOrDefault(x => x == groupObject.GroupName); // path2 = "24h - Ẩm thực"
                        DirectoryInfo directory = new DirectoryInfo(path1);
                        DirectoryInfo subDirectory = directory.GetDirectories().FirstOrDefault(x => x.Name == groupObject.GroupName);
                        if (subDirectory != null)
                        {
                            FileInfo file = subDirectory.GetFiles().FirstOrDefault();
                            if (file.Exists)
                            {
                                // doc topic len
                                string topicsInfoXmlFilePath = file.FullName; //path2 + "/ListTopics.xml";
                                XElement xmlTopics = XElement.Load(topicsInfoXmlFilePath);
                                List<Topic> listTopics = (from u in xmlTopics.Elements("TopicObject")
                                                          let y = u.Descendants("Comment").ToList()
                                                          select new Topic
                                                          {
                                                              TopicName = u.Element("TopicName").Value,
                                                              Content = u.Element("Content").Value,
                                                              ListComment = ConvertXElementToComment(y)
                                                          }).ToList();
                                TopicObject topicObject;
                                int indexRandomIn50Members;
                                UserObject userObjectRandomIn50Members;
                                foreach (Topic topic in listTopics)
                                {
                                    topicObject = new TopicObject();
                                    topicObject.Id = Guid.NewGuid().ToString();
                                    topicObject.TopicName = topic.TopicName;
                                    topicObject.Content = topic.Content;

                                    // tao ngau nhien user
                                    indexRandomIn50Members = random.Next(lengthList50Members);
                                    userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                                    topicObject.CreateBy = userObjectRandomIn50Members;
                                    topicObject.GroupId =  userObjectRandom.Region + "-" + groupObject.Id;
                                    topicObject.CreateDate = DateTime.Now;
                                    topicObject.LastModified = topicObject.CreateDate;
                                    topicObject.NumberOfView = 0;
                                    topicObject.isDeleted = false;

                                    CommentObject commentObject;
                                    List<CommentObject> listCommentObject = new List<CommentObject>();
                                    foreach (var comment in topic.ListComment)
                                    {
                                        commentObject = new CommentObject();
                                        commentObject.Id = Guid.NewGuid().ToString();
                                        commentObject.Content = comment.Content;
                                        commentObject.ParentContent = topic.Content;
                                        // tao user ngau nhien trong 50 thanh vien cua nhom
                                        indexRandomIn50Members = random.Next(lengthList50Members);
                                        userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                                        commentObject.CreateBy = userObjectRandomIn50Members;
                                        commentObject.CreateDate = DateTime.Now;
                                        commentObject.isDeleted = false;
                                        listCommentObject.Add(commentObject);
                                    }

                                    topicObject.ListComment = listCommentObject;
                                    topicObject.NumberOfComment = Convert.ToUInt32(listCommentObject.Count);

                                    listTopicObject.Add(topicObject);

                                    // luu tat ca topic vao day
                                    ListTopicObject.Add(topicObject);
                                }
                            }
                        }
                        
                    }
                    TopicObject tempTopic;
                    // them denormalize topic vao group
                    foreach (TopicObject topic in listTopicObject)
                    {
                        var reg = listUserObjectMember.Find(x => x.Id == topic.CreateBy.Id).Region;
                        tempTopic = new TopicObject();
                        tempTopic.Id = reg + "-" + topic.Id;
                        tempTopic.TopicName = topic.TopicName;
                        tempTopic.CreateBy = topic.CreateBy;
                        tempTopic.LastModified = topic.LastModified;
                        tempTopic.NumberOfComment = topic.NumberOfComment;
                        tempTopic.NumberOfView = topic.NumberOfView;
                        tempTopic.isDeleted = topic.isDeleted;
                        
                        groupObject.ListTopic.Add(tempTopic);
                    }

                    // luu vao session userGroupObject
                    //session.Store(userGroupObject);
                    // group xong het roi, luu xuong thoi
                    //session.Store(groupObject);

                    ListUserGroupObject.Add(userGroupObject);
                    ListGroupObject.Add(groupObject);

                }
            }

            #region RavenDB 2.0
            /*
            using (var bulkInsert = documentStoreShard.BulkInsert())
            {
                foreach (UserObject u in ListUserObject)
                {
                    bulkInsert.Store(u);
                }
                //foreach (GroupObject g in ListGroupObject)
                //{
                //    bulkInsert.Store(g);
                //}
                //foreach (UserGroupObject u in ListUserGroupObject)
                //{
                //    bulkInsert.Store(u);
                //}
                //foreach (UserGroupObject u in ListUserGroupObjectMember)
                //{
                //    bulkInsert.Store(u);
                //}
                //foreach (TopicObject t in ListTopicObject)
                //{
                //    bulkInsert.Store(t);
                //}
            }
            */
            #endregion

            #region RavenDB 1.0
            
            using (var session1 = documentStoreShard.OpenSession())
            {
                foreach (UserObject u in ListUserObject)
                {
                    session1.Store(u);
                }
                session1.SaveChanges();
                session1.Dispose();
            }
            
            using (var session1 = documentStoreShard.OpenSession())
            {
                foreach (GroupObject g in ListGroupObject)
                {
                    session1.Store(g);
                }
                session1.SaveChanges();
                session1.Dispose();
            }
            using (var session1 = documentStoreShard.OpenSession())
            {
                foreach (UserGroupObject u in ListUserGroupObject)
                {
                    session1.Store(u);
                }
                session1.SaveChanges();
                session1.Dispose();
            }
            using (var session1 = documentStoreShard.OpenSession())
            {
                foreach (UserGroupObject u in ListUserGroupObjectMember)
                {
                    session1.Store(u);
                }
                session1.SaveChanges();
                session1.Dispose();
            }
            using (var session1 = documentStoreShard.OpenSession())
            {
                foreach (TopicObject t in ListTopicObject)
                {
                    session1.Store(t);
                }
                session1.SaveChanges();
                session1.Dispose();
            }
             
            #endregion
        }

        /// <summary>
        ///  copy ham insert du lieu chinh
        /// </summary>
        public static void InsertDataSampleToServersCopy()
        {
            var session = documentStoreShard.OpenSession();
            GroupRoleObject groupRoleManager, groupRoleOwner, groupRoleMember;
            groupRoleManager = new GroupRoleObject();
            groupRoleManager.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRoleManager.GroupName = "Manager";
            groupRoleManager.IsGeneral = ServerGeneral;
            session.Store(groupRoleManager);

            groupRoleMember = new GroupRoleObject();
            groupRoleMember.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRoleMember.GroupName = "Member";
            groupRoleMember.IsGeneral = ServerGeneral;
            session.Store(groupRoleMember);

            groupRoleOwner = new GroupRoleObject();
            groupRoleOwner.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRoleOwner.GroupName = "Owner";
            groupRoleOwner.IsGeneral = ServerGeneral;
            session.Store(groupRoleOwner);

            session.SaveChanges();
            session.Dispose();

            // 2 danh sách lưu dữ liệu cục bộ để sử dụng cho lưu group và lưu Topic, Comment
            List<UserObject> ListUserObject = new List<UserObject>();
            List<GroupObject> ListGroupObject = new List<GroupObject>();
            List<UserGroupObject> ListUserGroupObject = new List<UserGroupObject>();
            List<UserGroupObject> ListUserGroupObjectMember = new List<UserGroupObject>();
            List<TopicObject> ListTopicObject = new List<TopicObject>();

            UserObject userObject = null;
            GroupObject groupObject = null;

            DataTable dt = GetListFullName(500);

            List<UserObject> ListUserObjectReference = new List<UserObject>();
            int i, j, k;
            for (i = 0; i < 500; i++)
            {
                userObject = new UserObject();
                userObject.Id = Guid.NewGuid().ToString();
                userObject.FullName = dt.Rows[i]["FullName"].ToString();
                userObject.UserName = "username" + (i + 1).ToString();
                userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                userObject.Email = "username" + (i + 1).ToString() + "@gmail.com";
                userObject.Region = ServerRegion[0];
                ListUserObject.Add(userObject);
            }

            foreach (var user in ListUserObject)
            {
                userObject = new UserObject();
                userObject.Id = user.Region + "-" + user.Id;
                userObject.FullName = user.FullName;
                userObject.UserName = user.UserName;
                userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                userObject.Email = user.Email;
                userObject.Region = ServerRegion[0];
                ListUserObjectReference.Add(userObject);
            }

            Random random = new Random();
            int indexRandom;
            int lengthListUserReferecen = ListUserObjectReference.Count;
            UserObject userObjectRandom;
            UserObject userObjectReal;
            string region;
            for (i = 1; i <= 100;i++ )
            {
                // tao group
                groupObject = new GroupObject();
                groupObject.Id = Guid.NewGuid().ToString();
                groupObject.GroupName = "Group " + i; 
                groupObject.Description = "This is new group";
                groupObject.IsPublic = false;
                groupObject.CreateDate = DateTime.Now;

                // tao ngau nhien user trong 5.000 Users
                indexRandom = random.Next(lengthListUserReferecen);
                userObjectRandom = ListUserObjectReference[indexRandom];
                region = userObjectRandom.Region;

                groupObject.CreateBy = userObjectRandom;
                groupObject.NewEvent = new GroupEvent();
                groupObject.NewEvent.Title = "New post";
                groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                groupObject.NewEvent.UserId = userObjectRandom.Id;

                // tao user group cho group vua tao
                var userGroupObject = new UserGroupObject();
                userGroupObject.Id = Guid.NewGuid().ToString();
                userGroupObject.UserId = userObjectRandom.Id;
                userGroupObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                userGroupObject.GroupName = groupObject.GroupName;
                userGroupObject.Description = groupObject.Description;
                userGroupObject.IsApprove = UserGroupStatus.Approve;
                userGroupObject.IsReceiveEmail = true;
                userGroupObject.JoinDate = DateTime.Now;
                userGroupObject.GroupRole = groupRoleOwner;
                userGroupObject.IsReceiveEmail = true;

                var userGroupObjectTemp = new UserGroupObject();
                userGroupObjectTemp.Id = region + "-" + userGroupObject.Id;
                userGroupObjectTemp.UserId = userGroupObject.UserId;
                userGroupObjectTemp.GroupId = userGroupObject.GroupId;
                userGroupObjectTemp.GroupName = userGroupObject.GroupName;
                userGroupObjectTemp.Description = userGroupObject.Description;
                userGroupObjectTemp.IsApprove = userGroupObject.IsApprove;
                userGroupObjectTemp.IsReceiveEmail = true;
                userGroupObjectTemp.JoinDate = userGroupObject.JoinDate;
                userGroupObjectTemp.GroupRole = userGroupObject.GroupRole;
                userGroupObjectTemp.IsReceiveEmail = true;

                // them usergroup vap group
                groupObject.ListUserGroup.Add(userGroupObjectTemp);

                // them user group vao user
                ListUserObject.Find(u => u.UserName == userObjectRandom.UserName).ListUserGroup.Add(userGroupObjectTemp);

                // tao 50 member cho moi group
                int indexRandomMember;
                UserObject userObjectMember;
                List<UserObject> listUserObjectMember = new List<UserObject>();
                indexRandomMember = random.Next(0, 49);
                listUserObjectMember = ListUserObjectReference.GetRange(indexRandomMember, 50);
                userObjectMember = listUserObjectMember.Find(u => u.Id.Equals(userObjectRandom.Id));
                if (userObjectMember != null)
                {
                    listUserObjectMember.Remove(userObjectMember);
                }
                int lengthList50Members = listUserObjectMember.Count;

                UserGroupObject userGroupObjectMember;
                UserGroupObject ugTemp;
                foreach (var user in listUserObjectMember) // duyet qua danh sach 50 member de luu xuong db
                {
                    userGroupObjectMember = new UserGroupObject();
                    userGroupObjectMember.Id = Guid.NewGuid().ToString();
                    userGroupObjectMember.UserId = user.Id;
                    userGroupObjectMember.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                    userGroupObjectMember.GroupName = groupObject.GroupName;
                    userGroupObjectMember.Description = groupObject.Description;
                    userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                    userGroupObjectMember.JoinDate = DateTime.Now;
                    userGroupObjectMember.GroupRole = groupRoleMember;
                    userGroupObjectMember.IsReceiveEmail = true;

                    ugTemp = new UserGroupObject();
                    ugTemp.Id = user.Region + "-" + userGroupObjectMember.Id;
                    ugTemp.UserId = userGroupObjectMember.UserId;
                    ugTemp.GroupId = userGroupObjectMember.GroupId;
                    ugTemp.GroupName = userGroupObjectMember.GroupName;
                    ugTemp.Description = userGroupObjectMember.Description;
                    ugTemp.IsApprove = userGroupObjectMember.IsApprove;
                    ugTemp.JoinDate = userGroupObjectMember.JoinDate;
                    ugTemp.GroupRole = userGroupObjectMember.GroupRole;
                    ugTemp.IsReceiveEmail = true;

                    // them member vao group
                    groupObject.ListUserGroup.Add(ugTemp);

                    // lay user ListUsersReference len de cap nhat danh sach ListUserGroup, ko lay tu db
                    ListUserObject.Find(u => u.UserName == user.UserName).ListUserGroup.Add(ugTemp);

                    // luu user group member xuong db
                    //session.Store(userGroupObjectMember);
                    ListUserGroupObjectMember.Add(userGroupObjectMember);
                }

                List<TopicObject> listTopicObject = new List<TopicObject>();
                TopicObject topicObject;
                int indexRandomIn50Members;
                UserObject userObjectRandomIn50Members;
                for (j = 1; j <= 50;j++ )
                {
                    topicObject = new TopicObject();
                    topicObject.Id = Guid.NewGuid().ToString();
                    topicObject.TopicName = "Topic " + j + groupObject.GroupName;
                    topicObject.Content = "Topic Content";

                    // tao ngau nhien user
                    indexRandomIn50Members = random.Next(lengthList50Members);
                    userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                    topicObject.CreateBy = userObjectRandomIn50Members;
                    topicObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                    topicObject.CreateDate = DateTime.Now;
                    topicObject.LastModified = topicObject.CreateDate;
                    topicObject.NumberOfView = 0;
                    topicObject.isDeleted = false;

                    CommentObject commentObject;
                    List<CommentObject> listCommentObject = new List<CommentObject>();
                    for (k = 1; k <= 100;k++ )
                    {
                        commentObject = new CommentObject();
                        commentObject.Id = Guid.NewGuid().ToString();
                        commentObject.Content = "Comment " + k;
                        commentObject.ParentContent = "Topic Content";
                        // tao user ngau nhien trong 50 thanh vien cua nhom
                        indexRandomIn50Members = random.Next(lengthList50Members);
                        userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                        commentObject.CreateBy = userObjectRandomIn50Members;
                        commentObject.CreateDate = DateTime.Now;
                        commentObject.isDeleted = false;
                        listCommentObject.Add(commentObject);
                    }

                    topicObject.ListComment = listCommentObject;
                    topicObject.NumberOfComment = Convert.ToUInt32(listCommentObject.Count);

                    listTopicObject.Add(topicObject);

                    // luu tat ca topic vao day
                    ListTopicObject.Add(topicObject);
                }

                TopicObject tempTopic;
                // them denormalize topic vao group
                foreach (TopicObject topic in listTopicObject)
                {
                    var reg = listUserObjectMember.Find(x => x.Id == topic.CreateBy.Id).Region;
                    tempTopic = new TopicObject();
                    tempTopic.Id = reg + "-" + topic.Id;
                    tempTopic.TopicName = topic.TopicName;
                    tempTopic.CreateBy = topic.CreateBy;
                    tempTopic.LastModified = topic.LastModified;
                    tempTopic.NumberOfComment = topic.NumberOfComment;
                    tempTopic.NumberOfView = topic.NumberOfView;
                    tempTopic.isDeleted = topic.isDeleted;

                    groupObject.ListTopic.Add(tempTopic);
                }

                // luu vao session userGroupObject
                //session.Store(userGroupObject);
                // group xong het roi, luu xuong thoi
                //session.Store(groupObject);

                ListUserGroupObject.Add(userGroupObject);
                ListGroupObject.Add(groupObject);
            }

            ListUserObjectReference.Clear();

            #region RavenDB 2.0
            /*
            using (var bulkInsert = documentStoreShard.BulkInsert())
            {
                foreach (UserObject u in ListUserObject)
                {
                    bulkInsert.Store(u);
                }
                //foreach (GroupObject g in ListGroupObject)
                //{
                //    bulkInsert.Store(g);
                //}
                //foreach (UserGroupObject u in ListUserGroupObject)
                //{
                //    bulkInsert.Store(u);
                //}
                //foreach (UserGroupObject u in ListUserGroupObjectMember)
                //{
                //    bulkInsert.Store(u);
                //}
                //foreach (TopicObject t in ListTopicObject)
                //{
                //    bulkInsert.Store(t);
                //}
            }
            */
            #endregion

            #region RavenDB 1.0
            try
            {
                using (var session1 = documentStoreShard.OpenSession())
                {
                    foreach (UserObject u in ListUserObject)
                    {
                        session1.Store(u);
                    }
                    session1.SaveChanges();
                    session1.Dispose();
                    ListUserObject.Clear();
                }

                using (var session1 = documentStoreShard.OpenSession())
                {
                    foreach (GroupObject g in ListGroupObject)
                    {
                        session1.Store(g);
                    }
                    session1.SaveChanges();
                    session1.Dispose();
                    ListGroupObject.Clear();
                }
                using (var session1 = documentStoreShard.OpenSession())
                {
                    foreach (UserGroupObject u in ListUserGroupObject)
                    {
                        session1.Store(u);
                    }
                    session1.SaveChanges();
                    session1.Dispose();
                    ListUserGroupObject.Clear();
                }
                using (var session1 = documentStoreShard.OpenSession())
                {
                    foreach (UserGroupObject u in ListUserGroupObjectMember)
                    {
                        session1.Store(u);
                    }
                    session1.SaveChanges();
                    session1.Dispose();
                    ListUserGroupObjectMember.Clear();
                }
                using (var session1 = documentStoreShard.OpenSession())
                {
                    foreach (TopicObject t in ListTopicObject)
                    {
                        session1.Store(t);
                    }
                    session1.SaveChanges();
                    session1.Dispose();
                    ListTopicObject.Clear();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Insert error!" + ex);
            }
            #endregion
        }

        public static void FullTextSearch()
        {
            //Console.Write("Enter key word:");
            //string search = Console.ReadLine();
            //var session = documentStoreShard.OpenSession();
            //var list = session.Query<GroupObject, GroupObject_Search>().Search(x=>x.GroupName, search);
            //foreach (var l in list)
            //{
            //    Console.WriteLine(l.GroupName);
            //}
        }
        

        public static List<Comment> ConvertXElementToComment(List<XElement> listElement)
        { 
            Comment comment = new Comment();
            List<Comment> listComment = new List<Comment>();
            foreach (var xElement in listElement)
            {
                comment.Content = xElement.Value;
                listComment.Add(comment);
            }
            return listComment;
        }

        public class GroupRSS
        {
            public string GroupName { get; set; }
            public string LinkRSS { get; set; }
        }

        public class User
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string Region { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }
        public class Topic
        {
            public string TopicName { get; set; }
            public string Content { get; set; }
            public List<Comment> ListComment { get; set; }
            public Topic()
            {
                ListComment = new List<Comment>();
            }
        }
        public class Comment
        {
            public string Content { get; set; }
        }

        // ham luu du lieu xuong 3 server
        /*
        public static void InsertDataSampleToServersVersion2()
        {
            string databaseName;
            IDocumentStore documentStoreShard;
            IDocumentStore[] documentStores;
            List<string> ServerRegion;
            string ServerGeneral;

            databaseName = "UITNoSQLDB6";
            string ConnectRavenDB = "http://localhost:8081,Asia;http://localhost:8082,MiddleEast;http://localhost:8083,America";
            string[] connects = ConnectRavenDB.Split(';');

            var shards = new Dictionary<string, IDocumentStore>();
            documentStores = new DocumentStore[connects.Length];
            string shardName = string.Empty;
            string shardUrl = string.Empty;
            int i = 0;
            ServerRegion = new List<string>();

            foreach (var connect in connects)
            {
                var co = connect.Split(',');
                shardUrl = co[0];
                shardName = co[1];
                shards.Add(shardName, new DocumentStore { Url = shardUrl, DefaultDatabase = databaseName });

                documentStores[i++] = new DocumentStore { Url = shardUrl, DefaultDatabase = databaseName }.Initialize();
                ServerRegion.Add(shardName);
            }

            DocumentConvention Conventions;
            Conventions = shards.First().Value.Conventions.Clone();
            var shardStrategy = new ShardStrategy(shards)
                .ShardingOn<UserObject>(u => u.Region)
                .ShardingOn<GroupObject>(g => g.CreateBy.Id)
                .ShardingOn<TopicObject>(t => t.CreateBy.Id)
                .ShardingOn<UserGroupObject>(u => u.UserId)
                .ShardingOn<GroupRoleObject>(r => r.IsGeneral);
            shardStrategy.Conventions.IdentityPartsSeparator = "-";
            shardStrategy.ModifyDocumentId = (convention, shardId, documentId) => shardId + convention.IdentityPartsSeparator + documentId;

            documentStoreShard = new ShardedDocumentStore(shardStrategy).Initialize();
            documentStoreShard.Conventions.MaxNumberOfRequestsPerSession = 100000;

            //set server general
            ServerGeneral = shardName;

            var session = documentStoreShard.OpenSession();
            GroupRoleObject groupRoleManager, groupRoleOwner, groupRoleMember;
            groupRoleManager = new GroupRoleObject();
            groupRoleManager.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRoleManager.GroupName = "Manager";
            groupRoleManager.IsGeneral = ServerGeneral;
            session.Store(groupRoleManager);

            groupRoleMember = new GroupRoleObject();
            groupRoleMember.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRoleMember.GroupName = "Member";
            groupRoleMember.IsGeneral = ServerGeneral;
            session.Store(groupRoleMember);

            groupRoleOwner = new GroupRoleObject();
            groupRoleOwner.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRoleOwner.GroupName = "Owner";
            groupRoleOwner.IsGeneral = ServerGeneral;
            session.Store(groupRoleOwner);

            // 2 danh sách lưu dữ liệu cục bộ để sử dụng cho lưu group và lưu Topic, Comment
            List<UserObject> ListUserObject = new List<UserObject>();
            List<GroupObject> ListGroupObject = new List<GroupObject>();

            UserObject userObject = null;
            GroupObject groupObject = null;

            // đọc dữ liệu 15.000 Users từ file xml
            string usersInfroXmlFilePath = STR_DATA_SERVER_USERS + "ListUsers.xml";
            XElement xmlUser = XElement.Load(usersInfroXmlFilePath);
            List<User> listUser = (from u in xmlUser.Elements("UserObject")
                                   select new User
                                   {
                                       FullName = u.Element("FullName").Value,
                                       UserName = u.Element("UserName").Value,
                                       Email = u.Element("Email").Value,
                                       Region = u.Element("Region").Value
                                   }
                                   ).ToList();
            foreach (var user in listUser)
            {
                userObject = new UserObject();
                userObject.Id = Guid.NewGuid().ToString();
                userObject.FullName = user.FullName;
                userObject.UserName = user.UserName;
                userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                userObject.Email = user.Email;
                userObject.Region = user.Region;
                session.Store(userObject);
                ListUserObject.Add(userObject);
            }
            // luu 3 GroupRole va 15.000 User xuong db
            session.SaveChanges();
            session.Dispose();

            // mo session moi
            session = documentStoreShard.OpenSession();

            // doc du lieu Groups tu xml
            string groupsInfoXmlFilePath = STR_DATA_SERVER_GROUPRSS + "GroupRSS.xml";
            if (File.Exists(groupsInfoXmlFilePath))
            {
                XElement xmlGroupRSS = XElement.Load(groupsInfoXmlFilePath);
                List<GroupRSS> listGroup = (from u in xmlGroupRSS.Elements("GroupRSS")
                                            select new GroupRSS
                                            {
                                                GroupName = u.Element("GroupName").Value,
                                            }).ToList();

                Random random = new Random();
                int indexRandom, lengthListUserObject = ListUserObject.Count;
                UserObject userObjectRandom;
                UserObject userObjectReal;
                foreach (var group in listGroup)
                {
                    // tao group
                    groupObject = new GroupObject();
                    groupObject.Id = Guid.NewGuid().ToString();
                    groupObject.GroupName = group.GroupName;
                    groupObject.Description = "This is new group";
                    groupObject.IsPublic = false;
                    groupObject.CreateDate = DateTime.Now;

                    // tao ngau nhien user trong 15.000 Users
                    indexRandom = random.Next(lengthListUserObject);
                    userObjectRandom = ListUserObject[indexRandom];

                    groupObject.CreateBy = userObjectRandom;
                    groupObject.NewEvent = new GroupEvent();
                    groupObject.NewEvent.Title = "New group";
                    groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                    groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                    groupObject.NewEvent.UserId = userObjectRandom.Id;

                    // tao user group cho group vua tao
                    var userGroupObject = new UserGroupObject();
                    userGroupObject.Id = Guid.NewGuid().ToString();
                    userGroupObject.UserId = userObjectRandom.Id;
                    userGroupObject.GroupId = groupObject.Id;
                    userGroupObject.GroupName = groupObject.GroupName;
                    userGroupObject.Description = groupObject.Description;
                    userGroupObject.IsApprove = UserGroupStatus.Approve;
                    userGroupObject.IsReceiveEmail = true;
                    userGroupObject.JoinDate = DateTime.Now;
                    userGroupObject.GroupRole = groupRoleOwner;

                    // them usergroup vap group
                    groupObject.ListUserGroup.Add(userGroupObject);

                    userObjectReal = session.Load<UserObject>(userObjectRandom.Id); // load user len tu db
                    userObjectReal.ListUserGroup.Add(userGroupObject); // them user group vao user

                    // luu vao session userGroupObject,userObjectReal
                    session.Store(userGroupObject);
                    session.Store(userObjectReal);

                    // tao 50 member cho moi group
                    int indexRandomMember;
                    UserObject userObjectMember;
                    List<UserObject> listUserObjectMember = new List<UserObject>();
                    int numberOfMember = 0;
                    while (numberOfMember < 50)
                    {
                        indexRandomMember = random.Next(lengthListUserObject);
                        if (indexRandomMember != indexRandom)
                        {
                            userObjectMember = ListUserObject[indexRandomMember];
                            listUserObjectMember.Add(userObjectMember);
                            numberOfMember += 1;
                        }
                    }

                    UserGroupObject userGroupObjectMember;
                    foreach (var user in listUserObjectMember) // duyet qua danh sach 50 member de luu xuong db
                    {
                        userGroupObjectMember = new UserGroupObject();
                        userGroupObjectMember.Id = Guid.NewGuid().ToString();
                        userGroupObjectMember.UserId = user.Id;
                        userGroupObjectMember.GroupId = groupObject.Id;
                        userGroupObjectMember.GroupName = groupObject.GroupName;
                        userGroupObjectMember.Description = groupObject.Description;
                        userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                        userGroupObjectMember.JoinDate = DateTime.Now;
                        userGroupObjectMember.GroupRole = groupRoleMember;
                        groupObject.ListUserGroup.Add(userGroupObjectMember); // them member vao group

                        // lay user tu db len de cap nhat danh sach ListUserGroup


                    }

                    // doc danh sach topic tu xml de luu vao group
                    string strSubFolderTopic;
                    List<TopicObject> listTopicObject = new List<TopicObject>();
                    foreach (string path1 in Directory.GetDirectories(STR_DATA_SERVER_TOPICS)) // path1 = "91538b3e-3f95-4525-9c81-512ea6abe16c"
                    {
                        string path2 = Directory.GetDirectories(path1).FirstOrDefault(x => x == groupObject.GroupName); // path2 = "24h - Ẩm thực"
                        if (path2 != null)
                        {
                            DirectoryInfo dir = new DirectoryInfo(path2);
                            FileInfo file = dir.GetFiles().FirstOrDefault();
                            if (file.Exists)
                            {
                                // doc topic len
                                string topicsInfoXmlFilePath = path2 + "/ListTopics.xml";
                                XElement xmlTopics = XElement.Load(topicsInfoXmlFilePath);
                                List<Topic> listTopics = (from u in xmlGroupRSS.Elements("TopicObject")
                                                          let y = u.Descendants("Comment").ToList()
                                                          select new Topic
                                                          {
                                                              TopicName = u.Element("TopicName").Value,
                                                              Content = u.Element("Content").Value,
                                                              ListComment = ConvertXElementToComment(y)
                                                          }).ToList();
                                TopicObject topicObject;
                                foreach (Topic topic in listTopics)
                                {
                                    topicObject = new TopicObject();
                                    topicObject.Id = Guid.NewGuid().ToString();
                                    topicObject.TopicName = topic.TopicName;
                                    topicObject.Content = topic.Content;

                                    // tao ngau nhien user

                                    //topicObject.CreateBy 
                                }
                            }
                        }
                    }




                    // luu usergroup, group, user xuong db

                    session.Store(groupObject);


                }
            }
            session.SaveChanges();
            session.Dispose();

        }
         */

        // ham moi nhat Test
        public static void InsertLargeDataUsingPatchingAPI()
        {
            int i, j, k, count = 0;
            int numberOfGroups = 100, numberOfTopicsForEachGroup = 50, numberOfCommentsForEachTopic = 100;
            int numberOfUsers = 500;
            // Asia: 0,1
            // MiddleEast: 
            // America: 
            int x, y = 2; // y so vong lap nho 
            for (x = 0; x < y; x++)
            {
                var session = documentStoreShard.OpenSession();

                // danh sach user de tham chieu
                List<UserObject> ListUserObjectAsia = new List<UserObject>();
                UserObject userObject;
                GroupObject groupObject;
                DataTable dt;
                dt = GetListFullName(numberOfUsers);

                for (i = 0; i < numberOfUsers; i++)
                {
                    userObject = new UserObject();
                    userObject.Id = Guid.NewGuid().ToString();
                    userObject.FullName = dt.Rows[i]["FullName"].ToString();
                    userObject.UserName = "username" + (x * numberOfUsers + i + 1).ToString();
                    userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                    userObject.Email = "username" + (x * numberOfUsers + i + 1).ToString() + "@gmail.com";
                    userObject.Region = ServerRegion[0];
                    ListUserObjectAsia.Add(userObject);
                    session.Store(userObject);
                }
                // luu Users xuong db
                session.SaveChanges();
                session.Dispose();

                Random random = new Random();
                int indexRandom, lengthListUserObject = ListUserObjectAsia.Count;
                UserObject userObjectRandom = new UserObject();
                for (i = 1; i <= numberOfGroups; i++)
                {
                    //using (session = documentStoreShard.OpenSession())
                    if (count == 0)
                    {
                        session = documentStoreShard.OpenSession();
                    }
                    {
                        // tao group
                        groupObject = new GroupObject();
                        groupObject.Id = Guid.NewGuid().ToString();
                        groupObject.GroupName = "Group " + (x * numberOfGroups + i);
                        groupObject.Description = "This is new group";
                        groupObject.IsPublic = false;
                        groupObject.CreateDate = DateTime.Now;

                        // tao ngau nhien user trong 500 Users Asia
                        indexRandom = random.Next(lengthListUserObject);
                        userObjectRandom = ListUserObjectAsia[indexRandom];

                        groupObject.CreateBy = userObjectRandom;
                        groupObject.NewEvent = new GroupEvent();
                        groupObject.NewEvent.Title = "New group";
                        groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                        groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                        groupObject.NewEvent.UserId = userObjectRandom.Id;

                        // tao user group cho group vua tao
                        var userGroupObject = new UserGroupObject();
                        userGroupObject.Id = Guid.NewGuid().ToString();
                        userGroupObject.UserId = userObjectRandom.Id;
                        userGroupObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                        userGroupObject.GroupName = groupObject.GroupName;
                        userGroupObject.Description = groupObject.Description;
                        userGroupObject.IsApprove = UserGroupStatus.Approve;
                        userGroupObject.IsReceiveEmail = true;
                        userGroupObject.JoinDate = DateTime.Now;
                        userGroupObject.GroupRole = groupRoleOwner;
                        userGroupObject.IsReceiveEmail = true;

                        session.Store(userGroupObject);

                        // them UserGroup vao GroupObject
                        groupObject.ListUserGroup.Add(userGroupObject);
                        // them UserGroup vao UserObject
                        //UserObject userLoad = session.Load<UserObject>(userObjectRandom.Id);
                        //userLoad.ListUserGroup.Add(userGroupObject);
                        //session.Store(userLoad);
                        //using patching API
                        documentStores[0].DatabaseCommands.Patch(
                        userObjectRandom.Id,
                        new[]
                            {
                                new PatchRequest
                                    {
                                        Type = PatchCommandType.Add,
                                        Name = "ListUserGroup",
                                        Value = RavenJObject.FromObject(userGroupObject)
                                    }
                            });

                        // tao 50 member cho moi group
                        int indexRandomMember;
                        UserObject userObjectMember;
                        List<UserObject> listUserObjectMember = new List<UserObject>();
                        indexRandomMember = random.Next(0, numberOfUsers - 51);
                        listUserObjectMember = ListUserObjectAsia.GetRange(indexRandomMember, 50);
                        userObjectMember = listUserObjectMember.Find(u => u.Id.Equals(userObjectRandom.Id));
                        if (userObjectMember != null)
                        {
                            listUserObjectMember.Remove(userObjectMember);
                        }

                        UserGroupObject userGroupObjectMember;
                        foreach (var user in listUserObjectMember) // duyet qua danh sach 50 member de luu xuong db
                        {
                            userGroupObjectMember = new UserGroupObject();
                            userGroupObjectMember.Id = Guid.NewGuid().ToString();
                            userGroupObjectMember.UserId = user.Id;
                            userGroupObjectMember.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                            userGroupObjectMember.GroupName = groupObject.GroupName;
                            userGroupObjectMember.Description = groupObject.Description;
                            userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                            userGroupObjectMember.JoinDate = DateTime.Now;
                            userGroupObjectMember.GroupRole = groupRoleMember;
                            userGroupObjectMember.IsReceiveEmail = true;

                            session.Store(userGroupObjectMember);

                            // them member vao group
                            groupObject.ListUserGroup.Add(userGroupObjectMember);

                            // them user group vao user
                            //userLoad = session.Load<UserObject>(user.Id);
                            //userLoad.ListUserGroup.Add(userGroupObjectMember);
                            //session.Store(userLoad);
                            //using patching API
                            documentStores[0].DatabaseCommands.Patch(
                            user.Id,
                            new[]
                            {
                                new PatchRequest
                                    {
                                        Type = PatchCommandType.Add,
                                        Name = "ListUserGroup",
                                        Value = RavenJObject.FromObject(userGroupObjectMember)
                                    }
                            });
                        }

                        List<TopicObject> listTopicObject = new List<TopicObject>();
                        TopicObject topicObject;
                        int indexRandomIn50Members;
                        int lengthOfListUserObjectMember = listUserObjectMember.Count();
                        UserObject userObjectRandomIn50Members;

                        for (j = 1; j <= numberOfTopicsForEachGroup; j++)
                        {
                            topicObject = new TopicObject();
                            topicObject.Id = Guid.NewGuid().ToString();
                            topicObject.TopicName = "Topic" + j + " " + groupObject.GroupName;
                            topicObject.Content = "Topic content.";

                            // tao ngau nhien user trong 50 thanh vien cua nhom
                            indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                            userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                            topicObject.CreateBy = userObjectRandomIn50Members;
                            topicObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                            topicObject.CreateDate = DateTime.Now;
                            topicObject.LastModified = topicObject.CreateDate;
                            topicObject.NumberOfView = 0;
                            topicObject.isDeleted = false;

                            CommentObject commentObject;
                            List<CommentObject> listCommentObject = new List<CommentObject>();
                            for (k = 1; k <= numberOfCommentsForEachTopic; k++)
                            {
                                commentObject = new CommentObject();
                                commentObject.Id = Guid.NewGuid().ToString();
                                commentObject.Content = "Comment " + k;
                                commentObject.ParentContent = "Topic content.";
                                // tao user ngau nhien trong 50 thanh vien cua nhom
                                indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                                userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                                commentObject.CreateBy = userObjectRandomIn50Members;
                                commentObject.CreateDate = DateTime.Now;
                                commentObject.isDeleted = false;
                                listCommentObject.Add(commentObject);
                            }

                            topicObject.ListComment = listCommentObject;
                            topicObject.NumberOfComment = Convert.ToUInt32(listCommentObject.Count);

                            listTopicObject.Add(topicObject);

                            session.Store(topicObject);

                        }

                        // them denormalize topic vao group
                        foreach (TopicObject topic in listTopicObject)
                        {
                            groupObject.ListTopic.Add(topic);
                        }

                        // group xong het roi, luu xuong thoi
                        session.Store(groupObject);
                        count++;
                        if (count == 5)
                        {
                            session.SaveChanges();
                            session.Dispose();
                            count = 0;
                        }
                    }
                }
            }
        }

        // ham moi nhat Test
        public static void InsertLargeDataNewestTest()
        {
            int i, j, k, count = 0;
            int numberOfGroups =100, numberOfTopicsForEachGroup = 50, numberOfCommentsForEachTopic =100;
            int numberOfUsers = 500;
            // Asia: 0,1,6,7,8,9,10,11,12,13-22,23,24 
            // MiddleEast: 2,3,25,26,27,28
            // America: 4,5,29-48
            int x, y = 49; // y so vong lap nho 
            for (x = 29; x < y; x++)
            {
                var session = documentStoreShard.OpenSession();
                
                // danh sach user de tham chieu
                List<UserObject> ListUserObjectAsia = new List<UserObject>();
                UserObject userObject;
                GroupObject groupObject;
                DataTable dt;
                dt = GetListFullName(numberOfUsers);

                for (i = 0; i < numberOfUsers; i++)
                {
                    userObject = new UserObject();
                    userObject.Id = Guid.NewGuid().ToString();
                    userObject.FullName = dt.Rows[i]["FullName"].ToString();
                    userObject.UserName = "username" + (x * numberOfUsers + i + 1).ToString();
                    userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                    userObject.Email = "username" + (x * numberOfUsers + i + 1).ToString() + "@gmail.com";
                    userObject.Region = ServerRegion[2];
                    ListUserObjectAsia.Add(userObject);
                    session.Store(userObject);
                }
                // luu Users xuong db
                session.SaveChanges();
                session.Dispose();

                Random random = new Random();
                int indexRandom, lengthListUserObject = ListUserObjectAsia.Count;
                UserObject userObjectRandom = new UserObject();
                for (i = 1; i <= numberOfGroups; i++)
                {
                    //using (session = documentStoreShard.OpenSession())
                    if (count == 0)
                    {
                        session = documentStoreShard.OpenSession();
                    }
                    {
                        // tao group
                        groupObject = new GroupObject();
                        groupObject.Id = Guid.NewGuid().ToString();
                        groupObject.GroupName = "Group " + (x*numberOfGroups + i);
                        groupObject.Description = "This is new group";
                        groupObject.IsPublic = false;
                        groupObject.CreateDate = DateTime.Now;

                        // tao ngau nhien user trong 500 Users Asia
                        indexRandom = random.Next(lengthListUserObject);
                        userObjectRandom = ListUserObjectAsia[indexRandom];

                        groupObject.CreateBy = userObjectRandom;
                        groupObject.NewEvent = new GroupEvent();
                        groupObject.NewEvent.Title = "New group";
                        groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                        groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                        groupObject.NewEvent.UserId = userObjectRandom.Id;

                        // tao user group cho group vua tao
                        var userGroupObject = new UserGroupObject();
                        userGroupObject.Id = Guid.NewGuid().ToString();
                        userGroupObject.UserId = userObjectRandom.Id;
                        userGroupObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                        userGroupObject.GroupName = groupObject.GroupName;
                        userGroupObject.Description = groupObject.Description;
                        userGroupObject.IsApprove = UserGroupStatus.Approve;
                        userGroupObject.IsReceiveEmail = true;
                        userGroupObject.JoinDate = DateTime.Now;
                        userGroupObject.GroupRole = groupRoleOwner;
                        userGroupObject.IsReceiveEmail = true;

                        session.Store(userGroupObject);

                        // them UserGroup vao GroupObject
                        groupObject.ListUserGroup.Add(userGroupObject);
                        // them UserGroup vao UserObject
                        UserObject userLoad = session.Load<UserObject>(userObjectRandom.Id);
                        userLoad.ListUserGroup.Add(userGroupObject);
                        session.Store(userLoad);

                        // tao 50 member cho moi group
                        int indexRandomMember;
                        UserObject userObjectMember;
                        List<UserObject> listUserObjectMember = new List<UserObject>();
                        indexRandomMember = random.Next(0, numberOfUsers - 51);
                        listUserObjectMember = ListUserObjectAsia.GetRange(indexRandomMember, 50);
                        userObjectMember = listUserObjectMember.Find(u => u.Id.Equals(userObjectRandom.Id));
                        if (userObjectMember != null)
                        {
                            listUserObjectMember.Remove(userObjectMember);
                        }

                        UserGroupObject userGroupObjectMember;
                        foreach (var user in listUserObjectMember) // duyet qua danh sach 50 member de luu xuong db
                        {
                            userGroupObjectMember = new UserGroupObject();
                            userGroupObjectMember.Id = Guid.NewGuid().ToString();
                            userGroupObjectMember.UserId = user.Id;
                            userGroupObjectMember.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                            userGroupObjectMember.GroupName = groupObject.GroupName;
                            userGroupObjectMember.Description = groupObject.Description;
                            userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                            userGroupObjectMember.JoinDate = DateTime.Now;
                            userGroupObjectMember.GroupRole = groupRoleMember;
                            userGroupObjectMember.IsReceiveEmail = true;

                            session.Store(userGroupObjectMember);

                            // them member vao group
                            groupObject.ListUserGroup.Add(userGroupObjectMember);

                            // them user group vao user
                            userLoad = session.Load<UserObject>(user.Id);
                            userLoad.ListUserGroup.Add(userGroupObjectMember);
                            session.Store(userLoad);
                        }

                        List<TopicObject> listTopicObject = new List<TopicObject>();
                        TopicObject topicObject;
                        int indexRandomIn50Members;
                        int lengthOfListUserObjectMember = listUserObjectMember.Count();
                        UserObject userObjectRandomIn50Members;

                        for (j = 1; j <= numberOfTopicsForEachGroup; j++)
                        {
                            topicObject = new TopicObject();
                            topicObject.Id = Guid.NewGuid().ToString();
                            topicObject.TopicName = "Topic" + j + " " + groupObject.GroupName;
                            topicObject.Content = "Topic content.";

                            // tao ngau nhien user trong 50 thanh vien cua nhom
                            indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                            userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                            topicObject.CreateBy = userObjectRandomIn50Members;
                            topicObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                            topicObject.CreateDate = DateTime.Now;
                            topicObject.LastModified = topicObject.CreateDate;
                            topicObject.NumberOfView = 0;
                            topicObject.isDeleted = false;

                            CommentObject commentObject;
                            List<CommentObject> listCommentObject = new List<CommentObject>();
                            for (k = 1; k <= numberOfCommentsForEachTopic; k++)
                            {
                                commentObject = new CommentObject();
                                commentObject.Id = Guid.NewGuid().ToString();
                                commentObject.Content = "Comment " + k;
                                commentObject.ParentContent = "Topic content.";
                                // tao user ngau nhien trong 50 thanh vien cua nhom
                                indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                                userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                                commentObject.CreateBy = userObjectRandomIn50Members;
                                commentObject.CreateDate = DateTime.Now;
                                commentObject.isDeleted = false;
                                listCommentObject.Add(commentObject);
                            }

                            topicObject.ListComment = listCommentObject;
                            topicObject.NumberOfComment = Convert.ToUInt32(listCommentObject.Count);

                            listTopicObject.Add(topicObject);

                            session.Store(topicObject);

                        }

                        // them denormalize topic vao group
                        foreach (TopicObject topic in listTopicObject)
                        {
                            groupObject.ListTopic.Add(topic);
                        }

                        // group xong het roi, luu xuong thoi
                        session.Store(groupObject);
                        count++;
                        if (count == 8)
                        {
                            session.SaveChanges();
                            session.Dispose();
                            count = 0;
                        }
                    }
                }
            }
        }

        // ham moi nhat
        public static void InsertLargeDataNewest()
        {
            var session = documentStoreShard.OpenSession();
            GroupRoleObject groupRoleManager, groupRoleOwner, groupRoleMember;
            groupRoleManager = new GroupRoleObject();
            groupRoleManager.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRoleManager.GroupName = "Manager";
            groupRoleManager.IsGeneral = ServerRegion[0];
            session.Store(groupRoleManager);

            groupRoleMember = new GroupRoleObject();
            groupRoleMember.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRoleMember.GroupName = "Member";
            groupRoleMember.IsGeneral = ServerRegion[0];
            session.Store(groupRoleMember);

            groupRoleOwner = new GroupRoleObject();
            groupRoleOwner.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRoleOwner.GroupName = "Owner";
            groupRoleOwner.IsGeneral = ServerRegion[0];
            session.Store(groupRoleOwner);

            // danh sach user de tham chieu
            List<UserObject> ListUserObjectAsia = new List<UserObject>();
            //List<UserObject> ListUserObjectMiddleEast = new List<UserObject>();
            //List<UserObject> ListUserObjectAmerica = new List<UserObject>();

            UserObject userObject;
            GroupObject groupObject;
            DataTable dt = GetListFullName(5000);

            int i, j, k;
            for (i = 0; i < 5000; i++)
            {
                userObject = new UserObject();
                userObject.Id = Guid.NewGuid().ToString();
                userObject.FullName = dt.Rows[i]["FullName"].ToString();
                userObject.UserName = "username" + (i + 1).ToString();
                userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                userObject.Email = "username" + (i + 1).ToString() + "@gmail.com";
                userObject.Region = ServerRegion[0];
                ListUserObjectAsia.Add(userObject);
                session.Store(userObject);
            }
            // luu 3 GroupRole va 5.000 User xuong db Asia
            session.SaveChanges();
            session.Dispose();

            int count = 0;

            
            Random random = new Random();
            int indexRandom, lengthListUserObject = ListUserObjectAsia.Count;
            UserObject userObjectRandom = new UserObject();
            for (i = 1; i <= 100; i++)
            {
                session = documentStoreShard.OpenSession();

                // tao group
                groupObject = new GroupObject();
                groupObject.Id = Guid.NewGuid().ToString();
                groupObject.GroupName = "Group " + i;
                groupObject.Description = "This is new group";
                groupObject.IsPublic = false;
                groupObject.CreateDate = DateTime.Now;

                // tao ngau nhien user trong 5.000 Users Asia
                indexRandom = random.Next(lengthListUserObject);
                userObjectRandom = ListUserObjectAsia[indexRandom];

                groupObject.CreateBy = userObjectRandom;
                groupObject.NewEvent = new GroupEvent();
                groupObject.NewEvent.Title = "New group";
                groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                groupObject.NewEvent.UserId = userObjectRandom.Id;

                // tao user group cho group vua tao
                var userGroupObject = new UserGroupObject();
                userGroupObject.Id = Guid.NewGuid().ToString();
                userGroupObject.UserId = userObjectRandom.Id;
                userGroupObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                userGroupObject.GroupName = groupObject.GroupName;
                userGroupObject.Description = groupObject.Description;
                userGroupObject.IsApprove = UserGroupStatus.Approve;
                userGroupObject.IsReceiveEmail = true;
                userGroupObject.JoinDate = DateTime.Now;
                userGroupObject.GroupRole = groupRoleOwner;
                userGroupObject.IsReceiveEmail = true;

                session.Store(userGroupObject);

                // them UserGroup vao GroupObject
                groupObject.ListUserGroup.Add(userGroupObject);
                // them UserGroup vao UserObject
                UserObject userLoad = session.Load<UserObject>(userObjectRandom.Id);
                userLoad.ListUserGroup.Add(userGroupObject);
                session.Store(userLoad);

                // tao 50 member cho moi group
                int indexRandomMember;
                UserObject userObjectMember;
                List<UserObject> listUserObjectMember = new List<UserObject>();
                indexRandomMember = random.Next(0, 4949);
                listUserObjectMember = ListUserObjectAsia.GetRange(indexRandomMember, 50);
                userObjectMember = listUserObjectMember.Find(u => u.Id.Equals(userObjectRandom.Id));
                if (userObjectMember != null)
                {
                    listUserObjectMember.Remove(userObjectMember);
                }

                UserGroupObject userGroupObjectMember;
                foreach (var user in listUserObjectMember) // duyet qua danh sach 50 member de luu xuong db
                {
                    userGroupObjectMember = new UserGroupObject();
                    userGroupObjectMember.Id = Guid.NewGuid().ToString();
                    userGroupObjectMember.UserId = user.Id;
                    userGroupObjectMember.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                    userGroupObjectMember.GroupName = groupObject.GroupName;
                    userGroupObjectMember.Description = groupObject.Description;
                    userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                    userGroupObjectMember.JoinDate = DateTime.Now;
                    userGroupObjectMember.GroupRole = groupRoleMember;
                    userGroupObjectMember.IsReceiveEmail = true;

                    session.Store(userGroupObjectMember);

                    // them member vao group
                    groupObject.ListUserGroup.Add(userGroupObjectMember);

                    // them user group vao user
                    userLoad = session.Load<UserObject>(user.Id);
                    userLoad.ListUserGroup.Add(userGroupObjectMember);
                    session.Store(userLoad);
                }

                List<TopicObject> listTopicObject = new List<TopicObject>();
                TopicObject topicObject;
                int indexRandomIn50Members;
                int lengthOfListUserObjectMember = listUserObjectMember.Count();
                UserObject userObjectRandomIn50Members;

                for (j = 1; j <= 100; j++)
                {
                    topicObject = new TopicObject();
                    topicObject.Id = Guid.NewGuid().ToString();
                    topicObject.TopicName = "Topic" + j + " " + groupObject.GroupName;
                    topicObject.Content = "Topic content.";

                    // tao ngau nhien user trong 50 thanh vien cua nhom
                    indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                    userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                    topicObject.CreateBy = userObjectRandomIn50Members;
                    topicObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                    topicObject.CreateDate = DateTime.Now;
                    topicObject.LastModified = topicObject.CreateDate;
                    topicObject.NumberOfView = 0;
                    topicObject.isDeleted = false;

                    CommentObject commentObject;
                    List<CommentObject> listCommentObject = new List<CommentObject>();
                    for (k = 1; k <= 100; k++)
                    {
                        commentObject = new CommentObject();
                        commentObject.Id = Guid.NewGuid().ToString();
                        commentObject.Content = "Comment " + k;
                        commentObject.ParentContent = "Topic content.";
                        // tao user ngau nhien trong 50 thanh vien cua nhom
                        indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                        userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                        commentObject.CreateBy = userObjectRandomIn50Members;
                        commentObject.CreateDate = DateTime.Now;
                        commentObject.isDeleted = false;
                        listCommentObject.Add(commentObject);
                    }

                    topicObject.ListComment = listCommentObject;
                    topicObject.NumberOfComment = Convert.ToUInt32(listCommentObject.Count);

                    listTopicObject.Add(topicObject);

                    session.Store(topicObject);

                }

                // them denormalize topic vao group
                foreach (TopicObject topic in listTopicObject)
                {
                    groupObject.ListTopic.Add(topic);
                }

                // group xong het roi, luu xuong thoi
                session.Store(groupObject);

                session.SaveChanges();
                session.Dispose();
            }
        }

        // ham
        public static void InsertLargeData()
        {
            var session = documentStoreShard.OpenSession();
            GroupRoleObject groupRoleManager, groupRoleOwner, groupRoleMember;
            groupRoleManager = new GroupRoleObject();
            groupRoleManager.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRoleManager.GroupName = "Manager";
            groupRoleManager.IsGeneral = ServerGeneral;
            session.Store(groupRoleManager);

            groupRoleMember = new GroupRoleObject();
            groupRoleMember.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRoleMember.GroupName = "Member";
            groupRoleMember.IsGeneral = ServerGeneral;
            session.Store(groupRoleMember);

            groupRoleOwner = new GroupRoleObject();
            groupRoleOwner.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRoleOwner.GroupName = "Owner";
            groupRoleOwner.IsGeneral = ServerGeneral;
            session.Store(groupRoleOwner);

            // danh sach user de tham chieu
            List<UserObject> ListUserObject = new List<UserObject>();

            UserObject userObject;
            GroupObject groupObject;
            DataTable dt = GetListFullName(15000);

            int i,j,k;
            int index;
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 5000; j++)
                {
                    index = i*5000 + j;
                    userObject = new UserObject();
                    userObject.Id = Guid.NewGuid().ToString();
                    userObject.FullName = dt.Rows[index]["FullName"].ToString();
                    userObject.UserName = "username" + (index + 1).ToString();
                    userObject.Password = "c4ca4238a0b923820dcc509a6f75849b";
                    userObject.Email = "username" + (index + 1).ToString() + "@gmail.com";
                    userObject.Region = ServerRegion[i];
                    ListUserObject.Add(userObject);
                    session.Store(userObject);
                }
            }
            // luu 3 GroupRole va 15.000 User xuong db
            session.SaveChanges();
            session.Dispose();

            dt.Dispose();

            int count = 0;

            Random random = new Random();
            int indexRandom, lengthListUserObject = ListUserObject.Count;
            UserObject userObjectRandom = new UserObject();
            for (i = 1; i <= 300;i++ )
            {
                if (count == 0)
                {
                    session = documentStoreShard.OpenSession();
                }
                // tao group
                groupObject = new GroupObject();
                groupObject.Id = Guid.NewGuid().ToString();
                groupObject.GroupName = "Group " + i;
                groupObject.Description = "This is new group";
                groupObject.IsPublic = false;
                groupObject.CreateDate = DateTime.Now;

                // tao ngau nhien user trong 15.000 Users
                indexRandom = random.Next(lengthListUserObject);
                userObjectRandom = ListUserObject[indexRandom];

                groupObject.CreateBy = userObjectRandom;
                groupObject.NewEvent = new GroupEvent();
                groupObject.NewEvent.Title = "New group";
                groupObject.NewEvent.CreateDate = groupObject.CreateDate;
                groupObject.NewEvent.CreateBy = userObjectRandom.FullName;
                groupObject.NewEvent.UserId = userObjectRandom.Id;

                // tao user group cho group vua tao
                var userGroupObject = new UserGroupObject();
                userGroupObject.Id = Guid.NewGuid().ToString();
                userGroupObject.UserId = userObjectRandom.Id;
                userGroupObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                userGroupObject.GroupName = groupObject.GroupName;
                userGroupObject.Description = groupObject.Description;
                userGroupObject.IsApprove = UserGroupStatus.Approve;
                userGroupObject.IsReceiveEmail = true;
                userGroupObject.JoinDate = DateTime.Now;
                userGroupObject.GroupRole = groupRoleOwner;
                userGroupObject.IsReceiveEmail = true;

                session.Store(userGroupObject);

                // them UserGroup vao GroupObject
                groupObject.ListUserGroup.Add(userGroupObject);
                // them UserGroup vao UserObject
                UserObject userLoad = session.Load<UserObject>(userObjectRandom.Id);
                userLoad.ListUserGroup.Add(userGroupObject);
                session.Store(userLoad);

                // tao 50 member cho moi group
                int indexRandomMember;
                UserObject userObjectMember;
                List<UserObject> listUserObjectMember = new List<UserObject>();
                indexRandomMember = random.Next(0,14949);
                listUserObjectMember = ListUserObject.GetRange(indexRandomMember,50);
                userObjectMember = listUserObjectMember.Find(u => u.Id.Equals(userObjectRandom.Id));
                if(userObjectMember != null)
                {
                    listUserObjectMember.Remove(userObjectMember);
                }

                UserGroupObject userGroupObjectMember;
                foreach (var user in listUserObjectMember) // duyet qua danh sach 50 member de luu xuong db
                {
                    userGroupObjectMember = new UserGroupObject();
                    userGroupObjectMember.Id = Guid.NewGuid().ToString();
                    userGroupObjectMember.UserId = user.Id;
                    userGroupObjectMember.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                    userGroupObjectMember.GroupName = groupObject.GroupName;
                    userGroupObjectMember.Description = groupObject.Description;
                    userGroupObjectMember.IsApprove = UserGroupStatus.Approve;
                    userGroupObjectMember.JoinDate = DateTime.Now;
                    userGroupObjectMember.GroupRole = groupRoleMember;
                    userGroupObjectMember.IsReceiveEmail = true;

                    session.Store(userGroupObjectMember);

                    // them member vao group
                    groupObject.ListUserGroup.Add(userGroupObjectMember);

                    // them user group vao user
                    userLoad = session.Load<UserObject>(user.Id);
                    userLoad.ListUserGroup.Add(userGroupObjectMember);
                    session.Store(userLoad);
                }

                List<TopicObject> listTopicObject = new List<TopicObject>();
                TopicObject topicObject;
                int indexRandomIn50Members;
                int lengthOfListUserObjectMember = listUserObjectMember.Count();
                UserObject userObjectRandomIn50Members;
                
                for (j = 1; j <= 100; j++ )
                {
                    topicObject = new TopicObject();
                    topicObject.Id = Guid.NewGuid().ToString();
                    topicObject.TopicName = "Topic" + j + " " + groupObject.GroupName;
                    topicObject.Content = "Topic content.";

                    // tao ngau nhien user trong 50 thanh vien cua nhom
                    indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                    userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                    topicObject.CreateBy = userObjectRandomIn50Members;
                    topicObject.GroupId = userObjectRandom.Region + "-" + groupObject.Id;
                    topicObject.CreateDate = DateTime.Now;
                    topicObject.LastModified = topicObject.CreateDate;
                    topicObject.NumberOfView = 0;
                    topicObject.isDeleted = false;

                    CommentObject commentObject;
                    List<CommentObject> listCommentObject = new List<CommentObject>();
                    for (k = 1; k <= 100;k++ )
                    {
                        commentObject = new CommentObject();
                        commentObject.Id = Guid.NewGuid().ToString();
                        commentObject.Content = "Comment " + k;
                        commentObject.ParentContent = "Topic content.";
                        // tao user ngau nhien trong 50 thanh vien cua nhom
                        indexRandomIn50Members = random.Next(lengthOfListUserObjectMember);
                        userObjectRandomIn50Members = listUserObjectMember[indexRandomIn50Members];
                        commentObject.CreateBy = userObjectRandomIn50Members;
                        commentObject.CreateDate = DateTime.Now;
                        commentObject.isDeleted = false;
                        listCommentObject.Add(commentObject);
                    }

                    topicObject.ListComment = listCommentObject;
                    topicObject.NumberOfComment = Convert.ToUInt32(listCommentObject.Count);

                    listTopicObject.Add(topicObject);

                    session.Store(topicObject);                    
                    
                }

                // them denormalize topic vao group
                foreach (TopicObject topic in listTopicObject)
                {
                    groupObject.ListTopic.Add(topic);
                }

                // group xong het roi, luu xuong thoi
                session.Store(groupObject);

                count++;
                if (count == 10)
                {
                    session.SaveChanges();
                    session.Dispose();
                    count = 0;
                }   
            }       
        }

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
}
