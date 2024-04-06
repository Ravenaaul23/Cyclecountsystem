//using CycleCountSystem__CSS_.Models;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
//using System;

//namespace CycleCountSystem__CSS_.LoginAutho
//{
//    public class AppicationUserManager
//    {
//        public class ApplicationUserManager : UserManager<ApplicationUser>
//        {
//            public ApplicationUserManager(IUserStore<ApplicationUser> store)
//                : base(store)
//            {
//            }

//            public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
//            {
//                var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<CyclecountsystemEntities>()));
//                // Konfigurasi manajer pengguna
////                return manager;
////            }
////        }
//    }
//}