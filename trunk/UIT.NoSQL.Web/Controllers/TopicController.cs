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
            var topic = topicService.Load(id);
            if (topic != null)
            {
                topic.NumberOfView += 1;
                topicService.Save(topic);

                var group = groupService.Load(topic.GroupId);
                group.ListTopic.Find(t => t.Id.Equals(topic.Id)).NumberOfView += 1;
                groupService.Save(group);

                // kiem tra quyen cua user
                UserObject user = (UserObject)(Session["user"]);
                string role = user.ListUserGroup.Find(u => u.GroupId == topic.GroupId).GroupRole.GroupName;

                // dua du lieu ra View gom UserId de kiem tra co cho xoa bai dang hay khong
                if (role == "Manager" || role == "Owner" )
                    ViewBag.IsMember = false;
                else
                    ViewBag.IsMember = true;
                ViewBag.UserId = user.Id;
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
        public ActionResult Create(TopicObject topic)
        {
            try
            {
                // TODO: Add insert logic here
                var user = (UserObject)Session["user"];
                //string groupId = TempData["GroupId"].ToString();

                topic.Id = Guid.NewGuid().ToString();
                topic.CreateDate = DateTime.Now;
                topic.LastModified = DateTime.Now;
                topic.NumberOfView = 0;
                topic.NumberOfComment = 0;
                topic.CreateBy = user;
                //topic.GroupId = groupId;
                
                topicService.Save(topic);


                var group = groupService.Load(topic.GroupId);
                group.ListTopic.Add(topic);
                group.NewEvent.Title = topic.TopicName;
                group.NewEvent.CreateDate = topic.CreateDate;
                group.NewEvent.CreateBy = user.FullName; ;
                groupService.Save(group);

                return RedirectToAction("Detail", "Group", new { id = topic.GroupId });
            }
            catch
            {
                return View();
            }
        }

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
            return Json("OK", JsonRequestBehavior.AllowGet);
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
        public JsonResult AddComment(string Id, string content, string parentContent)
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
            topic.ListComment.Add(comment);
            topic.NumberOfComment += 1;
            topic.LastModified = DateTime.Now;
            topicService.Save(topic);

            
            var group = groupService.Load(topic.GroupId);
            

            group.ListTopic.Find(t => t.Id.Equals(topic.Id)).NumberOfComment += 1;
            group.ListTopic.Find(t => t.Id.Equals(topic.Id)).LastModified = DateTime.Now;

            group.NewEvent.Title = "RE: " + topic.TopicName;
            group.NewEvent.CreateDate = comment.CreateDate;
            group.NewEvent.CreateBy = user.FullName;

            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();
            // Begin timing
            stopwatch.Start();
            groupService.Save(group);
            // Stop timing
            stopwatch.Stop();
            
            return Json(comment, JsonRequestBehavior.AllowGet);
            //return Json(stopwatch.Elapsed.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LoginFilter]
        public JsonResult DeleteComment(string topicId, string commentId)
        {
            var topic = topicService.Load(topicId);
            topic.ListComment.Find(c => c.Id == commentId).isDeleted = true;
            topicService.Save(topic);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}
