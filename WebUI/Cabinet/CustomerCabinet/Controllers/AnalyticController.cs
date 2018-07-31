using System.Web.Mvc;

namespace CustomerCabinet.Controllers
{
    [RoutePrefix("analytic")]
    public class AnalyticController : Controller
    {
        [HttpGet, Route("report")]
        public ActionResult Report() {
            return View();
        }

        [HttpGet, Route("general")]
        public ActionResult General()
        {
            return View();
        }

        [HttpGet, Route("products")]
        public ActionResult Products()
        {
            return View();
        }


        [HttpGet, Route("wins")]
        public ActionResult WinsCount()
        {
            return View();
        }
        
        [HttpGet, Route("economy")]
        public ActionResult Economy()
        {
            return View();
        }

      
    }
}
