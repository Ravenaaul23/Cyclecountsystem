using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CycleCountSystem__CSS_.LoginSecurity;
using CycleCountSystem__CSS_.Models;

namespace CycleCountSystem__CSS_.Controllers
{
    public class HomeController : Controller
    {
        private CyclecountsystemEntities db = new CyclecountsystemEntities();

        public ActionResult Dashboard()
        {          
            List<TB_Inventory> initialData = GetDataFromYourSource();

            return View(initialData);
        }

        private List<TB_Inventory> GetDataFromYourSource()
        {
            return db.TB_Inventory.ToList();
        }

        // Metode untuk menampilkan tampilan setelah memilih kategori
        public ActionResult FilterByCategory(string category)
        {
            // Di sini Anda dapat memfilter data berdasarkan kategori yang dipilih
            List<TB_Inventory> filteredData = FilterDataByCategory(category);

            return View("Dashboard", filteredData);
        }

        // Metode untuk memfilter data berdasarkan kategori ITR
        private List<TB_Inventory> FilterDataByCategory(string category)
        {
            List<TB_Inventory> allData = GetDataFromYourSource();
            List<TB_Inventory> filteredData;

            switch (category)
            {
                case "Kategori All":
                    filteredData = allData.Where(item =>
                        decimal.TryParse(item.ITR.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal itr) &&
                        itr >= 0.00M &&
                        itr <= 3.00M
                    ).ToList();
                    break;
                case "Kategori 1":
                    filteredData = allData.Where(item =>
                        decimal.TryParse(item.ITR.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal itr) &&
                        itr >= 2.50M &&
                        itr <= 3.00M
                    ).ToList();
                    break;
                case "Kategori 2":
                    filteredData = allData.Where(item =>
                        decimal.TryParse(item.ITR.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal itr) &&
                        itr >= 1.00M &&
                        itr <= 2.49M
                    ).ToList();
                    break;
                case "Kategori 3":
                    filteredData = allData.Where(item =>
                        decimal.TryParse(item.ITR.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal itr) &&
                        itr >= 0.00M &&
                        itr <= 0.99M
                    ).ToList();
                    break;
                default:
                    filteredData = allData;
                    break;
            }

            return filteredData;
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public TB_Role GetCurrentUser()
        {
            var username = HttpContext.User.Identity.Name;
            return db.TB_Role.FirstOrDefault(x => x.windows_account.ToLower() == username.ToLower());
        }

        public ActionResult Sidebar()
        {
            var currentUser = GetCurrentUser()?.role;

            if (currentUser == null)
            {
                ViewBag.role = 0;
            }
            else
            {
                ViewBag.role = Int32.Parse(currentUser);
            }

            return PartialView();
        }

    }
}