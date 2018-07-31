using KazMineralsCabinet.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KazMineralsCabinet.Controllers
{
    public class SupplierController : Controller
    {

        public SupplierController()
        { }
        
        public ActionResult Index(int? id)
        {
            return id != null ? View("Supplier", id) : View();
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
