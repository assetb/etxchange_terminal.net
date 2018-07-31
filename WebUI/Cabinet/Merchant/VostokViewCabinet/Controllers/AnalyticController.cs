using System.Web.Mvc;

namespace VostokViewCabinet.Controllers
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
