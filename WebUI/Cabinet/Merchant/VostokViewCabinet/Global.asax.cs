using AuthorizationCenter.services;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace VostokViewCabinet
{
    public class MvcApplication : HttpApplication
    {
        public static string URL_APPLICATION = "PathServerApplication";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                AuthContext.RegistryTokenInContext(authCookie, HttpContext.Current);
            }
        }
    }
}
