using EtsApp;
using Microsoft.Practices.Unity;
using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace ServerApp.App_Start
{
    public class UnityConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();
            var etsConfigFile = WebConfigurationManager.AppSettings["ets.config"];

            var pathToInitConfig = HttpContext.Current.Server.MapPath(String.Format("~/App_Data/{0}", etsConfigFile));

            container.RegisterType<IEtsApi, EtsApi>(new ContainerControlledLifetimeManager(), new InjectionConstructor(pathToInitConfig));
            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}