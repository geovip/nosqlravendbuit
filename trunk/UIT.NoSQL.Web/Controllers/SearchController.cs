using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Core.Domain;
using Raven.Client.Indexes;

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
        public ActionResult Index(string searchStr, int page)
        {
            DateTime start;
            DateTime end;
            int totalResult;

            start = DateTime.Now;
            searchStr = searchStr.Trim();

            List<GroupObject> listGroup = groupService.Search(searchStr, page*10, 10, out totalResult);
            end = DateTime.Now;

            TempData["totalResult"] = totalResult;
            TempData["paging"] = (totalResult/10) + ((totalResult%10) != 0?1:0);
            TempData["searchStr"] = searchStr;
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

        [HttpPost]
        public ActionResult More(string searchStr, int page)
        {
            DateTime start;
            DateTime end;
            int totalResult;

            start = DateTime.Now;
            searchStr = searchStr.Trim();

            List<GroupObject> listGroup = groupService.Search(searchStr, page*10, 10, out totalResult);
            end = DateTime.Now;
            
            return View(listGroup);
        }
    }
    
    //var session = MvcApplication.CurrentSession;
    //    new GroupObject_Count().Execute(MvcApplication.documentStore);

    //var result = session.Query< GroupObject_Count>().ToList();
    //        //.Where(x => x.SearchQuery == (object)searchStr);
    //        //.As<GroupObject>();
    //        //.ToList();

    ////int count = result;
    //public class GroupObject_Count : AbstractIndexCreationTask<GroupObject, GroupObject_Count.GroupReduceResult>
    //{

    //    public class GroupReduceResult
    //    {
    //        public String GroupName { get; set; }
    //        public int Count { get; set; }
    //    }

    //    public GroupObject_Count()
    //    {
    //        Map = groups =>
    //            from g in groups
    //            select new
    //            {
    //                //GroupName = g.Tags.Concat(new[]
    //                //                            {
    //                //                                g.GroupName,
    //                //                                g.Description
    //                //                            }),
    //                GroupName = g.GroupName,
    //                Count = 1
    //            };

    //        Reduce = results => from result in results
    //                            group result by result.GroupName into r
    //                            select new
    //                            {
    //                                GroupName = r.Key,
    //                                Count = r.Sum(x=>x.Count)
    //                            };
    //    }
    //}
}
