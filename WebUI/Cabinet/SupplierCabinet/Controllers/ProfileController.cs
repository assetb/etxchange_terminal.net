using System.Web.Mvc;

namespace SupplierCabinet.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Settings()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CompanyDetails()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CompanyEditor()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Product()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Document()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddDocument()
        {
            return View();
        }
    }
}
