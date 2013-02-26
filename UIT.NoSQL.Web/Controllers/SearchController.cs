using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using Raven.Client.Indexes;
using Raven.Client;
using UIT.NoSQL.Service;
using Raven.Abstractions.Data;
using UIT.NoSQL.Web.Factory;

namespace UIT.NoSQL.Web.Controllers
{
    public class SearchController : Controller
    {
        private IGroupService groupService;

        public SearchController(IGroupService groupService)
        {
            this.groupService = groupService;
        }

        //
        // GET: /Search/
        [HttpPost]
        public ActionResult Index(string searchStr)
        {
            DateTime start;
            DateTime end;
            int totalResult;

            start = DateTime.Now;
            searchStr = searchStr.Trim();
            groupService = new GroupService(MvcApplication.CurrentSession, MvcApplication.documentStores);
            List<GroupObject> listGroup = groupService.Search(searchStr, out totalResult);
            end = DateTime.Now;

            TempData["totalResult"] = totalResult;
            var sub = end - start;
            if(sub.Milliseconds < 1000)
            {
                TempData["time"] = sub.Milliseconds + " milliseconds";
            }
            else
            {
                TempData["time"] = sub.Seconds + " seconds";
            }

            return View(listGroup);
        }
    }
}
