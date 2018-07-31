using System.Web.Mvc;

namespace KazMineralsServiceCabinet.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index()
        {
            return View(1);
        }

        [HttpGet]
        public ActionResult Settings() {
            return View();
        }

        
    }
}
