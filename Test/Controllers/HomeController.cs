using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Dashboard()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login","Index");
            }

            ViewBag.Username = Session["UserName"];
            return View();
        }
    }
}