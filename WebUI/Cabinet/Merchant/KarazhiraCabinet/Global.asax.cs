using AuthorizationApp;
using AuthorizationApp.Services;
using KarazhiraCabinet.Services;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KarazhiraCabinet
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Utilites.NetworkDrive nd = new Utilites.NetworkDrive();
            nd.MapNetworkDrive(@"\\192.168.11.5\Archive", "Y:", "alta.net", "!KOR#Rjh");
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var userName = Context.GetUserNameFromCookie(Request);
            if (!string.IsNullOrEmpty(userName))
            {
                Context.User = new AltaUserProvider(userName);
            }
        }
    }
}
