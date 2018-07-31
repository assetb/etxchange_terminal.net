using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VostokCabinet.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index()
        {
            return View(1);
        }
        
        public ActionResult Supplier([Required] int id) {
            return View("SupplierOld", id);
        }

        [HttpGet]
        public ActionResult Info([Required] int id) {
            return View(id);
        }

        [HttpGet]
        public ActionResult Document([Required] int id)
        {
            return View(id);
        }

        [HttpGet]
        public ActionResult Products([Required] int id) {
            return View(id);
        }

        [HttpGet]
        public ActionResult Settings() {
            return View();
        }

        
    }
}
