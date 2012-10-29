using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.Domain;

namespace UIT.NoSQL.Web.Filters
{
    public class SecurityFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            bool isAllow = false;
            string groupID = filterContext.ActionParameters["id"].ToString();

            if (ctx.Session != null && ctx.Session["user"] != null)
            {
                var user = (UserObject)ctx.Session["user"];
                foreach (var userGroup in user.ListUserGroup)
                {
                    if (userGroup.GroupId.Equals(groupID))
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
                throw new FieldAccessException();
            }
        }
    }
}