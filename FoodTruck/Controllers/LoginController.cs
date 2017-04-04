using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class LoginController : Controller
    {

        ApplicationDbContext db;

        public LoginController()
        {
            db = new ApplicationDbContext();
        }


        // GET: Login
        public ActionResult Index()
        {
            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            if(User.IsInRole("Employee"))
            {
                return RedirectToAction("Index", "Employee");
            }

            return View();
        }
    }
}