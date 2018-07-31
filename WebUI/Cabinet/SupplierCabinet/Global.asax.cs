using AuthorizationApp;
using AuthorizationApp.Services;
using SupplierCabinet.App_Start;
using System;
using System.Web.Optimization;
using System.Web.Routing;

namespace SupplierCabinet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string URL_APPLICATION = "PathServerApplication";

        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
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
