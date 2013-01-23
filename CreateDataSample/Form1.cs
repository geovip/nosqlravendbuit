using HtmlAgilityPack;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Shard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using UIT.NoSQL.Core.Domain;


namespace CreateDataSample
{
    public partial class Form1 : Form
    {
        public static string STR_DATA_SERVER = "F:\\RavenServers-2230\\Data\\";
        public static string STR_DATA_SERVER_USERS = "F:\\RavenServers-2230\\Data\\Users\\";
        public static string STR_DATA_SERVER_GROUPS = "F:\\RavenServers-2230\\Data\\Groups\\";
        public static string STR_DATA_SERVER_GROUPRSS = "F:\\RavenServers-2230\\Data\\GroupRSS\\";
        public static string STR_DATA_SERVER_TOPICS = "F:\\RavenServers-2230\\Data\\Topics\\";

        DocumentStore documentStore1;
        GroupRoleObject groupRoleManager, groupRoleMember, groupRoleOwner;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            documentStore1 = new DocumentStore { Url = "http://localhost:8080/", DefaultDatabase = "GroupAndLinkRSSDB" };
            documentStore1.Initialize();
            documentStore1.Conventions.MaxNumberOfRequestsPerSession = 100000;
            //IndexCreation.CreateIndexes(typeof(GroupRSSIndex).Assembly, documentStore1);
            //IndexCreation.CreateIndexes(typeof(User_UserGroupIndex).Assembly, documentStore1);
            //IndexCreation.CreateIndexes(typeof(User_ByCustomerOrderInList_Analyzed).Assembly, documentStore);
            //CreateGroupRole();
        }

        private void CreateGroupRole()
        {
            var session = documentStore1.OpenSession();
            //create data sample
            groupRoleManager = new GroupRoleObject();
            groupRoleManager.Id = "7E946ED1-69E6-4B45-8273-FB7AC7367F50";
            groupRoleManager.GroupName = "Manager";
            session.Store(groupRoleManager);

            groupRoleMember = new GroupRoleObject();
            groupRoleMember.Id = "9A17E51B-7EAB-4E80-B3E4-6C3D44DCE3EB";
            groupRoleMember.GroupName = "Member";
            session.Store(groupRoleMember);

            groupRoleOwner = new GroupRoleObject();
            groupRoleOwner.Id = "79C6B725-F787-4FDF-B820-42A21174449D";
            groupRoleOwner.GroupName = "Owner";
            session.Store(groupRoleOwner);

            session.SaveChanges();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var session = documentStore1.OpenSession();
            int i;
            UserObject user;
            for (i = 1; i <= 10000; i++)
            {
                user = new UserObject();
                user.Id = Guid.NewGuid().ToString();
                user.FullName = "user" + i;
                user.UserName = "username" + i;
                user.Password = "1";
                user.Email = user.UserName + "@yahoo.com";
                session.Store(user);
            }
            session.SaveChanges();
            session.Dispose();
            MessageBox.Show("Insert 10.000 users success!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var session1 = documentStore1.OpenSession();
            //var session2 = documentStore2.OpenSession();
            int i;
            GroupObject group;
            UserGroupObject userGroup;
            Random random = new Random();
            int indexRandom;
            UserObject user;
            for (i = 1; i <= 20000; i++)
            {
                // Khoi tao group
                group = new GroupObject();
                group.Id = Guid.NewGuid().ToString();
                group.GroupName = "group name" + i;
                group.Description = "description" + i;
                group.IsPublic = true;
                group.CreateDate = DateTime.Now;

                // lay user ngau nhien
                indexRandom = random.Next(10000) + 1;
                user = session1.Query<UserObject, User_ByCustomerOrderInList_Analyzed>().Search(x => x.UserName, "username" + indexRandom).FirstOrDefault();

                group.CreateBy = user;
                group.NewEvent = new GroupEvent();
                group.NewEvent.Title = "New group";
                group.NewEvent.CreateDate = DateTime.Now;
                group.NewEvent.CreateBy = user.FullName;
                // luu group
                session1.Store(group);

                // khoi tao userGroup
                userGroup = new UserGroupObject();
                userGroup.Id = Guid.NewGuid().ToString();
                userGroup.UserId = user.Id;
                userGroup.GroupId = group.Id;
                userGroup.GroupName = group.GroupName;
                userGroup.Description = group.Description;
                userGroup.IsApprove = UserGroupStatus.Approve;
                userGroup.JoinDate = DateTime.Now;
                userGroup.GroupRole = groupRoleManager;

                group.ListUserGroup.Add(userGroup);
                user.ListUserGroup.Add(userGroup);

                // luu tat ca xuong db
                session1.Store(userGroup);
                session1.Store(group);
                //session1.Store(user);

            }
            session1.SaveChanges();
            session1.Dispose();
            //session2.Dispose();
            MessageBox.Show("Insert 20.000 groups success!");
        }

        public void TestSaveWithQuery()
        {
            var session = documentStore1.OpenSession();
            UserObject user = session.Load<UserObject>("dcbe05c2-9706-43dd-a7ad-66a798ccb24b");
            user.Password = "7";
            session.SaveChanges();
        }

        public class User_ByCustomerOrderInList_Analyzed : AbstractIndexCreationTask<UserObject>
        {
            public User_ByCustomerOrderInList_Analyzed()
            {
                Map = users => from user in users
                               select new { user.UserName };
                Indexes.Add(x => x.UserName, FieldIndexing.Analyzed);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TestSaveWithQuery();
        }

        public int NumberOfUserGroup()
        {
            //RavenQueryStatistics stats;
            int total = 0;
            var session = documentStore1.OpenSession();
            ////string username = "";
            ////UserObject user = new UserObject();
            ////for (int i = 1; i <= 10000; i++)
            ////{
            ////    username = "username" + i.ToString();
            ////    user = session.Query<UserObject>().Where(x => x.UserName == username).First();
            ////    total += user.ListUserGroup.Count();
            ////}
            ////session.Dispose();

            //total = session.Query<UserObject>().Sum(x => x.ListUserGroup.Count());
            //var list = getAll<>(documentStore1);
            //var allUsers = new List<UserObject>();
            int start = 0;
            while (true)
            {
                var current = session.Query<UserObject>().Take(1024).Skip(start).ToList();
                total += current.Sum(x=>x.ListUserGroup.Count());
                if (current.Count == 0)
                    break;

                start += current.Count;
                //allUsers.AddRange(current);

            }

            //total = allUsers.Count();
            return total;
        }
        public int NumberOfUserGroupUsingMapReduce()
        {
            int total = 0;
            var session = documentStore1.OpenSession();
            total = session.Query<User_UserGroupIndex.ReduceResult, User_UserGroupIndex>().Count();
            //total = session.Query<UserObject, User_ByCustomerOrderInList_Analyzed>().Sum(x => x.ListUserGroup.Count());
            return total;
        }

        public class User_UserGroupIndex : AbstractIndexCreationTask<UserObject, User_UserGroupIndex.ReduceResult>
        {
            public class ReduceResult
            {
                //public string UserId { get; set; }
                public string UserGroupId { get; set; } 
            }

            public User_UserGroupIndex()
            {
                Map = users => from u in users
                               from ug in u.ListUserGroup
                               select new
                               {
                                   UserGroupId = ug.Id
                               };
            }
        }

        //--------- 128 1024
        public static List<T> getAll<T>(DocumentStore docDB)
        {
            return getAllFrom(0, new List<T>(), docDB);
        }
        public static List<T> getAllFrom<T>(int startFrom, List<T> list, DocumentStore docDB)
        {
            var allUsers = list;

            using (var session = docDB.OpenSession())
            {
                int queryCount = 0;
                int start = startFrom;
                while (true)
                {
                    var current = session.Query<T>().Take(1024).Skip(start).ToList();
                    queryCount += 1;
                    if (current.Count == 0)
                        break;

                    start += current.Count;
                    allUsers.AddRange(current);

                    if (queryCount >= 30)
                    {
                        return getAllFrom(start, allUsers, docDB);
                    }
                }
            }
            return allUsers;
        }
        //---------

        public DataTable GetListFullName(int length)
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
            for (int i = 0; i < length; i++ )
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

        private void WriteTwoListOfUsersInfoXMLToFile()
        {
            DataTable dt = GetListFullName(15000);

            List<string> ServerRegion = new List<string> { "Asia", "MiddleEast", "America" };

            // tao cung luc 2 danh sach: doc1 khong co chua Region trong id
            XDocument doc1 = new XDocument();
            XDocument doc2 = new XDocument();
            XElement root1 = new XElement("ListUserObject");
            XElement root2 = new XElement("ListUserObject");
            XElement user1,user2;
            List<XElement> listUser1 = new List<XElement>();
            List<XElement> listUser2 = new List<XElement>();
            int i, j;
            int index = 0;
            string id;
            for (i = 0; i < ServerRegion.Count; i++) // 3 servers
            {
                for (j = 0; j < 5000; j++) // moi server 5000 users
                {
                    id = Guid.NewGuid().ToString();
                    user1 = new XElement("UserObject",
                                new XElement("Id", id),
                                new XElement("FullName", dt.Rows[index]["FullName"].ToString()),
                                new XElement("UserName", "username" + index + 1),
                                new XElement("Password", "c4ca4238a0b923820dcc509a6f75849b"),
                                new XElement("Email", "username" + index + "@gmail.com"),
                                new XElement("Region", ServerRegion[i])
                                );
                    listUser1.Add(user1);
                    user2 = new XElement("UserObject",
                                new XElement("Id", ServerRegion[i] + "-" +id),
                                new XElement("FullName", dt.Rows[index]["FullName"].ToString()),
                                new XElement("UserName", "username" + index + 1),
                                new XElement("Password", "c4ca4238a0b923820dcc509a6f75849b"),
                                new XElement("Email", "username" + index + "@gmail.com"),
                                new XElement("Region", ServerRegion[i])
                                );
                    listUser2.Add(user2);
                    index++;
                }
            }
            root1.Add(listUser1);
            doc1.Add(root1);
            string fileName1 = "ListUsers.xml";
            doc1.Save(STR_DATA_SERVER_USERS + fileName1);
            root2.Add(listUser2);
            doc2.Add(root2);
            string fileName2 = "ListUsersReference.xml";
            doc2.Save(STR_DATA_SERVER_USERS + fileName2);
            MessageBox.Show("Lưu thành công!");
        }

        private void WriteUsersInfoXMLToFile()
        {
            DataTable dt = GetListFullName(15000);

            List<string> ServerRegion = new List<string>{"Asia", "MiddleEast", "America"};
            XDocument doc = new XDocument();
            XElement root = new XElement("ListUserObject");
            XElement user;
            List<XElement> listUser = new List<XElement>();
            int i,j;
            int index = 0;
            for (i = 0; i < ServerRegion.Count; i++) // 3 servers
            {
                for (j = 0; j < 5000; j++) // moi server 5000 users
                {
                    user = new XElement("UserObject",
                                new XElement("FullName",dt.Rows[index++]["FullName"].ToString()),
                                new XElement("UserName", "username" + index),
                                new XElement("Email", "username"+index+"@gmail.com"),
                                new XElement("Region",ServerRegion[i])
                                );
                    listUser.Add(user);
                }
            }
            root.Add(listUser);
            doc.Add(root);
            string fileName = "ListUsers.xml"; 
            doc.Save(STR_DATA_SERVER_USERS + fileName);
            MessageBox.Show("Lưu thành công!");
        }

        private void TestWriteXMLToFile()
        {
            DataTable dt = GetListFullName(10000);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            XmlTextWriter xmlwriter = new XmlTextWriter(Application.StartupPath + "/Test.xml", Encoding.UTF8);
            xmlwriter.Formatting = Formatting.Indented;
            xmlwriter.WriteStartDocument();
            xmlwriter.WriteComment("Ghi dữ liệu họ tên ra XML");
            xmlwriter.WriteStartElement("ListUserObject");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                xmlwriter.WriteStartElement("UserObject");
                xmlwriter.WriteElementString("FullName", dt.Rows[i]["FullName"].ToString());
                xmlwriter.WriteEndElement();
            }
            xmlwriter.WriteEndElement();
            xmlwriter.WriteEndDocument();
            xmlwriter.Flush();
            xmlwriter.Close();

            sw.Stop();
            MessageBox.Show(sw.Elapsed.ToString());
        }


        private void ReadUsersInfoXMLFile()
        {
            try
            {
                //XmlTextReader reader = new XmlTextReader(Application.StartupPath + "/FullNameOf10000Users.xml");
                string xmlFilePath = STR_DATA_SERVER_USERS + "ListUsers.xml";
                XElement xmlUser = XElement.Load(xmlFilePath);
                List<User> listUser = (from u in xmlUser.Elements("UserObject")
                                       select new User
                                       {
                                           FullName = u.Element("FullName").Value,
                                           UserName = u.Element("UserName").Value,
                                           Email = u.Element("Email").Value,
                                           Region = u.Element("Region").Value
                                       }
                                       ).ToList();
                dataGridView1.DataSource = listUser;
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            WriteUsersInfoXMLToFile();
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReadUsersInfoXMLFile();
        }

        // thuc hien tren tab 3
        
        public class GroupRSSIndex : AbstractIndexCreationTask<GroupRSS>
        {
            public GroupRSSIndex()
            {
                Map = groupRSSs => from groupRSS in groupRSSs
                                   select new
                                   {
                                       GroupName = groupRSS.GroupName,
                                       LinkRSS = groupRSS.LinkRSS
                                   };
            }
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            var session = documentStore1.OpenSession("GroupAndLinkRSSDB");
            session.Store(new GroupRSS { GroupName = txtGroupName.Text.Trim(), LinkRSS=txtLinkRSS.Text.Trim()});
            session.SaveChanges();

            List<GroupRSS> listGroupRSS = session.Query<GroupRSS>("GroupRSSIndex").ToList();
            dGVListGroupRSS.DataSource = listGroupRSS;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ShowListGroupRSS();
            List<GroupRSS> listGroupRSS = ReadGroupRSSInfoXMLFile();
            dGVListGroupRSS.DataSource = listGroupRSS;
        }

        private void ShowListGroupRSS()
        {
            var session = documentStore1.OpenSession("GroupAndLinkRSSDB");
            List<GroupRSS> listGroupRSS = session.Query<GroupRSS>("GroupRSSIndex").ToList();
            dGVListGroupRSS.DataSource = listGroupRSS;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult dlgResult = dlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                txtPath.Text = dlg.FileName;
            }
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(txtPath.Text))
            {
                string excelContentType = "application/vnd.ms-excel";
                string excel2013ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                try
                {
                    //System.IO.FileInfo fileInfo = new FileInfo(txtPath.Text);
                    
                    string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;""", txtPath.Text);
                    string query = String.Format("select * from [{0}$]", "Sheet1");
                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, connectionString);
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);

                    DataTable dt = dataSet.Tables[0];
                    // luu du lieu xuong RavenDB
                    //var session = documentStore1.OpenSession("GroupAndLinkRSSDB");
                    //int i;
                    //for (i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    var groupRSS = new GroupRSS { GroupName = dt.Rows[i]["GroupName"].ToString(), LinkRSS = dt.Rows[i]["LinkRSS"].ToString() };
                    //    session.Store(groupRSS);
                    //}
                    //session.SaveChanges();

                    // doc du lieu len tu RavenDB
                    //ShowListGroupRSS();

                    // luu du lieu xuong XML
                    WriteGroupRSSInfoXMLToFile(dt);
                    // doc du lieu len tu XML
                    ReadGroupRSSInfoXMLFile();
                }
                catch
                {
                    MessageBox.Show("Error write!");
                }
                
            }
            else
            {
                MessageBox.Show("No File is Selected");
            }
        }

        
        // doc du lieu tu danh muc LinkRSS
        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "title";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "link";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "description";
            myDataTable.Columns.Add(myDataColumn);
            return myDataTable;
        }

        private void AddDataToTable(string Title,string link,string Descriptions,DataTable myTable)
        {
            DataRow row;
            row = myTable.NewRow();
            row["title"] = Title;
            row["link"] = link;
            row["description"] = Descriptions;
            myTable.Rows.Add(row);
        }

        public DataTable BindRSSItem(string rssURL)
        {
            DataTable myDataTable = CreateDataTable();
            try
            {
                WebRequest myRequest = WebRequest.Create(rssURL);
                WebResponse myResponse = myRequest.GetResponse();

                Stream rssStream = myResponse.GetResponseStream();
                XmlDocument rssDoc = new XmlDocument();
                rssDoc.Load(rssStream);
                XmlNodeList rssItems = rssDoc.SelectNodes("rss/channel/item");
                string title = "";
                string link = "";
                string description = "";
                for (int i = 0; i < rssItems.Count; i++)
                {
                    XmlNode rssDetail;
                    rssDetail = rssItems.Item(i).SelectSingleNode("title");
                    if (rssDetail != null)
                    {
                        title = rssDetail.InnerText;
                    }
                    else
                    {
                        title = "";
                    }
                    rssDetail = rssItems.Item(i).SelectSingleNode("link");
                    if (rssDetail != null)
                    {
                        link = rssDetail.InnerText;
                    }
                    else
                    {
                        link = "";
                    }

                    rssDetail = rssItems.Item(i).SelectSingleNode("description");
                    if (rssDetail != null)
                    {
                        description = rssDetail.InnerText;
                    }
                    else
                    {
                        description = "";
                    }
                    AddDataToTable(title, link, description, myDataTable);
                }
            }
            catch { }
            return myDataTable;
        }

        public void CreateFolder(string strPath)
        {
            try
            {
                if (Directory.Exists(strPath) == false)
                {
                    Directory.CreateDirectory(strPath);
                }
            }
            catch { }
        }

        private void WriteGroupRSSInfoXMLToFile(DataTable dt)
        {
            string strPath = STR_DATA_SERVER_GROUPRSS;
            string fileName = "GroupRSS.xml";

            XDocument doc = new XDocument();
            XElement root = new XElement("ListGroupRSS");
            int i, lenght = dt.Rows.Count;
            List<XElement> listGroupRSS = new List<XElement>();
            XElement groupRSS;
            for (i = 0; i < lenght; i++)
            {
                groupRSS = new XElement("GroupRSS",
                                new XElement("GroupName", dt.Rows[i]["GroupName"].ToString()),
                                new XElement("LinkRSS", dt.Rows[i]["LinkRSS"].ToString())
                                );
                listGroupRSS.Add(groupRSS);
            }
            root.Add(listGroupRSS);
            doc.Add(root);
            doc.Save(strPath + fileName);
            MessageBox.Show("Ghi thành công!");
        }

        private List<GroupRSS> ReadGroupRSSInfoXMLFile()
        {
            string xmlFilePath = STR_DATA_SERVER_GROUPRSS + "GroupRSS.xml" ;
            if (File.Exists(xmlFilePath))
            {
                XElement xmlGroupRSS = XElement.Load(xmlFilePath);
                List<GroupRSS> listGroupRSS = (from u in xmlGroupRSS.Elements("GroupRSS")
                                                select new GroupRSS
                                                {
                                                    GroupName = u.Element("GroupName").Value,
                                                    LinkRSS = u.Element("LinkRSS").Value
                                                }
                                                ).ToList();
                return listGroupRSS;
            }
            else {
                MessageBox.Show("Sai duong dan");
                return null;
            }
        }

        private void WriteTopicsInfoXMLToFile()
        {
            DataTable dt = new DataTable() ;
            List<GroupRSS> listGroupRSS = ReadGroupRSSInfoXMLFile();
            string strPath = STR_DATA_SERVER_TOPICS + Guid.NewGuid().ToString() + "\\";
            string strPathTemp;
            Directory.CreateDirectory(strPath); // moi lan luu du lieu moi la tao thu muc chua topics theo group
            foreach (var groupRSS in listGroupRSS)
            {
                strPathTemp = strPath + groupRSS.GroupName + "\\";
                if (!Directory.Exists(strPathTemp))
                {
                    Directory.CreateDirectory(strPathTemp);
                }

                // ghi du lieu ra file
                dt = BindRSSItem(groupRSS.LinkRSS);

                XDocument doc = new XDocument();
                XElement xTree = new XElement("ListTopicObject");

                List<XElement> listTopics = new List<XElement>();
                int i, length = dt.Rows.Count;
                string linkToWeb,content;
                List<string> listCommentsForTopic = new List<string>();
                XElement topic;
                List<XElement> listComment = new List<XElement>();
                for (i = 0; i < length; i++)
                {
                    linkToWeb = dt.Rows[i]["link"].ToString();
                    content = GetWebContent(linkToWeb);
                    listCommentsForTopic = GetCommentsForTopic(content);
                    listComment.Clear();
                    foreach (string cm in listCommentsForTopic)
                    {
                        listComment.Add(new XElement("Comment",cm));
                    }
                    topic = new XElement("TopicObject",
                                new XElement("TopicName", dt.Rows[i]["title"].ToString()),
                                new XElement("Content", dt.Rows[i]["description"].ToString()), 
                                new XElement("Comments",listComment)
                                );
                    listTopics.Add(topic);
                }
                xTree.Add(listTopics);
                doc.Add(xTree);

                string fileName = "ListTopics.xml";
                doc.Save(strPathTemp + fileName);
            }
            MessageBox.Show("Lưu thành công!");
        }

        public List<string> GetCommentsForTopic(string content)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            List<string> pTags = new List<string>();
            foreach (HtmlNode p in doc.DocumentNode.Descendants("p").Where(x => x.InnerText.Length >= 60))
            {
                pTags.Add(p.InnerText);
            }
            return pTags;
        }

        
        private void ReadTopicsInfoXMLFile()
        {
            try
            {
                //XmlTextReader reader = new XmlTextReader(Application.StartupPath + "/FullNameOf10000Users.xml");
                string xmlFilePath = "F:\\RavenServers-2230\\Data" + "/ListGroupFromRSS.xml";
                XElement xmlUser = XElement.Load(xmlFilePath);
                List<User> listUser = (from u in xmlUser.Elements("UserObject")
                                       select new User
                                       {
                                           FullName = u.Element("FullName").Value
                                       }
                                       ).ToList();
                dataGridView1.DataSource = listUser;
            }
            catch { }
        }

        public void TestPath()
        {
            string path = Path.GetFullPath("F:\\RavenServers-2230\\Data\\GroupRSS") + "\\Spam";

            if (!Directory.Exists(path))
            {
                
                Directory.CreateDirectory(path);
            }
            else
            {
                MessageBox.Show("OK!");
            }
        }

        private void btnTestPath_Click(object sender, EventArgs e)
        {
            //TestPath();
            TestXDocument();
        }

        public void TestXDocument()
        {
            DataTable dt = GetListFullName(10000);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            XDocument doc = new XDocument();
            XElement root = new XElement("ListUserObject");
            int i;
            for (i = 0; i < 10000; i++)
            {
                XElement el = new XElement("UserObject",
                                  new XElement("FullName", dt.Rows[i]["FullName"].ToString())
                              );
                root.Add(el);
            }

            doc.Add(root);
            doc.Save("List.xml");


            //XDocument doc = XDocument.Load("List.xml");
            //List<XElement> li = new List<XElement>();

            //int i;
            //for (i = 0; i < 10000; i++)
            //{
            //    XElement el = new XElement("UserObject",
            //                      new XElement("FullName", dt.Rows[i]["FullName"].ToString())
            //                  );
            //    li.Add(el);

            //}

            //doc.Root.AddFirst(li);
            //doc.Save("List.xml");
           
            sw.Stop();
            MessageBox.Show(sw.Elapsed.ToString());
           
            //TestWriteXMLToFile();
        }
        
        private void btnWriteFromRSS_Click(object sender, EventArgs e)
        {
            WriteTopicsInfoXMLToFile();
        }

        // Hàm tải nội dung HTLM của một địa chỉ cho trước
        public string GetWebContent(string strLink)
        {
            string strContent = "";
            try
            {
                WebRequest objWebRequest = WebRequest.Create(strLink);
                objWebRequest.Credentials = CredentialCache.DefaultCredentials;
                WebResponse objWebResponse = objWebRequest.GetResponse();
                Stream receiveStream = objWebResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                strContent = readStream.ReadToEnd();
                objWebResponse.Close();
                readStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return strContent;
        }

        // Lay tieu de
        public string GetTitle(string Content)
        {
            string pattern = "<h1 class=Title>[^<]+";
            Regex Title = new Regex(pattern);
            Match m = Title.Match(Content);
            if (m.Success)
                return m.Value.Substring(16, m.Value.Length - 16);
            return "";
        }

        private void btnReadContentFromRSS_Click(object sender, EventArgs e)
        {
            //string strLink = "http://www.24h.com.vn/tin-tuc-trong-ngay/100-nguoi-giau-nhat-du-xoa-doi-ngheo-ca-tg-c46a515625.html";
            string strLink = "http://vietnamnet.vn/vn/xa-hoi/106284/o-to-mat-lai-roi-vuc-20m-tren-duong-len-da-lat.html";
            
            string content = GetWebContent(strLink);
                        
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //doc.Load("file.htm", Encoding.GetEncoding("utf-8"));
            doc.LoadHtml(content);
            List<string> pTags = new List<string>();
            foreach(HtmlNode p in doc.DocumentNode.Descendants("p").Where(x => x.InnerText.Length >= 100))
            {
                 pTags.Add(p.InnerText);
            }
            //lbContent.Text = String.Join(String.Empty,pTags);
            richTextBox1.Text = String.Join(String.Empty, pTags);
        }

        private void btnWriteTwoListUsertoXML_Click(object sender, EventArgs e)
        {
            WriteTwoListOfUsersInfoXMLToFile();
        }


        
    }
}

