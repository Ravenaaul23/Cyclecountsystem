using CycleCountSystem__CSS_.Models;
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

            if (GeneralContants.ALL_ACCESS != true)
            {
                var currUser = HttpContext.Current.User.Identity.Name.ToLower();
                var haveAcess = db.TB_Role.FirstOrDefault(x => x.windows_account.ToLower() == currUser);
                if (haveAcess != null)
                {
                    var myAccess = haveAcess.role;
                    var currentRoles = Roles.Split(',').ToList();
                    if (currentRoles.Contains(myAccess))
                    {
                        authorized = true;
                    }
                }
            }
            return authorized;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult { ViewName = "AccessDenied" };
        }
    }
}