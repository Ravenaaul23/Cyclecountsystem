using CycleCountSystem__CSS_.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CycleCountSystem__CSS_.LoginSecurity;
using System.Web.Security;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System;
using System.Web.Helpers;

namespace CycleCountSystem__CSS_.Controllers
{
    public class EmailController : Controller
    {

        //     KODE CONTROLLER MEMBUAT ALAMAT EMAIL USER    //
        private CyclecountsystemEntities db = new CyclecountsystemEntities();
        [AuthorizationHandlerAttribute(Roles = "1")]
        // GET: User
         public ActionResult Index()
        {
            var DataUser = db.TB_Email.ToList();
            var data = new CombineViewModel
            {
                ListEmail = DataUser,
                ModelEmail = new Models.TB_Email()
            };
            return View(data);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            var model = new CombineViewModel();
            model.ModelEmail = new TB_Email();
            model.ListEmail = db.TB_Email.ToList();
            return View(model);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CombineViewModel data)
        {
            if (ModelState.IsValid)
            {
                db.TB_Email.Add(data.ModelEmail);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            data.ListEmail = db.TB_Email.ToList();
            return View("Create", data);
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(int id)
        {
            var dataEmail = db.TB_Email.FirstOrDefault(x => x.Id_email == id);
            if (dataEmail != null)
            {
                var email = new CombineViewModel
                {
                    ModelEmail = dataEmail,
                };
                return View(email);
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
                    var dataEmail = db.TB_Email.FirstOrDefault(x => x.Id_email == id);

                    if (dataEmail != null)
                    {
                        // Update properti entitas TB_Email dengan nilai yang diberikan dalam model CombineViewModel
                        dataEmail.Name = model.ModelEmail.Name;
                        dataEmail.Email = model.ModelEmail.Email;

                        // Set status entitas menjadi Modified agar perubahan dikenali oleh DbContext
                        db.Entry(dataEmail).State = EntityState.Modified;

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
            var dataEmail = db.TB_Email.FirstOrDefault(x => x.Id_email == id);
            if (dataEmail != null)
            {
                var email = new CombineViewModel
                {
                    ModelEmail = dataEmail,
                };
                return View(email);
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
                var dataEmail = db.TB_Email.FirstOrDefault(x => x.Id_email == id);
                if (dataEmail != null)
                    db.TB_Email.Remove(dataEmail);

                db.SaveChanges();
                return RedirectToAction("Index"); // Redirect kembali ke tampilan Index setelah berhasil menghapus data
            }
            catch (Exception)
            {
                // Handle Error if this failed to deleted
                return RedirectToAction("Index"); // Anda dapat memilih tindakan yang sesuai jika terjadi kesalahan
            }
        }
    }
}