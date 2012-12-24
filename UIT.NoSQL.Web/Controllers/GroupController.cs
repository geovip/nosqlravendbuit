﻿using System;
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
using Raven.Abstractions.Data;

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

        [LoginFilter]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [LoginFilter]
        public ActionResult Create(GroupObject group)
        {
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IGroupRoleService groupRoleService = MvcUnityContainer.Container.Resolve(typeof(IGroupRoleService), "") as IGroupRoleService;

            var groupRole = groupRoleService.LoadByName(GroupRoleType.Owner);
            
            UserObject user = ((UserObject)Session["user"]);
            string userId = user.Id;
            group.Id = Guid.NewGuid().ToString();
            group.CreateDate = DateTime.Now;
            group.CreateBy = user;
            group.IsPublic = false;
            group.NewEvent = new GroupEvent();
            group.NewEvent.Title = "New group";
            group.NewEvent.CreateDate = group.CreateDate;
            group.NewEvent.CreateBy = user.FullName;

            var userGroup = new UserGroupObject();
            userGroup.Id = Guid.NewGuid().ToString();
            userGroup.UserId = userId;
            userGroup.GroupId = group.Id;
            userGroup.GroupName = group.GroupName;
            userGroup.Description = group.Description;
            userGroup.IsApprove = UserGroupStatus.Approve;
            userGroup.JoinDate = DateTime.Now;
            userGroup.GroupRole = groupRole;
            group.ListUserGroup.Add(userGroup);
            
            //var user = (UserObject)Session["user"];
            user.ListUserGroup.Add(userGroup);
            
            userService.Save(user);
            groupService.Save(group);
            userGroupService.Save(userGroup);

            return RedirectToAction("Detail", "Group", new  { id = group.Id });
        }

        [MemberFilter(TypeID=TypeIDEnum.GroupID)]
        public ActionResult Detail(string id)
        {
            var group = groupService.Load(id);

            UserObject user = (UserObject)Session["user"];
            var role = user.ListUserGroup.Find(u => u.GroupId == id).GroupRole.GroupName;
            if(role.Equals("Member"))
                ViewBag.IsMember = true;
            else
                ViewBag.IsMember = false;
            ViewBag.GroupName = group.GroupName;
            ViewBag.GroupId = group.Id;

            return View(group.ListTopic);
        }

        [HttpPost]
        [LoginFilter]
        public String Join(string id)
        {
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IGroupRoleService groupRoleService = MvcUnityContainer.Container.Resolve(typeof(IGroupRoleService), "") as IGroupRoleService;
            var userId = ((UserObject)Session["user"]).Id;
            var user = userService.Load(userId);
            bool reSend = false;

            foreach (var usergroup in user.ListUserGroup)
            {
                if (usergroup.GroupId.Equals(id))
                {
                    if (usergroup.IsApprove == UserGroupStatus.JoinRequest)
                    {
                        return "Please wait for approval";
                    }
                    else if (usergroup.IsApprove == UserGroupStatus.Reject)
                    {
                        reSend = true;
                    }
                    else
                    {
                        Redirect("/Group/Detail/" + id);
                        return string.Empty;
                    }
                }
            }

            var group = groupService.Load(id);

            if (reSend)
            {
                foreach (var usergroup in group.ListUserGroup)
                {
                    if (usergroup.UserId.Equals(userId))
                    {
                        usergroup.IsApprove = UserGroupStatus.JoinRequest;
                    }
                }
            }
            else
            {
                var groupRole = groupRoleService.LoadByName(GroupRoleType.Member);
                var userGroup = new UserGroupObject();

                userGroup.Id = Guid.NewGuid().ToString();
                userGroup.UserId = user.Id; ;
                userGroup.GroupId = group.Id;
                userGroup.GroupName = group.GroupName;
                userGroup.Description = group.Description;
                userGroup.IsApprove = UserGroupStatus.JoinRequest;
                userGroup.GroupRole = groupRole;

                group.ListUserGroup.Add(userGroup);
                user.ListUserGroup.Add(userGroup);

                Session["user"] = user;
                userService.Save(user);
                userGroupService.Save(userGroup);
            }
            groupService.Save(group);

            return "Request success";
        }

        [ManagerFilter(TypeID = TypeIDEnum.GroupID)]
        public ActionResult JoinRequest(string id)
        {
            List<UserGroupObject> listUserGroup = new List<UserGroupObject>();
            var groupObject = groupService.LoadWithUser(id);
            
            foreach (var userGroup in groupObject.ListUserGroup)
            {
                if (userGroup.IsApprove == UserGroupStatus.JoinRequest)
                {
                    listUserGroup.Add(userGroup);
                }
            }
            
            return View(listUserGroup);
        }

        [HttpPost]
        public String RejectRequest(string id)
        {
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IUserGroupService userGroupService = MvcUnityContainer.Container.Resolve(typeof(IUserGroupService), "") as IUserGroupService;

            var userGroup = userGroupService.Load(id);
            var groupObject = groupService.Load(userGroup.GroupId);
            var user = userService.Load(userGroup.UserId);

            userGroup.IsApprove = UserGroupStatus.Reject;

            foreach (var item in groupObject.ListUserGroup)
            {
                if (item.Id.Equals(userGroup.Id))
                {
                    item.IsApprove = UserGroupStatus.Reject;
                    break;
                }
            }

            foreach (var item in user.ListUserGroup)
            {
                if (item.Id.Equals(userGroup.Id))
                {
                    item.IsApprove = UserGroupStatus.Reject;
                    break;
                }
            }

            groupService.Save(groupObject);
            userService.Save(user);
            userGroupService.Save(userGroup);

            return "Success";
        }

        [HttpPost]
        public String ActiveRequest(string id)
        {            
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IUserGroupService userGroupService = MvcUnityContainer.Container.Resolve(typeof(IUserGroupService), "") as IUserGroupService;

            var userGroup = userGroupService.Load(id);
            var groupObject = groupService.Load(userGroup.GroupId);
            var user = userService.Load(userGroup.UserId);

            userGroup.IsApprove = UserGroupStatus.Approve;
            userGroup.JoinDate = DateTime.Now;

            foreach (var item in groupObject.ListUserGroup)
            {
                if (item.Id.Equals(userGroup.Id))
                {
                    item.IsApprove = UserGroupStatus.Approve;
                    item.JoinDate = userGroup.JoinDate;
                    break;
                }
            }

            foreach (var item in user.ListUserGroup)
            {
                if (item.Id.Equals(userGroup.Id))
                {
                    item.IsApprove = UserGroupStatus.Approve;
                    item.JoinDate = userGroup.JoinDate;
                    break;
                }
            }

            groupService.Save(groupObject);
            userService.Save(user);
            userGroupService.Save(userGroup);
            
            //return RedirectToAction("JoinRequest", new { id = userGroup.GroupId });
            return "Success";
        }

        [ManagerFilter(TypeID = TypeIDEnum.GroupID)]
        public ActionResult Member(string id)
        {
            List<UserObject> listUser;
            List<UserGroupObject> listUserGroup;
            var group = groupService.LoadWithUser(id, out listUser, out listUserGroup);
            if (group != null)
            {
                List<ListUserModels> listUserModel = new List<ListUserModels>();
                ListUserModels userModels = null;
                for (int i = 0; i < listUser.Count; i++)
                {
                    if (listUserGroup[i].IsApprove != UserGroupStatus.Approve)
                    {
                        continue;
                    }
                    userModels = new ListUserModels();
                    userModels.UserGroupID = listUserGroup[i].Id;
                    userModels.FullName = listUser[i].FullName;
                    userModels.UserName = listUser[i].UserName;
                    userModels.Email = listUser[i].Email;
                    userModels.Role = listUserGroup[i].GroupRole.GroupName;

                    listUserModel.Add(userModels);
                }

                return View(listUserModel);
            }

            return RedirectToAction("AccessDenied", new { id });
        }

        [HttpPost]
        public string RemoveMember(string id)
        {
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IUserGroupService userGroupService = MvcUnityContainer.Container.Resolve(typeof(IUserGroupService), "") as IUserGroupService;

            var userGroup = userGroupService.Load(id);
            if (userGroup.GroupRole.GroupName.Equals(GroupRoleType.Owner.ToString()))
            {
                return "Can't remove owner group";
            }

            var groupObject = groupService.Load(userGroup.GroupId);
            var user = userService.Load(userGroup.UserId);

            foreach (var item in groupObject.ListUserGroup)
            {
                if (item.Id.Equals(id))
                {
                    groupObject.ListUserGroup.Remove(item);
                    break;
                }
            }

            foreach (var item in user.ListUserGroup)
            {
                if (item.Id.Equals(id))
                {
                    user.ListUserGroup.Remove(item);
                    break;
                }
            }

            userGroupService.Delete(id);
            userService.Save(user);
            groupService.Save(groupObject);

            return "success";
        }

        public ActionResult LeftManager(string id)
        {
            var group = groupService.Load(id);
            TempData["GroupId"] = id;
            TempData["GroupName"] = group.GroupName;
            return View();
        }

        [ManagerFilter(TypeID = TypeIDEnum.GroupID)]
        public ActionResult Manager(string id)
        {
            return View();
        }
        [ManagerFilter(TypeID = TypeIDEnum.GroupID)]
        public ActionResult Setting(string id)
        {
            var group = groupService.Load(id);
            return View(group);
        }

        [HttpPost]
        public String UpdateSetting(GroupObject group)
        {
            var groupOld = groupService.Load(group.Id);
            groupOld.IsPublic = group.IsPublic;
            groupOld.GroupName = group.GroupName;
            groupOld.Description = group.Description;

            groupService.Save(groupOld);
            return "Success";
        }

        public ActionResult TopMenuUser(string id, string action)
        {
            TempData["GroupId"] = id;
            TempData["account"] = action;
            return View();
        }

        public ActionResult AccessDenied(string id)
        {
            if (id == null)
            {
                TempData["GroupId"] = "null";
                return View();
            }

            bool isAllow = CheckViewGroup(groupService.Load(id));
            if (isAllow)
            {
                return RedirectToAction("Detail", new  { id = id });
            }
            else
            {
                TempData["GroupId"] = id;
                return View();
            }
        }

        [NonAction]
        private bool CheckViewGroup(GroupObject groupObject)
        {
            bool isAllow = false;
            if (groupObject.IsPublic)
            {
                isAllow = true;
            }
            else
            {
                if (Session["user"] != null)
                {
                    var userID = ((UserObject)Session["user"]).Id;
                    IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
                    var user = userService.Load(userID);

                    foreach (var userGroup in user.ListUserGroup)
                    {
                        if (userGroup.GroupId.Equals(groupObject.Id) && userGroup.IsApprove == UserGroupStatus.Approve)
                        {
                            isAllow = true;
                            break;
                        }
                    }
                }
            }

            return isAllow;
        }
    }
}
