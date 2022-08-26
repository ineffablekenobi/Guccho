using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guccho.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HttpCookie access = Request.Cookies["access"];
            string role = access != null ? access.Value.Split('=')[1] : "undefined";

            if (!role.Equals("undefined"))
            {
                if (role.Equals("admin"))
                {
                    return RedirectToAction("SignIn", "Admins");
                }
                else
                {
                    return RedirectToAction("SignIn", "Students");
                }
            }
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

      

    }
}