using PersonalCabinetSupplier.Areas.Search.Models;
using System.Web.Mvc;

namespace PersonalCabinetSupplier.Areas.Search.Controllers
{
    public class SupplierController : Controller
    {
        public ActionResult Index() {

            return View(new SearchModel<SupplierParametsSearch, Supplier>());
        }
    }
}
