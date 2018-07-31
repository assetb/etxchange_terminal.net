using AuthorizationApp;
using AuthorizationApp.Services;
using System.Web.Mvc;

namespace SupplierCabinet.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {            
            System.Web.HttpContext.Current.LogOut();
            return new RedirectResult("Index");
        }
    }
}