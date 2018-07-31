using System.Web.Http;
using Unity.WebApi;

namespace AuthorizationCenter.App_Start
{
    public static class UnityApiActivator
    {
        public static void Register(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
        }
    }
}