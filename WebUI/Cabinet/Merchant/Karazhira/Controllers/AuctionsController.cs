using System.Web.Mvc;

namespace Karazhira.Controllers
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
