using CycleCountSystem__CSS_.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CycleCountSystem__CSS_.App_Constant;

namespace CycleCountSystem__CSS_.LoginSecurity
{
    public class AuthorizationHandlerAttribute : AuthorizeAttribute
    {
        private CyclecountsystemEntities db = new CyclecountsystemEntities();

        public AuthorizationHandlerAttribute()
        {
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var routeData = httpContext.Request.RequestContext.RouteData;
            bool authorized = false;

            // Check if user has all access
            if (GeneralContants.ALL_ACCESS != true)
            {
                var currUser = httpContext.User.Identity.Name.ToLower();
                var haveAccess = db.TB_Akun.FirstOrDefault(x => x.windows_account.ToLower() == currUser);
                if (haveAccess != null)
                {
                    var myAccess = haveAccess.Id_role;
                    var currentRoles = Roles.Split(',').Select(int.Parse).ToList();
                    if (myAccess.HasValue && currentRoles.Contains(myAccess.Value))
                    {
                        authorized = true;
                    }
                }
            }

            return authorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult { ViewName = "UnauthorizedAccess" };
        }

        // Method to refresh user's identity
        private void RefreshUserIdentity(string userName)
        {
            var user = db.TB_Akun.FirstOrDefault(x => x.windows_account.ToLower() == userName.ToLower());
            if (user != null)
            {
                var roles = user.Id_role.ToString();
                var identity = new System.Security.Principal.GenericIdentity(userName, "Forms");
                var principal = new System.Security.Principal.GenericPrincipal(identity, roles.Split(','));
                HttpContext.Current.User = principal;
            }
        }
    }
}
