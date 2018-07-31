using System.Web.Mvc;

namespace KazMineralsServiceCabinet.Controllers
{
    public class AuctionsController : Controller
    {

        public AuctionsController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
