using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Service;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Web.Filters;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using UIT.NoSQL.Web.Factory;

namespace UIT.NoSQL.Web.Controllers
{
    public class TopicController : Controller
    {
        private ITopicService topicService;
        private IGroupService groupService;

        public TopicController(ITopicService topicService, IGroupService groupService) 
        {
            this.topicService = topicService;
            this.groupService = groupService;
        }

        //
        // GET: /Topic/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Topic/Details/5
        [MemberFilter(TypeID = TypeIDEnum.TopicID)]
        public ActionResult Detail(string id)
        {
            //ViewBag.Role = TempData["Role"];
            var topic = topicService.Load(id);
            if (topic != null)
            {
                var group = groupService.Load(topic.GroupId);
                // Get Current Role
                string roleStr = "unlogin";
                UserObject userSession = (UserObject)Session["user"];
                if (userSession != null)
                {
                    // lấy lại thông tin mới cập nhật của user hiện tại
                    string userId = userSession.Id;
                    IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
                    Session["user"] = userService.Load(userId);
                    userSession = (UserObject)Session["user"];
                    ///////////////////////////////////////////////////

                    var userGroup = userSession.ListUserGroup.Find(u => u.GroupId == topic.GroupId);
                    if (userGroup != null)
                    {
                        if (userGroup.IsApprove == UserGroupStatus.Approve) // la member, manager, owner
                        {
                            roleStr = userGroup.GroupRole.GroupName;
                            //update lai so luong bai dang moi = 0
                            var userTopic = topic.ListUserTopic.Find(ut => ut.UserId == userSession.Id);
                            if (userTopic != null)
                            {
                                userTopic.NumberOfNewPosts = 0;
                            }
                            var topicInGroup = group.ListTopic.Find(t => t.Id.Equals(topic.Id));
                            if (topicInGroup != null)
                            {
                                var userTopicInGroup = topicInGroup.ListUserTopic.Find(ut => ut.UserId.Equals(userSession.Id));
                                if (userTopicInGroup != null)
                                {
                                    userTopicInGroup.NumberOfNewPosts = 0;
                                }
                            }
                        }
                        else if (userGroup.IsApprove == UserGroupStatus.JoinRequest)
                            roleStr = "WaitingForAccepting";
                        else
                            roleStr = "JoinGroup";
                        ViewBag.UserId = userSession.Id;
                    }
                    else
                        roleStr = "JoinGroup";
                }
                ViewBag.Role = roleStr;
                ViewBag.HostData = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

                topic.NumberOfView += 1;
                topicService.Save(topic);

                group.ListTopic.Find(t => t.Id.Equals(topic.Id)).NumberOfView += 1;
                groupService.Save(group);

                return View(topic);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Group", new { id = topic.GroupId });
            }
        }

        //
        // GET: /Topic/Create
        [MemberFilter(TypeID = TypeIDEnum.GroupID)]
        public ActionResult Create(string id)
        {
            TempData["GroupId"] = id;
            GroupObject group = groupService.Load(id);
            ViewBag.GroupName = group.GroupName;
            ViewBag.FullName = group.CreateBy.FullName;
            return View();
        }

        //
        // POST: /Topic/Create

        [HttpPost]
        [ValidateInput(false)]
        public string Create(string GroupId, string TopicName, string Content, List<FileAttach> listFilesAttach)
        {
            try
            {
                var user = (UserObject)Session["user"];
                TopicObject topic = new TopicObject();
                topic.Id = Guid.NewGuid().ToString();
                topic.TopicName = TopicName;
                topic.Content = Content;
                topic.CreateDate = DateTime.Now;
                topic.LastModified = DateTime.Now;
                topic.NumberOfView = 0;
                topic.NumberOfComment = 0;
                topic.CreateBy = user;
                topic.GroupId = GroupId;

                if (listFilesAttach != null)
                {
                    List<FileAttach> listTemp = new List<FileAttach>();
                    foreach (var fileAttach in listFilesAttach)
                    {
                        var file = new FileAttach();
                        file.Id = Guid.NewGuid().ToString();
                        file.DisplayName = fileAttach.DisplayName;
                        file.Size = fileAttach.Size;
                        var realName = fileAttach.RealName.Replace("/Files/", "").Replace("/", "-");
                        file.RealName = realName;
                        listTemp.Add(file);
                    }
                    topic.ListFilesAttach = listTemp;
                }

                var group = groupService.Load(topic.GroupId);
                List<UserTopic> listUserTopicTemp = new List<UserTopic>();
                foreach (var userGroup in group.ListUserGroup)
                {
                    if (userGroup.IsApprove == UserGroupStatus.Approve)
                    {
                        UserTopic userTopic = new UserTopic();
                        userTopic.UserId = userGroup.UserId;
                        if (userGroup.UserId != user.Id)
                            userTopic.NumberOfNewPosts += 1;
                        listUserTopicTemp.Add(userTopic);
                    }
                }
                topic.ListUserTopic = listUserTopicTemp;

                topicService.Save(topic);
            
                group.ListTopic.Add(topic);
                group.NewEvent.Title = topic.TopicName;
                group.NewEvent.CreateDate = topic.CreateDate;
                group.NewEvent.CreateBy = user.FullName; ;
                group.NewEvent.UserId = user.Id;
                groupService.Save(group);

                //send email
                string subject = user.FullName + " created " + topic.TopicName;
                string body = topic.Content;
                groupService.SendEmail(group.ListUserGroup, subject, body);

                return "success";
            }
            catch
            {
                return "error";
            }
        }
        //public ActionResult Create(string GroupId,string TopicName, string Content, List<FileAttach> listFilesAttach)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here
        //        var user = (UserObject)Session["user"];
        //        //string groupId = TempData["GroupId"].ToString();
        //        TopicObject topic = new TopicObject();
        //        topic.Id = Guid.NewGuid().ToString();
        //        topic.TopicName = TopicName;
        //        topic.Content = Content;
        //        topic.CreateDate = DateTime.Now;
        //        topic.LastModified = DateTime.Now;
        //        topic.NumberOfView = 0;
        //        topic.NumberOfComment = 0;
        //        topic.CreateBy = user;
        //        topic.GroupId = GroupId;

        //        List<FileAttach> listTemp = new List<FileAttach>();
        //        foreach (var fileAttach in listFilesAttach)
        //        {
        //            var file = new FileAttach();
        //            file.Id = Guid.NewGuid().ToString();
        //            file.DisplayName = fileAttach.DisplayName;
        //            file.Size = fileAttach.Size;
        //            var realName = fileAttach.RealName.Replace("/Files/", "").Replace("/", "-");
        //            //url = Path.Combine("/" + ConfigurationManager.AppSettings["DIR_FILE_UPLOADS"] + "/", url);
        //            file.RealName = realName;
        //            listTemp.Add(file);
        //        }
        //        topic.ListFilesAttach = listTemp;
      
        //        topicService.Save(topic);

        //        var group = groupService.Load(topic.GroupId);
        //        group.ListTopic.Add(topic);
        //        group.NewEvent.Title = topic.TopicName;
        //        group.NewEvent.CreateDate = topic.CreateDate;
        //        group.NewEvent.CreateBy = user.FullName; ;
        //        groupService.Save(group);

        //        //send email
        //        string subject = user.FullName + " created " + topic.TopicName;
        //        string body = topic.Content;
        //        groupService.SendEmail(group.ListUserGroup, subject, body);

        //        return RedirectToAction("Detail", "Group", new { id = topic.GroupId });
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //
        // GET: /Topic/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Topic/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult DeleteTopic(string topicIds)
        {
            string[] ids = topicIds.Split(',');
            foreach (string id in ids)
            {
                var topic = topicService.Load(id);
                if (topic != null)
                {
                    topic.isDeleted = true;
                    topicService.Save(topic);

                    var group = groupService.Load(topic.GroupId);
                    group.ListTopic.Find(t => t.Id.Equals(topic.Id)).isDeleted = true;
                    groupService.Save(group);
                }
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAll()
        {
            return Json(topicService.GetAll(),JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetById(string id)
        {
            return Json(topicService.Load(id),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginFilter]
        [MemberFilter(TypeID = TypeIDEnum.TopicID)]
        public JsonResult AddComment(string Id, string content, string parentContent, List<FileAttach> listFilesAttach)
        {
            var topic = topicService.Load(Id);
            if (topic.ListComment == null)
            {
                topic.ListComment = new List<CommentObject>();
            }
            CommentObject comment = new CommentObject();
            
            var user = (UserObject)Session["user"];
            
            comment.Content = content;
            comment.ParentContent = parentContent;
            comment.Id = Guid.NewGuid().ToString();
            
            comment.CreateBy = user;
            
            comment.CreateDate = DateTime.Now;
            comment.isDeleted = false;

            if (listFilesAttach!=null)
            {
                List<FileAttach> listTemp = new List<FileAttach>();
                foreach (var fileAttach in listFilesAttach)
                {
                    var file = new FileAttach();
                    file.Id = Guid.NewGuid().ToString();
                    file.DisplayName = fileAttach.DisplayName;
                    file.Size = fileAttach.Size;
                    var realName = fileAttach.RealName.Replace("/Files/", "").Replace("/", "-");
                    file.RealName = realName;
                    listTemp.Add(file);
                }
                comment.ListFilesAttach = listTemp;
            }
            
            topic.ListComment.Add(comment);
            topic.NumberOfComment += 1;
            topic.LastModified = DateTime.Now;

            // cap nhap lai danh sach user voi so luong bai viet moi
            //for (int i = 0; i< topic.ListUserTopic.Count; i++ )
            //{
            //    if (topic.ListUserTopic[i].UserId != user.Id) 
            //        topic.ListUserTopic[i].NumberOfNewPosts += 1;
            //}

            var group = groupService.Load(topic.GroupId);
            List<UserTopic> listUserTopicTemp = new List<UserTopic>();
            foreach (var userGroup in group.ListUserGroup)
            {
                if (userGroup.IsApprove == UserGroupStatus.Approve)
                {
                    UserTopic userTopic = new UserTopic();
                    userTopic.UserId = userGroup.UserId;
                    if (userGroup.UserId != user.Id)
                        userTopic.NumberOfNewPosts += 1;
                    listUserTopicTemp.Add(userTopic);
                }
            }
            foreach (var userTopic in listUserTopicTemp)
            {
                UserTopic temp = topic.ListUserTopic.Find(t => t.UserId.Equals(userTopic.UserId));
                if (temp != null)
                {
                    userTopic.NumberOfNewPosts += temp.NumberOfNewPosts;
                }
            }
            topic.ListUserTopic = listUserTopicTemp;

            topicService.Save(topic);
            
            group.ListTopic.Find(t => t.Id.Equals(topic.Id)).NumberOfComment += 1;
            group.ListTopic.Find(t => t.Id.Equals(topic.Id)).LastModified = DateTime.Now;
            group.ListTopic.Find(t=> t.Id.Equals(topic.Id)).ListUserTopic = topic.ListUserTopic;

            group.NewEvent.Title = "RE: " + topic.TopicName;
            group.NewEvent.CreateDate = comment.CreateDate;
            group.NewEvent.CreateBy = user.FullName;
            group.NewEvent.UserId = user.Id;

            groupService.Save(group);
            
            //send email
            string subject = user.FullName + " replied topic: " + topic.TopicName;
            string body = comment.Content;
            groupService.SendEmail(group.ListUserGroup, subject, body);

            return Json(comment, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LoginFilter]
        public JsonResult DeleteComment(string topicId, string commentId)
        {
            var topic = topicService.Load(topicId);
            topic.ListComment.Find(c => c.Id == commentId).isDeleted = true;
            topicService.Save(topic);

            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}
