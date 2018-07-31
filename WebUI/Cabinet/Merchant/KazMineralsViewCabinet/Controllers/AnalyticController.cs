using System.Web.Mvc;

namespace KazMineralsCabinet.Controllers
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
