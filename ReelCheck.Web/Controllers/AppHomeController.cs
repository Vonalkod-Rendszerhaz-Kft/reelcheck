using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReelCheck.Web.Controllers
{
    public class AppHomeController : Controller
    {
        public ActionResult About()
        {
            return View();
        }
    }
}