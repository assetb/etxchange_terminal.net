using System.Web.Mvc;

namespace PaydaTrade.Controllers
{
    public class SupplierController : Controller
    {

        public SupplierController()
        { }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Company(int id)
        {
            return View("Supplier", model: id);
        }

        public ActionResult SearchByProduct()
        {
            return View();
        }

        public ActionResult Unreliability(int companyid, string bin, string name)
        {
            return View();
        }

        public ActionResult KgdDuty(int companyid, string bin, string name)
        {
            return View();
        }

        public ActionResult Taxpayer(int companyid, string bin, string name)
        {
            return View();
        }

        public ActionResult SearchSupplier()
        {
            return View();
        }
    }
}
