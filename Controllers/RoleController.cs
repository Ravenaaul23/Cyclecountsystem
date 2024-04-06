using CycleCountSystem__CSS_.LoginSecurity;
using CycleCountSystem__CSS_.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CycleCountSystem__CSS_.Controllers
{
    //[AuthorizationHandlerAttribute(Roles = "0")]
    public class RoleController : Controller
    {
        private CyclecountsystemEntities db = new CyclecountsystemEntities();
        // GET: Role
        public ActionResult Index()
        {
            var roleUser = db.TB_Role.ToList();
            var data = new CombineViewModel
            {
                ListRole = roleUser,
                ModelRole = new Models.TB_Role()
            };
            return View(data);
        }

        // GET: Role/Details/5
        public ActionResult Details(int id)
        {
            return View(db.TB_Role.Where(x => x.Id_role == id).FirstOrDefault());
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
            var dataUser = db.TB_Role.FirstOrDefault(c => c.Id_role == id);
            if (dataUser != null)
            {
                var user = new CombineViewModel
                {
                    ModelRole = dataUser,
                };
                return View(user);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        public ActionResult Edit(CombineViewModel model)
        {
            try
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine("Error: " + error.ErrorMessage);
                    }
                }

                if (ModelState.IsValid)
                {
                    var user = db.TB_Role.FirstOrDefault(c => c.Id_role == model.ModelRole.Id_role);
                    if (user != null)
                    {
                        user.windows_account = model.ModelRole.windows_account;
                        user.role = model.ModelRole.role;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                while (innerException != null && !(innerException is SqlException))
                {
                    innerException = innerException.InnerException;
                }

                if (innerException is SqlException sqlException)
                {
                    // Handle kesalahan SQL di sini, misalnya, mencetak pesan kesalahan.
                    Console.WriteLine("SQL Error: " + sqlException.Message);
                }
                else
                {
                    // Handle kesalahan lain yang mungkin terjadi saat menyimpan perubahan.
                    Console.WriteLine("Error: " + ex.Message);
                }

                // Redirect atau tampilkan tampilan pesan kesalahan yang sesuai.
                return View("ErrorView");
            }
        }

        // GET: Role/Delete/5
        public ActionResult Delete(int id)
        {
            return View(db.TB_Role.Where(x => x.Id_role == id).FirstOrDefault());
        }

        // POST: Role/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, TB_Role Role)
        {
            try
            {
                // TODO: Add delete logic here
                Role = db.TB_Role.Where(x => x.Id_role == id).FirstOrDefault();
                db.TB_Role.Remove(Role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
