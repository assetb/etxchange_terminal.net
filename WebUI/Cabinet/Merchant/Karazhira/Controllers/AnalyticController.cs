using System.Web.Mvc;

namespace VostokCabinet.Controllers
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
    }
}
