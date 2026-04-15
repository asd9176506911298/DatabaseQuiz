using System;
using System.Collections.Generic;
using System.IO;
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

            ViewBag.IsAdmin = member.isAdmin;

            if (uploadedFile == null || uploadedFile.ContentLength == 0)
            {
                ViewBag.ErrorMessage = "請選擇檔案";
                return View();
            }

            if (uploadedFile.ContentLength > 2 * 1024 * 1024)
            {
                ViewBag.ErrorMessage = "圖片超過容量限制";
                return View();
            }

            // Magic Header 驗證
            byte[] buffer = new byte[8];
            uploadedFile.InputStream.Read(buffer, 0, 8);
            uploadedFile.InputStream.Position = 0;
            string hexHeader = BitConverter.ToString(buffer).Replace("-", "").ToUpper();

            bool isPNG = hexHeader.StartsWith("89504E47");
            bool isJPG = hexHeader.StartsWith("FFD8FF");

            if (!isPNG && !isJPG)
            {
                ViewBag.ErrorMessage = "檔案內容格式不符 (Magic Number 驗證失敗)";
                return View();
            }

            string extension = Path.GetExtension(uploadedFile.FileName).ToLower();
            if (extension != ".jpg" && extension != ".png")
            {
                ViewBag.ErrorMessage = "支援JPG、PNG格式";
                return View();
            }

            ViewBag.SuccessMessage = "檔案驗證成功！(模擬上傳完成)";
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}