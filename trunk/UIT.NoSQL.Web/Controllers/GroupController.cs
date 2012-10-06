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

        public GroupController(IGroupService groupService)
        {
            this.groupService = groupService;
        }
        //
        // GET: /Group/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(GroupObject group)
        {
            group.GroupId = Guid.NewGuid().ToString();
            group.CreateDate = DateTime.Now;
            group.CreateBy = "cho' Huy";

            groupService.Save(group);
            return View();
        }
    }
}
