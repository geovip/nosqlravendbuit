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

        private int GetTotalResultEachServer(string searchStr, IDocumentStore documentStore)
        {
            var session = documentStore.OpenSession();
            return session.Query<GroupObject, GroupObject_Search>()
                .Search(x => x.GroupName, searchStr).Count();
        }
        
        private List<GroupObject> DoSearch(int page, string searchStr, int pageSize, int[] results, int totalServer)
        {
            var listGroups = new List<GroupObject>();
            bool begin = false;
            int total = page * pageSize;
            int skip = 0;
            int take = 10;

            for (int i = 0; i < totalServer; i++)
            {
                if (begin == false)
                {
                    skip += results[i];
                    if (total < skip)
                    {
                        begin = true;
                        skip -= results[i];
                    }
                }

                if (begin == true)
                {
                    if (skip <= 0)
                    {
                        skip = total;
                    }
                    else
                    {
                        skip -= total;
                    }
                }


                var session = MvcApplication.documentStores[i].OpenSession();
                var listResults = session.Query<GroupObject, GroupObject_Search>()
                    .Skip(skip)
                    .Take(take)
                    .Search(x => x.GroupName, searchStr)
                    .ToList();

                listGroups.AddRange(listResults);
                if (listGroups.Count >= pageSize)
                {
                    break;
                }
            }

            return listGroups;
        }

        //
        // GET: /Search/
        [HttpGet]
        public ActionResult Index(string searchStr)
        {
            DateTime start;
            DateTime end;

            start = DateTime.Now;

            int totalServer = MvcApplication.documentStores.Count();
            int[] results = new int[totalServer];
            int totalResult = 0;

            for (int i = 0; i < totalServer; i++)
            {
                results[i] = GetTotalResultEachServer(searchStr, MvcApplication.documentStores[i]);
                totalResult += results[i];
            }


            var listGroups = DoSearch(0, searchStr, 10, results, totalServer);

            string resultsStr = string.Empty;
            for (int i = 0; i < totalServer; i++)
			{
                resultsStr += results[i] + ";";
			}
            resultsStr = resultsStr.Substring(0, resultsStr.Length - 1);
            TempData["results"] = resultsStr;
            TempData["totalResult"] = totalResult;
            TempData["paging"] = (totalResult / 10) + ((totalResult % 10) != 0 ? 1 : 0);
            TempData["searchStr"] = searchStr;

            end = DateTime.Now;
            var sub = end - start;
            if (sub.Milliseconds < 1000)
            {
                TempData["time"] = sub.Milliseconds + " milliseconds";
            }
            else
            {
                TempData["time"] = sub.Seconds + " seconds";
            }

            #region "Old"
            //DateTime start;
            //DateTime end;
            //int totalResult;

            //start = DateTime.Now;
            //searchStr = searchStr.Trim();
            //groupService = new GroupService(MvcApplication.CurrentSession, MvcApplication.documentStores);
            //List<GroupObject> listGroup = groupService.Search(searchStr, out totalResult);
            //end = DateTime.Now;

            //TempData["totalResult"] = totalResult;
            //var sub = end - start;
            //if(sub.Milliseconds < 1000)
            //{
            //    TempData["time"] = sub.Milliseconds + " milliseconds";
            //}
            //else
            //{
            //    TempData["time"] = sub.Seconds + " seconds";
            //}
            #endregion
            return View(listGroups);
        }

        [HttpPost]
        public ActionResult More(string searchStr, int page, string resultsStr)
        {
            int totalServer = MvcApplication.documentStores.Count();
            int[] results = new int[totalServer];
            string[] resultsArr = resultsStr.Split(';');

            for (int i = 0; i < totalServer; i++)
            {
                results[i] = Int32.Parse(resultsArr[i]);
            }

            searchStr = searchStr.Trim();

            var listGroups = DoSearch(page, searchStr, 10, results, totalServer);

            return View(listGroups);
        }
    }
}
