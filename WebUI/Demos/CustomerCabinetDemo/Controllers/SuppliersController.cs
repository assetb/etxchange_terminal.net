using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerCabinetDemo.Services;
using CustomerCabinetDemo.Models;

namespace CustomerCabinetDemo.Controllers
{
    public class SuppliersController : Controller
    {
        [HttpGet]
        public ActionResult Companies()
        {
            return View(DbStorage.GetSuppliers());
        }

        [HttpGet]
        public ActionResult Details(string name) {
            return View(DbStorage.GetSupplier(name));
        }

        [HttpGet]
        public ActionResult SearchByGood() {
            return View(new List<Supplier>());
        }

        [HttpPost]
        public ActionResult SearchByGood(string key) {
            return View(DbStorage.GetSuppliersByGood(key));
        }

        [HttpGet]
        public ActionResult KgdDuty(string name) {
            return View(DbStorage.GetSupplier(name));
        }

        [HttpGet]
        public ActionResult Unreliability(string name) {
            return View(DbStorage.GetSupplier(name));
        }

        [HttpGet]
        public ActionResult Taxpayer(string name) {
            return View(DbStorage.GetSupplier(name));
        }
    }
}