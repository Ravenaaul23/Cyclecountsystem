using CycleCountSystem__CSS_.LoginSecurity;
using CycleCountSystem__CSS_.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace CycleCountSystem__CSS_.Controllers
{
    public class AkunController : Controller
    {
        private CyclecountsystemEntities db = new CyclecountsystemEntities();
        [AuthorizationHandlerAttribute(Roles = "1,2")]

        // GET: Role
        public ActionResult Index()
        {
            var DataAkun = db.TB_Akun.ToList();
            var data = new CombineViewModel
            {
                ListAkun = DataAkun,
                ModelAkun = new Models.TB_Akun()
            };
            return View(data);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            var model = new CombineViewModel();
            model.ModelAkun = new TB_Akun();
            model.ListAkun = db.TB_Akun.ToList();
            return View(model);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CombineViewModel data)
        {
            if (ModelState.IsValid)
            {
                db.TB_Akun.Add(data.ModelAkun);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            data.ListAkun = db.TB_Akun.ToList();
            return View("Create", data);
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(int id)
        {
            var dataAkun = db.TB_Akun.FirstOrDefault(x => x.Id_akun == id);
            if (dataAkun != null)
            {
                var akun = new CombineViewModel
                {
                    ModelAkun = dataAkun,
                };
                return View(akun);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CombineViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Temukan data yang akan diedit berdasarkan ID
                    var dataAkun = db.TB_Akun.FirstOrDefault(x => x.Id_akun == id);

                    if (dataAkun != null)
                    {
                        // Update properti entitas TB_Akun dengan nilai yang diberikan dalam model CombineViewModel
                        dataAkun.windows_account = model.ModelAkun.windows_account;
                        dataAkun.Id_role = model.ModelAkun.Id_role;

                        // Set status entitas menjadi Modified agar perubahan dikenali oleh DbContext
                        db.Entry(dataAkun).State = EntityState.Modified;

                        // Simpan perubahan ke database
                        db.SaveChanges();
                    }
                    else
                    {
                        // Handle jika data tidak ditemukan
                        return HttpNotFound();
                    }

                    // Redirect kembali ke tampilan Index setelah berhasil mengedit data
                    return RedirectToAction("Index");
                }
                else
                {
                    // Jika model tidak valid, kembalikan tampilan edit dengan model yang sama
                    return View(model);
                }
            }
            catch (Exception)
            {
                // Handle error jika gagal menyimpan perubahan ke database
                return RedirectToAction("Index");
            }
        }

        // GET: Inventory/Delete/5
        public ActionResult Delete(int id)
        {
            var dataAkun = db.TB_Akun.FirstOrDefault(x => x.Id_akun == id);
            if (dataAkun != null)
            {
                var akun = new CombineViewModel
                {
                    ModelAkun = dataAkun,
                };
                return View(akun);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, CombineViewModel data)
        {
            try
            {
                // Temukan data yang akan dihapus dari database
                var dataAkun = db.TB_Akun.FirstOrDefault(x => x.Id_akun == id);
                if (dataAkun != null)
                    db.TB_Akun.Remove(dataAkun);

                db.SaveChanges();
                return RedirectToAction("Index"); // Redirect kembali ke tampilan Index setelah berhasil menghapus data
            }
            catch (Exception)
            {
                // Handle Error if this failed to deleted
                return RedirectToAction("Index"); // Anda dapat memilih tindakan yang sesuai jika terjadi kesalahan
            }
        }

        public ActionResult UnauthorizedAccess()
        {
            return View();
        }
    }
}
