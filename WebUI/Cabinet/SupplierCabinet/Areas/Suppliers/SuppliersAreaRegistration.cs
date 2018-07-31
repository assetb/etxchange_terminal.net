using System.Web.Mvc;

namespace PersonalCabinetSupplier.Areas.Suppliers
{
    public class SuppliersAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Suppliers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Suppliers_default",
                "Suppliers/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}