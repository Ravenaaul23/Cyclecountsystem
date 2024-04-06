using CycleCountSystem__CSS_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CycleCountSystem__CSS_.LoginSecurity;
using System.Web.Security;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace CycleCountSystem__CSS_.Controllers
{
    public class UserController : Controller
    {
        //     KODE CONTROLLER MEMBUAT ALAMAT EMAIL USER    //
        private CyclecountsystemEntities db = new CyclecountsystemEntities();
        // GET: User
        [AuthorizationHandlerAttribute(Roles = "0")]
        public ActionResult Index()
        {
            var roleUser = db.TB_User.ToList();
            var data = new CombineViewModel
            {
                ListUser = roleUser,
                UserModel = new Models.TB_User()
            };
            return View(data);
        }

        // GET: User/Create
        [AuthorizationHandlerAttribute(Roles = "0")]
        public ActionResult Create()
        {
            var model = new CombineViewModel();
            model.UserModel = new TB_User();
            model.ListUser = db.TB_User.ToList();
            return View(model);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizationHandlerAttribute(Roles = "0")]
        public ActionResult Create(CombineViewModel data)
        {
            if (ModelState.IsValid)
            {
                db.TB_User.Add(data.UserModel);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            data.ListUser = db.TB_User.ToList();
            return View("Create", data);
        }

        // GET: Inventory/Edit/5
        [AuthorizationHandlerAttribute(Roles = "0")]
        public ActionResult Edit(int id)
        {
            var dataUser = db.TB_User.FirstOrDefault(c => c.Id_user == id);
            if(dataUser != null)
            {
                var user = new CombineViewModel
                {
                    UserModel = dataUser,
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
        [AuthorizationHandlerAttribute(Roles = "0")]
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
                    var user = db.TB_User.FirstOrDefault(c => c.Id_user == model.UserModel.Id_user);
                    if(user != null)
                    {
                        user.Email = model.UserModel.Email;
                        user.Name = model.UserModel.Name;
                        user.Windows_account = model.UserModel.Windows_account;

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

        // GET: Inventory/Delete/5
        [AuthorizationHandlerAttribute(Roles = "0")]
        public ActionResult Delete(int id)
        {
            return View(db.TB_User.Where(x => x.Id_user == id).FirstOrDefault());
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        [AuthorizationHandlerAttribute(Roles = "0")]
        public ActionResult Delete(int id, TB_User user)
        {
            try
            {
                // TODO: Add delete logic here
                user = db.TB_User.Where(x => x.Id_user == id).FirstOrDefault();
                db.TB_User.Remove(user);
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
