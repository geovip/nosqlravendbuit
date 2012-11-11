using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Web.Factory;
using UIT.NoSQL.Core.IService;

namespace UIT.NoSQL.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool CheckViewGroup(GroupObject groupObject)
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
                        }
                    }
                }
            }

            return isAllow;
        }

        protected bool CheckViewTopic(TopicObject topicObject)
        {
            IGroupService groupService = MvcUnityContainer.Container.Resolve(typeof(IGroupService), "") as IGroupService;
            var groupObject = groupService.Load(topicObject.GroupId);

            return CheckViewGroup(groupObject);
        }
    }
}