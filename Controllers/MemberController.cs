using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
            // 從 Session 取得登入者的 Member 物件
            var member = Session["User"] as DatabaseQuiz.Models.Member;

            // 如果 Session 掉了或是找不到資料，預設為一般用戶
            if (member == null)
            {
                ViewBag.IsAdmin = false;
                ViewBag.UserDisplayName = "USER"; // 預設顯示
            }
            else
            {
                ViewBag.IsAdmin = member.isAdmin;
                ViewBag.UserDisplayName = member.isAdmin ? "ADMIN" : "USER";
            }

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}