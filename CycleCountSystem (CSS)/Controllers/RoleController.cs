using CycleCountSystem__CSS_.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CycleCountSystem__CSS_.Controllers
{
    public class RoleController : Controller
    {
        private CyclecountsystemEntities db = new CyclecountsystemEntities();

        // GET: Role
        public ActionResult Index()
        {
            var DataRole = db.TB_Role.ToList();
            var data = new CombineViewModel
            {
                ListRole = DataRole,
                ModelRole = new Models.TB_Role()
            };
            return View(data);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            var model = new CombineViewModel();
            model.ModelRole = new TB_Role();
            model.ListRole = db.TB_Role.ToList();
            return View(model);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CombineViewModel data)
        {
            if (ModelState.IsValid)
            {
                db.TB_Role.Add(data.ModelRole);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            data.ListRole = db.TB_Role.ToList();
            return View("Create", data);
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(int id)
        {
            var dataRole = db.TB_Role.FirstOrDefault(x => x.Id_role == id);
            if (dataRole != null)
            {
                var role = new CombineViewModel
                {
                    ModelRole = dataRole,
                };
                return View(role);
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
                    var dataRole = db.TB_Role.FirstOrDefault(x => x.Id_role == id);

                    if (dataRole != null)
                    {
                        // Update properti entitas TB_Role dengan nilai yang diberikan dalam model CombineViewModel
                        dataRole.role = model.ModelRole.role;

                        // Set status entitas menjadi Modified agar perubahan dikenali oleh DbContext
                        db.Entry(dataRole).State = EntityState.Modified;

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
            var dataRole = db.TB_Role.FirstOrDefault(x => x.Id_role == id);
            if (dataRole != null)
            {
                var role = new CombineViewModel
                {
                    ModelRole = dataRole,
                };
                return View(role);
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
                var dataRole = db.TB_Role.FirstOrDefault(x => x.Id_role == id);
                if (dataRole != null)
                    db.TB_Role.Remove(dataRole);

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