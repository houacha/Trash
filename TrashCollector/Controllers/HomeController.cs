using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrashCollector.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "What are we about?";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Reach out to us here.";

            return View();
        }
    }
}