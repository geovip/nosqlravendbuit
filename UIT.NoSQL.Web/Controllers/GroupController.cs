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
            group.Tags = new[]{group.GroupName, group.Description};

            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;
            IGroupRoleService groupRoleService = MvcUnityContainer.Container.Resolve(typeof(IGroupRoleService), "") as IGroupRoleService;

            var groupRole = groupRoleService.LoadByName(GroupRoleType.Owner);

            string userId = ((UserObject)Session["user"]).Id;
            group.Id = Guid.NewGuid().ToString();
            group.CreateDate = DateTime.Now;
            group.CreateBy = userId;
            group.IsPublic = false;

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
            
            var user = (UserObject)Session["user"];
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
            ViewBag.IsMember = true;
            ViewBag.GroupName = group.GroupName;

            return View(group.ListTopic);
        }

        public ActionResult AccessDenied(string id)
        {
            TempData["GroupId"] = id;
            TempData["GroupId"] = id;
            return View();
        }

        [HttpPost]
        [LoginFilter]
        public ActionResult Join(string id)
        {
            IGroupRoleService groupRoleService = MvcUnityContainer.Container.Resolve(typeof(IGroupRoleService), "") as IGroupRoleService;
            var groupRole = groupRoleService.LoadByName(GroupRoleType.Member);
            var user = (UserObject)Session["user"];
            var group = groupService.Load(id);
            var userGroup = new UserGroupObject();

            userGroup.Id = Guid.NewGuid().ToString();
            userGroup.UserId = user.Id;;
            userGroup.GroupId = group.Id;
            userGroup.GroupName = group.GroupName;
            userGroup.Description = group.Description;
            userGroup.IsApprove = UserGroupStatus.JoinRequest;
            userGroup.GroupRole = groupRole;

            group.ListUserGroup.Add(userGroup);

            user.ListUserGroup.Add(userGroup);
            IUserService userService = MvcUnityContainer.Container.Resolve(typeof(IUserService), "") as IUserService;

            userService.Save(user);
            groupService.Save(group);
            userGroupService.Save(userGroup);

            return RedirectToAction("Detail", new { id });
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
                    //userGroup.ListGroupRole.Add(groupRole);
                    break;
                }
            }

            foreach (var item in user.ListUserGroup)
            {
                if (item.Id.Equals(userGroup.Id))
                {
                    item.IsApprove = UserGroupStatus.Approve;
                    item.JoinDate = userGroup.JoinDate;
                    //userGroup.ListGroupRole.Add(groupRole);
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
            TempData["IsMember"] = id;
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

        public ActionResult TopMenuUser(string id)
        {
            TempData["IsMember"] = CheckViewGroup(groupService.Load(id)).ToString();
            TempData["GroupId"] = id;
            return View();
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
