using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                Console.WriteLine("Hello");
            }

            ViewBag.Username = Session["UserName"];
            return View();
        }
    }

}


