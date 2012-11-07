using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Service;
using UIT.NoSQL.Core.Domain;

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

        public ActionResult Detail(string id)
        {
            var topic = topicService.Load(id);
            return View(topic);
        }

        //
        // GET: /Topic/Create

        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
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

                topic.Id = Guid.NewGuid().ToString();
                topic.CreateDate = DateTime.Now;
                topic.LastModified = DateTime.Now;
                topic.NumberOfView = 0;
                topic.NumberOfComment = 0;
                topic.CreateBy = user;

                topicService.Save(topic);

                //string groupId = Session["GroupId"].ToString();
                string groupId = TempData["GroupId"].ToString();

                var group = groupService.Load(groupId);
                group.ListTopic.Add(topic);
                groupService.Save(group);

                return RedirectToAction("Index");
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

        //
        // GET: /Topic/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Topic/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult GetAll()
        {
            return Json(topicService.GetAll(),JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetById(string id)
        {
            return Json(topicService.Load(id),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddComment(string topicId, string content)
        {
            var topic = topicService.Load(topicId);
            if (topic.ListComment == null)
            {
                topic.ListComment = new List<CommentObject>();
            }
            CommentObject comment = new CommentObject();
            comment.Content = content;
            comment.ParentConten = "parentContent";
            comment.Id = Guid.NewGuid().ToString();
            comment.CreateBy = (UserObject)Session["user"];
            comment.CreateDate = DateTime.Now;
            comment.isDeleted = false;
            topic.ListComment.Add(comment);
            topicService.Save(topic);
            return Json(comment, JsonRequestBehavior.AllowGet);
        }
    }
}
