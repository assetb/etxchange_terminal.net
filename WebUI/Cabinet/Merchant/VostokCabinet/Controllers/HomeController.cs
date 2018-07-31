using AuthorizationApp;
using AuthorizationApp.Services;
using System.Web.Mvc;


namespace VostokCabinet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {               
            System.Web.HttpContext.Current.LogOut();
            return new RedirectResult("Index");
        }
    }
}
