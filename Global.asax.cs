using CycleCountSystem__CSS_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OfficeOpenXml;


namespace CycleCountSystem__CSS_
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Data.Entity.Database.SetInitializer<CyclecountsystemEntities>(null); // Ganti YourDbContext dengan nama DbContext Anda
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
    }
}
