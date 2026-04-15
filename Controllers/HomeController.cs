using DatabaseQuiz.Models;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

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
            else if (!IsValidTaiwanId(UserId))
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
            else if (!IsValidPhoneNumber(PhoneNumber))
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

            Session["User"] = member;

            // 登入成功
            FormsAuthentication.SetAuthCookie(UserId, true);
            return RedirectToAction("Index", "Member");
        }

        private bool IsValidTaiwanId(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id.Length != 10)
                return false;

            id = id.ToUpper();

            // 字母對應數字表（A=10, B=11, ... Z=35，跳過某些字母）
            var letterMap = new Dictionary<char, int>
            {
                {'A',10},{'B',11},{'C',12},{'D',13},{'E',14},{'F',15},{'G',16},
                {'H',17},{'I',34},{'J',18},{'K',19},{'L',20},{'M',21},{'N',22},
                {'O',35},{'P',23},{'Q',24},{'R',25},{'S',26},{'T',27},{'U',28},
                {'V',29},{'W',32},{'X',30},{'Y',31},{'Z',33}
            };

            // 第1碼必須是合法英文字母
            if (!letterMap.ContainsKey(id[0]))
                return false;

            // 第2碼必須是 1 或 2
            if (id[1] != '1' && id[1] != '2')
                return false;

            // 第3～10碼必須都是數字
            for (int i = 2; i < 10; i++)
            {
                if (!char.IsDigit(id[i]))
                    return false;
            }

            // 加權校驗計算
            int letterValue = letterMap[id[0]];
            int n1 = letterValue / 10;   // 十位數
            int n2 = letterValue % 10;   // 個位數

            int[] weights = { 1, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
            int[] digits = new int[11];

            digits[0] = n1;
            digits[1] = n2;
            for (int i = 2; i < 11; i++)
                digits[i] = id[i - 1] - '0';

            int sum = 0;
            for (int i = 0; i < 11; i++)
                sum += digits[i] * weights[i];

            return sum % 10 == 0;
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10)
                return false;

            // 必須以 09 開頭，且全為數字
            return phone.StartsWith("09") && phone.All(char.IsDigit);
        }
    }
}