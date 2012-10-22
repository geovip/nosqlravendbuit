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
    public class UserGroupController : Controller
    {
        private IUserGroupService userGroupService;
        public UserGroupController(IUserGroupService userGroupService)
        {
            this.userGroupService = userGroupService;
        }

        //
        // GET: /UserGroup/

        public ActionResult Index()
        {
            var user = (UserObject)Session["user"];
            if (user == null)
            {
                ViewBag.IsLogined = false;
                return View();
            }
            else
            {
                ViewBag.IsLogined = true;
                //var userGroups = userGroupService.GetByUser(user.Id);
                return View(user.ListUserGroup);
            }         
        }

        //
        // GET: /UserGroup/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /UserGroup/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UserGroup/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /UserGroup/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /UserGroup/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /UserGroup/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /UserGroup/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
