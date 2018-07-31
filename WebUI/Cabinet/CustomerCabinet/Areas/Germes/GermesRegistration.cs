using System.Web.Http;
using System.Web.Mvc;


namespace CustomerCabinet.Areas.Germes
{
    public class GermesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Germes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Germes_Default",
                "Germes/{action}/{apiId}",
                new { controller = "Order", action = "Index", apiId = UrlParameter.Optional });

            //Or.Register(GlobalConfiguration.Configuration);
        }
    }
}