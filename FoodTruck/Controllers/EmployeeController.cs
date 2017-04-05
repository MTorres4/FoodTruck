using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Schedule()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            //Here MyDatabaseEntities is our entity datacontext (see Step 4)
            using (Entities1 dc = new Entities1())
            {
                var v = dc.WorkerSchedules.OrderBy(a => a.StartAt).ToList();
                return new JsonResult { Data = v, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //Action for Save event
        [HttpPost]
        public JsonResult SaveEvent(WorkerSchedule evt)
        {
            bool status = false;
            using (Entities1 dc = new Entities1())
            {
                if (evt.EndAt != null && evt.StartAt.TimeOfDay == new TimeSpan(0, 0, 0) &&
                    evt.EndAt.Value.TimeOfDay == new TimeSpan(0, 0, 0))
                {
                    evt.IsFullDay = true;
                }
                else
                {
                    evt.IsFullDay = false;
                }
                if (evt.EventID > 0)
                {
                    var v = dc.WorkerSchedules.Where(a => a.EventID.Equals(evt.EventID)).FirstOrDefault();
                    if (v != null)
                    {
                        v.Title = evt.Title;
                        v.Description = evt.Description;
                        v.StartAt = evt.StartAt;
                        v.EndAt = evt.EndAt;
                        v.IsFullDay = evt.IsFullDay;
                    }
                }
                else
                {
                    dc.WorkerSchedules.Add(evt);
                }
                dc.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            bool status = false;
            using (Entities1 dc = new Entities1())
            {
                var v = dc.WorkerSchedules.Where(a => a.EventID.Equals(eventID)).FirstOrDefault();
                if (v != null)
                {
                    dc.WorkerSchedules.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}