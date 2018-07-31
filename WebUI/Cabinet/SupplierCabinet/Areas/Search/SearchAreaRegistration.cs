using System.Web.Mvc;

namespace PersonalCabinetSupplier.Areas.Search
{
    public class SearchAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Search";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Search_default",
                "Search/{controller}/{action}/{id}",
                new { controller = "Supplier", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}