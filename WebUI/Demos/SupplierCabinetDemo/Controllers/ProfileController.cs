using SupplierCabinetDemo.Models;
using SupplierCabinetDemo.Services;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SupplierCabinetDemo.Controllers
{
    public class ProfileController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(DbStorage.GetSuppliers()[0]);
        }

        [HttpPost]
        public ActionResult Update(Supplier supplier)
        {
            supplier.goods = DbStorage.GetSuppliers()[0].goods;
            DbStorage.GetSuppliers()[0] = supplier;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateLots(List<Good> goods)
        {
            DbStorage.GetSuppliers()[0].goods = goods;
            return RedirectToAction("Index");
        }
        
        public ActionResult InsertGood(Good good) {
            if (Request.RequestType == "POST" &&  good != null)
            {
                DbStorage.GetSuppliers()[0].goods.Add(good);
                return RedirectToAction("Index");
            } else
            {
                return View();
            }
        }

        public ActionResult DeletetGood(int id)
        {
            DbStorage.GetSuppliers()[0].goods.RemoveAt(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SaveCommertical(HttpPostedFileBase file)
        {

            var path = Server.MapPath("~/Resources/Trash");

            var fullPath = string.Format("{0}/{1}", path, file.FileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            file.SaveAs(fullPath);
            DbStorage.GetSuppliers()[0].commercialFile = "Resources/Trash/" + file.FileName;

            return RedirectToAction("Index");
        }
    }
}
