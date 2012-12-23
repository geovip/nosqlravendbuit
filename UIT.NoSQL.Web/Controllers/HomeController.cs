using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Service;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Web.Factory;

namespace UIT.NoSQL.Web.Controllers
{
    public class HomeController : Controller
    {
        private IUserService userService;
        private ITopicService topicService;

        public HomeController(IUserService userService, ITopicService topicService)
        {
            this.userService = userService;
            this.topicService = topicService;
        }

        public ActionResult Index()
        {
            var user = (UserObject)Session["user"];
            if (user == null || user.ListUserGroup.Count <= 0)
            {
                TempData["IsNew"] = "True";
                return View();
            }
            else
            {
                TempData["IsNew"] = "False";
                bool isEnd = user.ListUserGroup.Count <= 10;
                IGroupService groupService = MvcUnityContainer.Container.Resolve(typeof(IGroupService), "") as IGroupService;
                string[] arrId;
                if (isEnd)
                {
                    arrId = new string[user.ListUserGroup.Count];
                }
                else
                {
                    arrId = new string[10];
                }

                for (int i = 0; i < arrId.Length; i++)
                {
                    arrId[i] = user.ListUserGroup[i].GroupId;
                }

                List<GroupObject> listGroup = groupService.LoadList(arrId);
                TempData["paging"] = user.ListUserGroup.Count / 10 + +((user.ListUserGroup.Count % 10) != 0 ? 1 : 0); ;
                return View(listGroup);
            }
        }

        [HttpPost]
        public ActionResult IndexMore(int page)
        {
            IGroupService groupService = MvcUnityContainer.Container.Resolve(typeof(IGroupService), "") as IGroupService;
            var user = (UserObject)Session["user"];
            string[] arrId;

            if ((page + 1) * 10 > user.ListUserGroup.Count)
            {
                arrId = new string[user.ListUserGroup.Count - page * 10];
            }
            else
            {
                arrId = new string[10];
            }

            int start = page * 10;
            for (int i = 0; i < arrId.Length; i++)
            {
                arrId[i] = user.ListUserGroup[start + i].GroupId;
            }

            List<GroupObject> listGroup = groupService.LoadList(arrId);
            return View(listGroup);
        }

        public ActionResult LeftMenu(string id)
        {
            var user = (UserObject)Session["user"];
            if (user == null)
            {
                return View();
            }
            else
            {
                //var userGroups = userGroupService.GetByUser(user.Id);
                if (id == null)
                {
                    id = string.Empty;
                }
                TempData["GroupId"] = id;
                return View(user.ListUserGroup);
            } 
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            //TopicObject topic = new TopicObject();
            //topic.TopicID = "T02";
            //topic.Title = "new title";
            //topic.Content = "new content";
            //topic.CreateDate = DateTime.Now;

            //topicService.Save(topic);

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }

        public string Initialized()
        {
            new Utility(MvcApplication.CurrentSession).Initialized();

            return "Initialized data success!!!<br/><a href='/Home'>Go Home</a>";
        }
    }
}
