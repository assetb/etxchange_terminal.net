using System.Collections.Generic;
using System.Web.Mvc;

namespace SupplierCabinet.Controllers
{
    [RoutePrefix("Auctions")]
    public class AuctionsController : Controller
    {

        public ActionResult Index(int? id)
        {
            if (id == null || id == 0)
            {
                return View("ListActiveAuctionsView");
            }
            else
            {
                return View("AuctionDetailsView", id);
            }
        }

        [HttpGet]
        public ActionResult Played(int? id)
        {
            if (id == null)
            {
                return View("ListPastAuctionsView");
            }
            else
            {
                return View("AuctionDetailsView", id);
            }
        }

        [HttpGet, Route("{auctionId}/Procuratory")]
        public ActionResult Procuratory(int auctionId)
        {
            return View(auctionId);
        }

        [HttpGet, Route("{auctionId}/TechSpecForm/{lotId}")]
        public ActionResult TechSpecForm(int auctionId, int lotId)
        {
            var model = new Dictionary<string, int>();
            model.Add("auctionId", auctionId);
            model.Add("lotId", lotId);
            return View(model);
        }
    }
}