using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatabaseQuiz.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        // GET: Member
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Upload()
        {
            return View();
        }
    }
}