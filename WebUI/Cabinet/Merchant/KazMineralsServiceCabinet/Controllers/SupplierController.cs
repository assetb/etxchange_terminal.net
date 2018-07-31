using System.Web.Mvc;

namespace KazMineralsServiceCabinet.Controllers
{
    public class SupplierController : Controller
    {

        public SupplierController()
        { }

        public ActionResult Index(int? id)
        {
            return id != null ? View("Supplier", model: string.Format("{0}", id)) : View();
        }

        public ActionResult Company(int? id)
        {
            return View("Supplier", model:string.Format("company/{0}", id));
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
    }
}
