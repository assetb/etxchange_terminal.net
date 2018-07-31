using AuthorizationApp;
using AuthorizationApp.Services;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InkaiCabinet
{
    public class MvcApplication : HttpApplication
    {
        public static string URL_APPLICATION = "PathServerApplication";
        public static string URL_CUSTOMER_CABINET = "InkaiCabinet";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var userName = Context.GetUserNameFromCookie(Request);
            if (!string.IsNullOrEmpty(userName))
            {
                Context.User = new AltaUserProvider(userName);
            }
        }
    }
}
