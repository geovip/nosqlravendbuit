using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Web.Factory;

namespace UIT.NoSQL.Web.Filters
{
    public class MemberFilter : ActionFilterAttribute
    {
        public TypeIDEnum TypeID { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            bool isAllow = false;
            string groupID = string.Empty;

            if(TypeID == TypeIDEnum.GroupID)
            {
                groupID = filterContext.ActionParameters["id"].ToString();
            }
            else if (TypeID == TypeIDEnum.TopicID)
            {
                string topicID = filterContext.ActionParameters["id"].ToString();
                ITopicService topicService = MvcUnityContainer.Container.Resolve(typeof(ITopicService), "") as ITopicService;
                var topicObject = topicService.Load(topicID);

                groupID = topicObject.GroupId;
            }

            if (ctx.Session != null && ctx.Session["user"] != null)
            {
                var user = (UserObject)ctx.Session["user"];
                foreach (var userGroup in user.ListUserGroup)
                {
                    if (userGroup.GroupId.Equals(groupID) && userGroup.IsApprove == UserGroupStatus.Approve)
                    {
                        isAllow = true;
                        break;
                    }
                }
            }

            if (isAllow)
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("/Group/AccessDenied/" + groupID);
            }
        }
    }
}