using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace AuthorizationCenter.App_Start
{
    public static class WebMvcConfig
    {
        public static void Registration(RouteCollection routes) {
            routes.MapRoute(
                name: "MVC",
                 url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = RouteParameter.Optional }
            );
        }
    }
}