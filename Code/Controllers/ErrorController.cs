using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMEWebCAD.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            string msg = Request.QueryString["msg"].ToString();
            ViewBag.strError = msg;
            return View();
        }

    }
}
