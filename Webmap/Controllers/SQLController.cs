using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webmap.Controllers
{
    public class SQLController : Controller
    {
        // GET: SQL
        public JsonResult Upit(DateTime start, DateTime end)
        {
            RuckusLogEntities6 obj = new RuckusLogEntities6();
            DateTime startDate = start;
            DateTime endDate = end;

            var deviceCount = (from r in obj.Ruckus_Log where r.EventTypeId == 2 && (r.TimeStamp >= startDate && r.TimeStamp <= endDate) select r).Count();

            return Json(new { data1 = deviceCount }, JsonRequestBehavior.AllowGet);
        }
    }
}