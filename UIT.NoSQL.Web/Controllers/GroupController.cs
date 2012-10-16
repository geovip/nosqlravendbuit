using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Service;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;

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
            groupService.Save(group);

            var userGroup = new UserGroupObject();
            userGroup.Id = Guid.NewGuid().ToString();
            userGroup.UserId = userId;
            userGroup.GroupId = group.Id;
            userGroup.GroupName = group.GroupName;
            userGroup.Description = group.Description;
            userGroupService.Save(userGroup);

            return RedirectToAction("Index", "UserGroup");
        }

    }
}
