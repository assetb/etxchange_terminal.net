using AltaLog;
using AuthorizationApp;
using AuthorizationApp.Services;
using Microsoft.Practices.Unity;
using ServerApp.App_Start;
using System;
using System.Diagnostics;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Services.Description;

namespace ServerApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public const string SUB_SYSTEM_PYADA_TRADING = "payda trading";

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(UnityConfig.Register);

            AppJournal.logFileName = Server.MapPath("~/App_Data/serverApp.log");
            MappingConfig.Register();
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            string login;
            if (Boolean.TrueString.Equals(WebConfigurationManager.AppSettings.Get("IsDebug")))
            {
                login = WebConfigurationManager.AppSettings.Get("UserDebug");
            }
            else
            {
                login = Context.GetUserNameFromCookie(Request);
            }

            if (!string.IsNullOrEmpty(login))
            {
                Context.User = new AltaUserProvider(login, UnityConfig.GetConfiguredContainer().Resolve<IAltaUserRepository>(UnityConfig.BROKER_BASE));
                if (Context.User.IsInRole(SUB_SYSTEM_PYADA_TRADING))
                {
                    Context.User = new AltaUserProvider(login, UnityConfig.GetConfiguredContainer().Resolve<IAltaUserRepository>(UnityConfig.SUB_SYSTEM_PAYDA_TRADE));
                }
            }
        }

        public void ConfigureServices(ServiceCollection services)
        {
            Debug.WriteLine("ConfigurationServices");
        }
    }
}
