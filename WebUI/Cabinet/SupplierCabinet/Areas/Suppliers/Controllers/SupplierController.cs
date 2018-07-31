using System.Collections.Generic;
using System.Web.Mvc;

namespace PersonalCabinetSupplier.Areas.Suppliers.Controllers
{
    public class SupplierController : Controller
    {
        public ActionResult Index() {
            return View(new List<object>());
        }
    }
}
