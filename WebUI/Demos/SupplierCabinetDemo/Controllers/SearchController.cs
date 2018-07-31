using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SupplierCabinetDemo.Services;
using SupplierCabinetDemo.Models;

namespace SupplierCabinetDemo.Controllers
{
    public class SearchController : Controller
    {
        [HttpGet]
        public ActionResult Companies()
        {
            return View(DbStorage.GetCustomers());
        }

        [HttpGet]
        public ActionResult Details(string name) {
            return View(DbStorage.GetCustomer(name));
        }

        [HttpGet]
        public ActionResult SearchByGood() {
            return View(new List<Auction>());
        }

        [HttpPost]
        public ActionResult SearchByGood(string key) {
            return View(DbStorage.GetAuctions(true, true).Where(a=>a.lotName.ToLower().Contains(key.ToLower())));
        }

        [HttpGet]
        public ActionResult SearchByCustomer() {
            return View(new List<Auction>());
        }

        [HttpPost]
        public ActionResult SearchByCustomer(string key) {
            return View(DbStorage.GetAuctions(true, true).Where(a => a.customer.ToLower().Contains(key.ToLower())));
        }

        public JsonResult GetGoods() {
            return Json(DbStorage.GetGoods());
        }
    }
}