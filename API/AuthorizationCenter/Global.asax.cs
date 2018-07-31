using AuthorizationApp;
using AuthorizationApp.Services;
using AuthorizationCenter.App_Start;
using Microsoft.Practices.Unity;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AuthorizationCenter
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            UnityWebActivator.Start();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(UnityApiActivator.Register);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            WebMvcConfig.Registration(RouteTable.Routes);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var userName = Context.GetUserNameFromCookie(Request);
            if (!string.IsNullOrEmpty(userName))
            {
                Context.User = new AltaUserProvider(userName, UnityConfig.GetConfiguredContainer().Resolve<IAltaUserRepository>());
            }
        }

        protected void Application_End()
        {
            UnityWebActivator.Shutdown();
        }

    }
}
