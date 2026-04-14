using DatabaseQuiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DatabaseQuiz.Controllers
{
    public class HomeController : Controller
    {
        MyDBContext db = new MyDBContext();

        public ActionResult Index()
        {
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserId, string PhoneNumber)
        {
            bool hasError = false;

            // 1. 檢核身分證字號
            if (string.IsNullOrWhiteSpace(UserId))
            {
                ViewBag.UserIdMessage = "請輸入身分證字號";
                hasError = true;
            }
            else if (UserId.Length != 10) // 簡單範例：身分證應為10碼
            {
                ViewBag.UserIdMessage = "身分證字號有誤";
                hasError = true;
            }

            // 2. 檢核手機門號
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ViewBag.PhoneNumberMessage = "請輸入手機門號";
                hasError = true;
            }
            else if (PhoneNumber.Length != 10) // 假設手機需為10碼
            {
                ViewBag.PhoneNumberMessage = "手機門號長度不正確";
                hasError = true;
            }

            // 如果前兩步有錯，直接回傳頁面顯示錯誤訊息
            if (hasError)
            {
                return View();
            }

            // 3. 資料庫驗證
            var member = db.Members.Where(m => m.UserId == UserId && m.PhoneNumber == PhoneNumber).FirstOrDefault();

            if (member == null)
            {
                // 如果格式正確但找不到人，通常會歸類在身分證或手機錯誤（或是統一顯示登入失敗）
                ViewBag.UserIdMessage = "此帳號不存在";
                return View();
            }

            Session["Welcome"] = $"{member.UserName} 您好!";
            // 登入成功
            FormsAuthentication.SetAuthCookie(UserId, true);
            return RedirectToAction("Index", "Member");
        }
    }
}