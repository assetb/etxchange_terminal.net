using System.Web.Mvc;
using System.Web.Security;

namespace KazakhaltynCabinet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();

            return new RedirectResult("Index");
        }
    }
}
