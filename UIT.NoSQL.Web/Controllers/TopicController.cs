using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Service;

namespace UIT.NoSQL.Web.Controllers
{
    public class TopicController : Controller
    {
        private ITopicService topicService;

        public TopicController(ITopicService topicService) 
        {
            this.topicService = topicService;
        }

        //
        // GET: /Topic/

        public ActionResult Index()
        {
            //var topics = topicService.GetAll();
            //return View(topics);
            return View();
        }

    }
}
