using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Service;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Web.Factory;
using UIT.NoSQL.Web.Filters;
using UIT.NoSQL.Web.Models;

namespace UIT.NoSQL.Web.Controllers
{
    public class GroupController : BaseController
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
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IGroupRoleService groupRoleService = MvcUnityContainer.Container.Resolve(typeof(IGroupRoleService), "") as IGroupRoleService;

            var groupRole = groupRoleService.LoadByName(GroupRoleType.Owner);

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
            userGroup.IsApprove = true;
            userGroup.JoinDate = DateTime.Now;
            userGroup.ListGroupRole.Add(groupRole);
            group.ListUserGroup.Add(userGroup);
            
            var user = (UserObject)Session["user"];
            user.ListUserGroup.Add(userGroup);
            
            userService.Save(user);
            groupService.Save(group);
            userGroupService.Save(userGroup);

            return RedirectToAction("Detail", "Group", new  { id = group.Id });
        }

        public ActionResult Detail(string id)
        {
            var group = groupService.Load(id);
            if (group != null)
            {
                if (SecurityCheck(group, id))
                {
                    ViewBag.IsMember = true;

                    ViewBag.ListTopic = group.ListTopic;
                    ViewBag.GroupID = id;

                    TempData["GroupId"] = id;

                    return View();
                }
            }
            
            return RedirectToAction("AccessDenied", new { id });
        }

        public ActionResult AccessDenied(string id)
        {
            ViewBag.GroupID = id;

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
            userGroup.IsApprove = false;
            group.ListUserGroup.Add(userGroup);

            user.ListUserGroup.Add(userGroup);
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;

            userService.Save(user);
            groupService.Save(group);
            userGroupService.Save(userGroup);

            return RedirectToAction("Detail", new { id });
        }

        public ActionResult JoinRequest(string id)
        {
            List<UserGroupObject> listUserGroup = new List<UserGroupObject>();
            var groupObject = groupService.LoadWithUser(id);
            
            foreach (var userGroup in groupObject.ListUserGroup)
            {
                if (!userGroup.IsApprove)
                {
                    listUserGroup.Add(userGroup);
                    ViewBag.GroupID = id;
                }
            }
            ViewBag.GroupID = id;
            
            return View(listUserGroup);
        }

        [HttpPost]
        public ActionResult ActiveRequest(string id)
        {            
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IUserGroupService userGroupService = MvcUnityContainer.Container.Resolve(typeof(IUserGroupService), "") as IUserGroupService;
            IGroupRoleService groupRoleService = MvcUnityContainer.Container.Resolve(typeof(IGroupRoleService), "") as IGroupRoleService;

            var userGroup = userGroupService.Load(id);
            var groupObject = groupService.Load(userGroup.GroupId);
            var groupRole = groupRoleService.LoadByName(GroupRoleType.Member);
            var user = userService.Load(userGroup.UserId);

            userGroup.IsApprove = true;
            userGroup.JoinDate = DateTime.Now;
            userGroup.ListGroupRole.Add(groupRole);

            foreach (var item in groupObject.ListUserGroup)
            {
                if (item.Id.Equals(userGroup.Id))
                {
                    item.IsApprove = true;
                    item.JoinDate = userGroup.JoinDate;
                    item.ListGroupRole.Add(groupRole);
                    //userGroup.ListGroupRole.Add(groupRole);
                    break;
                }
            }

            foreach (var item in user.ListUserGroup)
            {
                if (item.Id.Equals(userGroup.Id))
                {
                    item.IsApprove = true;
                    item.JoinDate = userGroup.JoinDate;
                    item.ListGroupRole.Add(groupRole);
                    //userGroup.ListGroupRole.Add(groupRole);
                    break;
                }
            }

            groupService.Save(groupObject);
            userService.Save(user);
            userGroupService.Save(userGroup);
            
            return RedirectToAction("JoinRequest", new { id = userGroup.GroupId });
        }

        public ActionResult Member(string id)
        {
            List<UserObject> listUser;
            List<UserGroupObject> listUserGroup;
            var group = groupService.LoadWithUser(id, out listUser, out listUserGroup);
            if (group != null)
            {
                if (SecurityCheck(group, id))
                {
                    List<ListUserModels> listUserModel = new List<ListUserModels>();
                    ListUserModels userModels = null;
                    for (int i = 0; i < listUser.Count; i++)
                    {
                        if (listUserGroup[i].ListGroupRole.Count == 0)
                        {
                            continue;
                        }
                        userModels = new ListUserModels();
                        userModels.UserGroupID = listUserGroup[i].Id;
                        userModels.FullName = listUser[i].FullName;
                        userModels.UserName = listUser[i].UserName;
                        userModels.Email = listUser[i].Email;
                        userModels.Role = listUserGroup[i].ListGroupRole[0].GroupName;

                        listUserModel.Add(userModels);
                    }
                    ViewBag.GroupID = id;

                    return View(listUserModel);
                }
            }
            
            return RedirectToAction("AccessDenied", new { id });
        }

        public ActionResult LeftManager(string id)
        {
            ViewBag.GroupID = id;
            return View();
        }
    }
}
