using System.Web.Mvc;

namespace SupplierCabinet.Controllers
{
    [RoutePrefix("Order")]
    public class OrderController : Controller
    {
        [HttpGet, Route("{auctionId}")]
        public ActionResult SupplierOrder(int auctionId)
        {
            return View(auctionId);
        }
    }
}