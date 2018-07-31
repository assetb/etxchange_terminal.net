using System.Web.Mvc;

namespace KazMineralsServiceCabinet.Controllers
{
    [RoutePrefix("analytic")]
    public class AnalyticController : Controller
    {
        [HttpGet, Route("general")]
        public ActionResult General()
        {
            return View();
        }
    }
}
