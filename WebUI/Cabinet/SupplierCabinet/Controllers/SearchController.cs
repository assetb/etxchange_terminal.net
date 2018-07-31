using System.Web.Mvc;

namespace SupplierCabinet.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult SearchByCustomer(int? id) {
            if(id == null || id == 0) {
                return View();
            } else {
                return View("CustomerProfile", id);
            }
        }

        public ActionResult SearchByGood(int? id) {
            if(id == null || id == 0) {
                return View();
            } else {
                return View("CustomerProfile", id);
            }
        }
    }
}