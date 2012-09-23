using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Service;
using UIT.NoSQL.Core.IService;


namespace UIT.NoSQL.Web.Controllers
{
    public class HomeController : Controller
    {
        private ITopicService topicService;

        public HomeController(ITopicService topicService)
        {
            this.topicService = topicService;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            
            TopicObject topic = new TopicObject();
            topic.TopicID = "T02";
            topic.Title = "new title";
            topic.Content = "new content";
            topic.CreateDate = DateTime.Now;

            topicService.Save(topic);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
    }
}
