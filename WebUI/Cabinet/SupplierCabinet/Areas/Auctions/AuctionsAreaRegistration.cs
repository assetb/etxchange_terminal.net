using System.Web.Mvc;

namespace PersonalCabinetSupplier.Areas.Auctions
{
    public class AuctionsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Auctions";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Auctions_default",
                "Auctions/{controller}/{action}/{id}",
                new { controller = "Auction", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}