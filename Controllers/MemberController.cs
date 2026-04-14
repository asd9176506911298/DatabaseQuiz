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

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase uploadedFile)
        {
            var member = Session["User"] as DatabaseQuiz.Models.Member;
            if (member == null || !member.isAdmin)
            {
                TempData["Error"] = "權限不足，無法上傳圖片";
                return RedirectToAction("Upload");
            }

            // 2. 檢查是否有檔案
            if (uploadedFile != null && uploadedFile.ContentLength > 0)
            {
                // 3. 後端檢查大小 (2MB = 2097152 bytes)
                if (uploadedFile.ContentLength > 2 * 1024 * 1024)
                {
                    ViewBag.ErrorMessage = "圖片超過容量限制";
                    return View();
                }

                // 4. 後端檢查副檔名
                string extension = System.IO.Path.GetExtension(uploadedFile.FileName).ToLower();
                if (extension == ".jpg" || extension == ".png")
                {
                    ViewBag.SuccessMessage = "檔案驗證成功！(模擬上傳完成)";
                    ViewBag.IsAdmin = true; // 保持頁面狀態
                    ViewBag.UserDisplayName = "ADMIN";
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = "支援JPG、PNG格式";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "請選擇檔案";
            }

            // 失敗則回傳原頁面
            ViewBag.IsAdmin = member.isAdmin;
            ViewBag.UserDisplayName = "ADMIN";
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}