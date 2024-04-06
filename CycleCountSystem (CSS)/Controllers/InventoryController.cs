using CycleCountSystem__CSS_.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using OfficeOpenXml;
using CycleCountSystem__CSS_.Helper;
using CycleCountSystem__CSS_.LoginSecurity;

namespace CycleCountSystem__CSS_.Controllers
{
    public class InventoryController : Controller
    {
        private CyclecountsystemEntities db = new CyclecountsystemEntities();
        [AuthorizationHandlerAttribute(Roles = "1")]
        // GET: Inventory
        public ActionResult Index()
        {
            return View(db.TB_Inventory.ToList());
        }

        // GET: Inventory/Details/5
        public ActionResult Details(int id)
        {
            return View(db.TB_Inventory.Where(x => x.Id_Inventory == id).FirstOrDefault());
        }

        // GET: Inventory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TB_Inventory inventory)
        {
            try
            {
                string uploadedByUsername = User.Identity.Name;

                int? idAkun = db.TB_Akun
                    .Where(akun => akun.windows_account == uploadedByUsername)
                    .Select(akun => akun.Id_akun)
                    .FirstOrDefault();

                if (idAkun != null)
                {
                    inventory.Id_akun = idAkun;
                    inventory.Calculate = false;
                    db.TB_Inventory.Add(inventory);
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Error");
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(int id)
        {
            return View(db.TB_Inventory.Where(x => x.Id_Inventory == id).FirstOrDefault());
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        public ActionResult Edit(TB_Inventory inventory)
        {
            try
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventory/Delete/5
        public ActionResult Delete(int id)
        {
            return View(db.TB_Inventory.Where(x => x.Id_Inventory == id).FirstOrDefault());
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, TB_Inventory inventory)
        {
            try
            {
                // TODO: Add delete logic here
                inventory = db.TB_Inventory.Where(x => x.Id_Inventory == id).FirstOrDefault();
                db.TB_Inventory.Remove(inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // Upload Excel 
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, int kategori)
        {
            if (file != null && file.ContentLength > 0)
            {
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    using (var up = new CyclecountsystemEntities()) 
                    {                   
                        string uploadedByUsername = User.Identity.Name;

                        int? idAkun = up.TB_Akun
                            .Where(akun => akun.windows_account == uploadedByUsername)
                            .Select(akun => akun.Id_akun)
                            .FirstOrDefault();

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            // Memeriksa apakah ada kolom yang kosong
                            // Trim membersihkan jika ada kolom yang spasi 
                            //IsNullOrEmpty memeriksa string null/kosong
                            if (string.IsNullOrEmpty(worksheet.Cells[row, 1]?.Text?.Trim()) ||
                                string.IsNullOrEmpty(worksheet.Cells[row, 2]?.Text?.Trim()) ||
                                string.IsNullOrEmpty(worksheet.Cells[row, 3]?.Text?.Trim()) ||
                                string.IsNullOrEmpty(worksheet.Cells[row, 4]?.Text?.Trim()) ||
                                string.IsNullOrEmpty(worksheet.Cells[row, 5]?.Text?.Trim()) ||
                                string.IsNullOrEmpty(worksheet.Cells[row, 6]?.Text?.Trim()))
                            {
                                TempData["ErrorMessage"] = "Excel tidak lengkap, harap perbaiki";
                                return RedirectToAction("Index");
                            }

                            // Jika semua kolom telah diisi, buat objek TB_Inventory
                            var inventory = new TB_Inventory
                            {
                                Id_material = worksheet.Cells[row, 1].Text.Trim(),
                                Mat_description = worksheet.Cells[row, 2].Text.Trim(),
                                Batch_number = worksheet.Cells[row, 3].Text.Trim(),
                                Bin = worksheet.Cells[row, 4].Text.Trim(),
                                Qty = worksheet.Cells[row, 5].Text.Trim(),
                                ITR = worksheet.Cells[row, 6].Text.Trim()
                            };

                            inventory.Calculate = false;
                            inventory.Id_akun = idAkun;

                            up.TB_Inventory.Add(inventory);
                        }

                        up.SaveChanges();

                        // Panggil action Notifikasiemail untuk mengirim notifikasi email setelah berhasil mengunggah data
                        var viewModel = new CombineViewModel
                        {
                            CycleCounts = db.TB_Cyclecount.ToList(),
                            EmailAddresses = db.TB_Email.Select(user => user.Email).ToList(),
                            UserNames = db.TB_Email.Select(user => user.Name).ToList(),
                            DateCount = DateTime.Now,
                            SelectedKategori = kategori // Meneruskan nilai kategori yang dipilih ke view model
                        };

                        Notifikasiemail(viewModel); // Memanggil aksi Notifikasiemail untuk mengirim pemberitahuan email setelah berhasil mengunggah data
                    }
                }
            }

            return RedirectToAction("Index");
        }

        //     KODE CONTROLLER MENGHAPUS SEMUA DATA TB_Inventory    //

        [HttpPost]
        public ActionResult DeleteAllData()
        {
            try
            {
                using (var context = new CyclecountsystemEntities())
                {
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE TB_Inventory");
                }

                TempData["SuccessMessage"] = "Data inventory berhasil dihapus.";

                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan saat menghapus data.";
                return RedirectToAction("Error");
            }
        }


        //     KODE CONTROLLER SEND EMAIL    //
        [HttpPost]
        public ActionResult Notifikasiemail(CombineViewModel model)
        {
            // Ambil daftar email dari TB_User
            var emailAddresses = model.EmailAddresses; // Menggunakan emailAddresses dari model

            // Ambil daftar name dari TB_User
            var userNames = model.UserNames; // Menggunakan userNames dari model

            // Menambahkan variabel baru untuk data yang akan digunakan dalam Alarm template
            DateTime dateCount = DateTime.Now;

            // Send Email 
            if (emailAddresses != null)
            {
                var selectedEmailList = emailAddresses;
                string emailTemplate = RenderPartialToString("AlarmTamplate", model); // Menggunakan model sebagai parameter untuk AlarmTemplate

                SendAlarm mailSender = new SendAlarm();

                // Ambil daftar nama yang sesuai dengan alamat email yang ada dalam emailAddresses
                var nameList = db.TB_Email.Where(user => emailAddresses.Contains(user.Email))
                                        .Select(user => user.Name)
                                        .ToList();

                // Pastikan daftar nama dan daftar email memiliki jumlah yang sama
                if (nameList.Count == selectedEmailList.Count)
                {
                    for (int i = 0; i < selectedEmailList.Count; i++)
                    {
                        mailSender.SendAlarmToSuperior(emailTemplate, nameList[i], selectedEmailList[i]);
                    }
                }
                else
                {
                    // Handle kesalahan jika jumlah nama dan email tidak sesuai
                }
            }

            return RedirectToAction("Index");
        }

        //     KODE CONTROLLER MENGHUBUNGKAN DENGAN EMAIL TAMPLATE   //

        public string RenderPartialToString(string viewName, object model)
        {
            var controller = this;
            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
