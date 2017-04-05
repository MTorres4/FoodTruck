using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;
using Tweetinvi.Models;

namespace FoodTruck.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Auth.SetUserCredentials("uCu6RTd7IiP6ZaoNNg3vZggOK", "2t25i0TIj52oo10KtEQ4T0QaFurJVxqIaeymx56lguhYT3rn0y", "847438635699814402-eGKgSfcnbe9hODxFMdZPANRsJv247Up", "pxwLkwZDepBVJcfMVWOLBPAyXGrK5ZD2NDtZK1YRM6YXG");
            var user = Tweetinvi.User.GetUserFromScreenName("SupermansTruck");
            var userIdentifier = new UserIdentifier("SupermansTruck");
            var tweets = Timeline.GetUserTimeline(userIdentifier);
            var model = tweets.Select(x => new TweetViewModel() { Date = x.CreatedAt, Text = x.FullText }).ToList();
            return View(model);
        }

        public JsonResult GetEvents()
        {
            //Here MyDatabaseEntities is our entity datacontext (see Step 4)
            using (Entities dc = new Entities())
            {
                var v = dc.Events.OrderBy(a => a.StartAt).ToList();
                return new JsonResult { Data = v, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //Action for Save event
        [HttpPost]
        public JsonResult SaveEvent(Event evt)
        {
            bool status = false;
            using (Entities dc = new Entities())
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
                    var v = dc.Events.Where(a => a.EventID.Equals(evt.EventID)).FirstOrDefault();
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
                    dc.Events.Add(evt);
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
            using (Entities dc = new Entities())
            {
                var v = dc.Events.Where(a => a.EventID.Equals(eventID)).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult Opportunities()
        {
            return View();
        }
    }
}