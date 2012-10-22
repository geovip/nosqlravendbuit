using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Service;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Web.Factory;

namespace UIT.NoSQL.Web.Controllers
{
    public class GroupController : Controller
    {     
        private IGroupService groupService;
        private IUserGroupService userGroupService;

        public GroupController(IGroupService groupService, IUserGroupService userGroupService)
        {
            this.groupService = groupService;
            this.userGroupService = userGroupService; 
        }
        //
        // GET: /Group/

        public ActionResult Index()
        {
            //var user = (UserObject)Session["user"];
            //if (user == null)
            //{
            //    ViewBag.IsLogined = false;
            //    return View();
            //}
            //else
            {
                ViewBag.IsLogined = true;
                var groups = groupService.GetAll();
                return View(groups);
            }         
        }

        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index","Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(GroupObject group)
        {
            string userId = ((UserObject)Session["user"]).Id;
            group.Id = Guid.NewGuid().ToString();
            group.CreateDate = DateTime.Now;
            group.CreateBy = userId;

            var userGroup = new UserGroupObject();
            userGroup.Id = Guid.NewGuid().ToString();
            userGroup.UserId = userId;
            userGroup.GroupId = group.Id;
            userGroup.GroupName = group.GroupName;
            userGroup.Description = group.Description;
            group.ListUserGroup.Add(userGroup);
            

            var user = (UserObject)Session["user"];
            user.ListUserGroup.Add(userGroup);
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            
            userService.Save(user);
            groupService.Save(group);
            userGroupService.Save(userGroup);

            return RedirectToAction("Detail", "Group", new  { id = group.Id });
        }

        public ActionResult Detail(string id)
        {
            //string groupId = Request.QueryString[0];
            var group = groupService.Load(id);
            ViewBag.ListTopic = group.ListTopic;
            ViewBag.GroupID = id;

            var user = (UserObject)Session["user"];
            bool isMember = false;
            foreach (var userGroup in group.ListUserGroup)
            {
                if (userGroup.UserId.Equals(user.Id))
                {
                    isMember = true;
                    break;
                }
            }

            ViewBag.IsMember = isMember;

            return View();
        }

        [HttpPost]
        public ActionResult Join(string id)
        {
            var user = (UserObject)Session["user"];
            var group = groupService.Load(id);
            var userGroup = new UserGroupObject();
            userGroup.Id = Guid.NewGuid().ToString();
            userGroup.UserId = user.Id;;
            userGroup.GroupId = group.Id;
            userGroup.GroupName = group.GroupName;
            userGroup.Description = group.Description;
            group.ListUserGroup.Add(userGroup);

            user.ListUserGroup.Add(userGroup);
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            userService.Save(user);

            groupService.Save(group);
            userGroupService.Save(userGroup);

            return RedirectToAction("Detail", new { id });
        }

        public ActionResult Member(string id)
        {
            var group = groupService.Load(id);
            return View(group.ListUserGroup);
        }
    }
}
