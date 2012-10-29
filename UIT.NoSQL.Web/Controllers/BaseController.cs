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
        protected bool SecurityCheck(GroupObject groupObject, string groupID)
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
                        if (userGroup.GroupId.Equals(groupID) && userGroup.IsApprove == true)
                        {
                            isAllow = true;
                        }
                    }
                }
            }

            return isAllow;
        }
    }
}