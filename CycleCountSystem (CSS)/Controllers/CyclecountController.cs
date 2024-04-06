using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using Rotativa;
using CycleCountSystem__CSS_.Helper;
using CycleCountSystem__CSS_.LoginSecurity;
using CycleCountSystem__CSS_.Models;

namespace CycleCountSystem_CSS.Controllers
{
    public class CyclecountController : Controller
    {
        private CyclecountsystemEntities db = new CyclecountsystemEntities();

        //     KODE CONTROLLER SEND EMAIL    //
        [HttpPost]
        [AuthorizationHandlerAttribute(Roles = "2")]
        public ActionResult SubmitEmail(CombineViewModel model)
        {
            // Ambil daftar email, user dan kalkulasi terakhir dari TB_User
            var emailAddresses = db.TB_Email.Select(user => user.Email).ToList();
            var userNames = db.TB_Email.Select(user => user.Name).ToList();
            var lastCalculation = db.TB_Cyclecount.OrderByDescending(c => c.Id_Cyclecount).FirstOrDefault();

            // Menambahkan variabel baru untuk data yang akan digunakan dalam email template
            DateTime dateCount = DateTime.Now;
            string lastCalculationId = null;
            string lastCalculationBatch = null;
            string lastCalculationBin = null;
            string lastCalculationPercent = null;
            TB_Inventory lastCalculationInventory = null;
            string selisihQty = null;
            decimal lastCalculationQty = 0;

            string qtyString = "Data tidak tersedia";

            if (lastCalculation != null)
            {
                dateCount = lastCalculation.Date_count ?? DateTime.Now;
                lastCalculationId = lastCalculation.Id_Material;
                lastCalculationBatch = lastCalculation.Batch_no;
                lastCalculationBin = lastCalculation.bin;
                lastCalculationPercent = lastCalculation.Persentase;

                // Ambil data inventory terbaru sesuai inputan Id_Material, Batch_number, dan Bin pada kalkulasi terakhir
                lastCalculationInventory = db.TB_Inventory
                    .FirstOrDefault(inv =>
                        inv.Id_material == lastCalculation.Id_Material &&
                        inv.Batch_number == lastCalculation.Batch_no &&
                        inv.Bin == lastCalculation.bin);

                qtyString = lastCalculationInventory != null ? Convert.ToDecimal(lastCalculationInventory.Qty).ToString("#,##0") : "Data tidak tersedia";

                lastCalculationQty = decimal.TryParse(lastCalculation.Data_aktual, out decimal qty) ? qty : 0;
                selisihQty = lastCalculation.SelisihQty;
            }

            var viewModel = new CombineViewModel
            {
                CycleCounts = db.TB_Cyclecount.ToList(),
                EmailAddresses = emailAddresses,
                LastCalculationId = lastCalculationId,
                LastCalculationPercent = lastCalculationPercent,
                LastCalculationBatch = lastCalculationBatch,
                LastCalculationBin = lastCalculationBin,
                UserNames = userNames,
                DateCount = dateCount,
                LastCalculationInventoryQty = qtyString,
                DataAktual = lastCalculationQty,
                SelisihQty = selisihQty
            };

            // Konversi DataAktual ke string dengan format "#,##0"
            viewModel.DataAktualString = viewModel.DataAktual.ToString("#,##0");

            // Send Email 
            if (emailAddresses != null)
            {
                var selectedEmailList = emailAddresses;
                string emailTemplate = RenderPartialToString("EmailTamplate", viewModel);

                SendMail mailSender = new SendMail();

                var nameList = db.TB_Email.Where(user => emailAddresses.Contains(user.Email))
                                        .Select(user => user.Name)
                                        .ToList();

                // Pastikan daftar nama dan daftar email memiliki jumlah yang sama
                if (nameList.Count == selectedEmailList.Count)
                {
                    for (int i = 0; i < selectedEmailList.Count; i++)
                    {
                        mailSender.SendMailToSuperior(emailTemplate, nameList[i], selectedEmailList[i]);
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

        public ActionResult Index(CombineViewModel model)
        {
            // Ambil daftar email dari TB_User
            var emailAddresses = db.TB_Email.Select(user => user.Email).ToList();

            // Ambil daftar name dari TB_User
            var userNames = db.TB_Email.Select(user => user.Name).ToList();

            // Ambil data kalkulasi terbaru (jika ada)
            var lastCalculation = db.TB_Cyclecount.OrderByDescending(c => c.Id_Cyclecount).FirstOrDefault();

            // Inisialisasi nilai yang akan digunakan di ViewModel
            decimal lastCalculationQty = 0;
            string tandaSelisih = "+";
            string lastCalculationPercent = "0.00";
            decimal selisihQty = 0; // Deklarasikan selisihQty di sini
            DateTime dateCount = DateTime.Now;
            TB_Inventory lastCalculationInventory = null; // Deklarasikan lastCalculationInventory di sini

            if (lastCalculation != null)
            {
                lastCalculationQty = decimal.TryParse(lastCalculation.Data_aktual, out decimal qty) ? qty : 0;

                // Ambil data inventory terbaru sesuai inputan Id_Material, Batch_number, dan Bin pada kalkulasi terakhir
                lastCalculationInventory = db.TB_Inventory
                    .FirstOrDefault(inv =>
                        inv.Id_material == lastCalculation.Id_Material &&
                        inv.Batch_number == lastCalculation.Batch_no &&
                        inv.Bin == lastCalculation.bin);

                // Hitung selisih Qty
                selisihQty = lastCalculationQty - (decimal.TryParse(lastCalculationInventory?.Qty, out decimal qtyInventory) ? qtyInventory : 0m);

                // Tentukan tanda selisih
                tandaSelisih = selisihQty >= 0 ? "+" : "-";

                dateCount = lastCalculation.Date_count ?? DateTime.Now;

                // Hitung persentase
                decimal percentase = (
                      decimal.TryParse(lastCalculationInventory?.Qty, out decimal qtyPercent) ? qtyPercent : 0m
                  ) / lastCalculationQty * 100;
                lastCalculationPercent = percentase.ToString("0.00");
            }

            // Konversi Qty dari lastCalculationInventory ke string dengan format "#,##0"
            string qtyString = lastCalculationInventory != null ? Convert.ToDecimal(lastCalculationInventory.Qty).ToString("#,##0") : "Data tidak tersedia";


            var viewModel = new CombineViewModel
            {
                CycleCounts = db.TB_Cyclecount.ToList(),
                Inventories = db.TB_Inventory.ToList(),
                EmailAddresses = db.TB_Email.Select(user => user.Email).ToList(),
                LastCalculationId = lastCalculation?.Id_Material,
                LastCalculationBatch = lastCalculation?.Batch_no,
                LastCalculationBin = lastCalculation?.bin,
                LastCalculationPercent = lastCalculationPercent,
                LastCalculationInventory = lastCalculationInventory,
                DataAktual = lastCalculationQty,
                SelisihQty = $"{tandaSelisih}{Math.Abs(selisihQty):0.00}",
                DateCount = dateCount,
                LastCalculationInventoryQty = qtyString,
            };

            // Konversi DataAktual ke string dengan format "#,##0"
            viewModel.DataAktualString = viewModel.DataAktual.ToString("#,##0");

            return View(viewModel);
        }

        //     KODE CONTROLLER MEMBUAT KALKULASI PERHITUNGAN    //
        // GET: Cyclecount/Create
        [AuthorizationHandlerAttribute(Roles = "2")]
        public ActionResult Create(int id)
        {
            // Mengambil data inventaris yang sesuai dengan id_inventory yang dipilih
            var inventory = db.TB_Inventory.FirstOrDefault(x => x.Id_Inventory == id);
            if (inventory == null)
            {
                ModelState.AddModelError("", "Data inventaris tidak ditemukan");
                return RedirectToAction("Index", "Cyclecount");
            }

            // Menginisialisasi model CombineViewModel dengan data inventaris yang ditemukan
            var viewModel = new CombineViewModel
            {
                Inventories = new List<TB_Inventory> { inventory },
                DataAktual = 0 // Set nilai awal DataAktual menjadi 0
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizationHandlerAttribute(Roles = "2")]
        public ActionResult Create(CombineViewModel model, string Data_aktual, int id)
        {
            // Mengambil data inventaris dari ModelState
            var inventory = db.TB_Inventory.FirstOrDefault(x => x.Id_Inventory == id);
            if (inventory == null)
            {
                ModelState.AddModelError("", "Data inventaris tidak ditemukan");
                return View(model);
            }

            // Validasi nilai Data_aktual
            if (!decimal.TryParse(Data_aktual, out decimal dataAktualDecimal) || dataAktualDecimal < 0)
            {
                ModelState.AddModelError("", "Data tidak valid");
                return View(model);
            }

            // Menghitung selisih dan persentase
            var qty = decimal.TryParse(inventory.Qty, out decimal qtyDecimal) ? qtyDecimal : 0;
            decimal selisihQty = dataAktualDecimal - qty; // Hitung selisih qty
            string tandaSelisih = selisihQty >= 0 ? "+" : "-"; // Tentukan tanda selisih
            decimal persentase = (dataAktualDecimal / qty) * 100; // Hitung persentase

            //Mendapatkan user windows yang kalkulasi 
            string uploadedByUsername = User.Identity.Name;

            int? idAkun = db.TB_Akun
                .Where(akun => akun.windows_account == uploadedByUsername)
                .Select(akun => akun.Id_akun)
                .FirstOrDefault();

            // Buat entri baru di TB_Cyclecount
            var cyclecountEntry = new TB_Cyclecount
            {
                Id_Material = inventory.Id_material,
                Batch_no = inventory.Batch_number,
                bin = inventory.Bin,
                Data_aktual = dataAktualDecimal.ToString(),
                SelisihQty = $"{tandaSelisih}{Math.Abs(selisihQty):0.00}", // Tambahkan tanda dan format hasil selisihQty
                Persentase = persentase.ToString("0.00"), // Tambahkan persentase dengan format dua desimal
                Date_count = DateTime.Now, // Tambahkan tanggal kalkulasi
                Id_akun = idAkun
            };
      
            db.TB_Cyclecount.Add(cyclecountEntry);

            // Simpan perubahan ke database
            db.SaveChanges();

            // Perbarui nilai kolom Calculate di TB_Inventory
            if (persentase == 100.00m) 
            {
                inventory.Calculate = true ;
            }
            else
            {
                inventory.Calculate = false;
            }

            // Jika kalkulasi gagal, langsung panggil SubmitEmail
            if (persentase != 100.00m) 
            {
                SubmitEmail(new CombineViewModel());
            };

            db.SaveChanges();

            // Kirim model ke view
            return RedirectToAction("Index", "Cyclecount");
        }


        //     KODE CONTROLLER MENGAKSES ALAMAT EMAIL    //

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // GET: Data List TB_Cyclecount dan bisa di ubah
        [AuthorizationHandlerAttribute(Roles = "1")]
        public ActionResult Indexadmin()
        {
            // Ambil data kalkulasi terbaru (jika ada)
            var lastCalculation = db.TB_Cyclecount.OrderByDescending(c => c.Id_Cyclecount).FirstOrDefault();

            //Ambil data Qty di sistem berdasarkan inputan 

            string lastCalculationId = null;
            string lastCalculationPercent = null;

            if (lastCalculation != null)
            {
                lastCalculationId = lastCalculation.Id_Material;
                lastCalculationPercent = lastCalculation.Persentase;
            }

            // Ambil data inventory terbaru sesuai inputan Id_Material, Batch_number, dan Bin pada kalkulasi terakhir
            var lastCalculationInventory = db.TB_Inventory
                .FirstOrDefault(inv =>
                    inv.Id_material == lastCalculationId &&
                    inv.Batch_number == lastCalculation.Batch_no &&
                    inv.Bin == lastCalculation.bin);

            var viewModel = new CombineViewModel
            {
                CycleCounts = db.TB_Cyclecount.ToList(),
                Inventories = db.TB_Inventory.ToList(),
                LastCalculationId = lastCalculationId,
                LastCalculationPercent = lastCalculationPercent,
                LastCalculationInventory = lastCalculationInventory
            };

            return View(viewModel);
        }

        // GET: Cyclecount/Delete/5
        [AuthorizationHandlerAttribute(Roles = "1")]
        public ActionResult Delete(int id)
        {
            return View(db.TB_Cyclecount.Where(x => x.Id_Cyclecount == id).FirstOrDefault());
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        [AuthorizationHandlerAttribute(Roles = "1")]
        public ActionResult Delete(int id, TB_Cyclecount cyclecount)
        {
            try
            {
                // TODO: Add delete logic here
                cyclecount = db.TB_Cyclecount.Where(x => x.Id_Cyclecount == id).FirstOrDefault();
                db.TB_Cyclecount.Remove(cyclecount);
                db.SaveChanges();
                return RedirectToAction("Indexadmin");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DownloadPDF(DateTime startDate, DateTime endDate)
        {
            //Ambil semua data pada TB_Cyclecount
            var viewModel = new CombineViewModel
            {
                CycleCounts = db.TB_Cyclecount.ToList(),
                Inventories = db.TB_Inventory.ToList(),
            };

            // Pastikan Model tidak null dan CycleCounts tidak null sebelum menggunakan
            if (viewModel != null && viewModel.CycleCounts != null)
            {
                // Filter data berdasarkan rentang tanggal
                //AddDays(1) memastikan tng yang sama dengan endDate masuk ke dalam hasil yang di filter
                var filteredCycleCounts = viewModel.CycleCounts.Where(x => x.Date_count >= startDate.Date && x.Date_count < endDate.Date.AddDays(1)).Reverse();

                // Ambil data yang sudah di filter 
                var data = new CombineViewModel
                {
                    CycleCounts = filteredCycleCounts.ToList(),
                    Inventories = viewModel.Inventories
                };

                // Render view ke string HTML
                var html = this.RenderViewToString("DownloadPDF", data);

                // Konversi HTML ke PDF menggunakan Rotativa
                var pdfBytes = new ViewAsPdf("DownloadPDF", data).BuildFile(this.ControllerContext);

                return File(pdfBytes, "application/pdf", "CycleCountReport.pdf");
            }
            else
            {
                // Handle case where Model or Model.CycleCounts is null
                // Return appropriate response or handle the error
                // For example:
                return RedirectToAction("Error", "Home");
            }
        }

        // Helper method untuk merender view ke string HTML
        protected string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(this.ControllerContext, viewName, null);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}