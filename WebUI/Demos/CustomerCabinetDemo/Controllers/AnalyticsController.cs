using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerCabinetDemo.Services;
using CustomerCabinetDemo.Models;

namespace CustomerCabinetDemo.Controllers {
    public class AnalyticsController : Controller {
        [HttpGet]
        public ActionResult GeneralStatistics() {
            return View(DbStorage.GetAuctions(true, true));
        }

        public JsonResult GeneralStatisticChart() {
            var res = DbStorage.GetAuctions(true, true);

            List<GeneralStatistic> pItems = new List<GeneralStatistic>();

            pItems.Add(new GeneralStatistic { Name = "Ожидаемые", Number = res.Where(r => r.status == false).ToList().Count() });
            pItems.Add(new GeneralStatistic { Name = "Состоявшиеся", Number = res.Where(r => r.status == true).ToList().Count() });
            pItems.Add(new GeneralStatistic { Name = "Не состоявшиеся", Number = 0 });

            return Json(pItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EconomyGraph() {
            return View(DbStorage.GetAuctions(true));
        }

        public JsonResult EconomyStatisticChart() {
            decimal spendSum = 0;
            decimal economySum = 0;

            foreach(var item in DbStorage.GetAuctions(true)) {
                spendSum += item.startPrice;
                economySum += item.startPrice / 100 * 4;
            }

            List<GeneralStatistic> pItems = new List<GeneralStatistic>();

            pItems.Add(new GeneralStatistic { Name = "Затраченное", Number = Convert.ToInt32(spendSum) });
            pItems.Add(new GeneralStatistic { Name = "Сэкономленное", Number = Convert.ToInt32(economySum) });

            return Json(pItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GoodAndSupplier() {
            return View(new Supplier());
        }

        [HttpPost]
        public ActionResult GoodAndSupplier(string key) {
            var auction = DbStorage.GetAuctions(true).Where(a => a.lotName.ToLower().Contains(key.ToLower())).FirstOrDefault();
            if(auction != null) {
                var supplier = DbStorage.GetSupplier(auction.winner);

                if(supplier != null) return View(supplier);
            }
            return View(new Supplier());
        }

        [HttpGet]
        public ActionResult GoodAndSuppliers() {
            return View(new List<Supplier>());
        }

        [HttpPost]
        public ActionResult GoodAndSuppliers(string key) {
            var auctions = DbStorage.GetAuctions(true).Where(a => a.lotName.ToLower().Contains(key.ToLower()));
            List<Supplier> suppliers = new List<Supplier>();
            Supplier supplier = new Supplier();

            foreach(var item in auctions) {
                if(suppliers.Where(s => s.name == item.winner).Count() < 1) {
                    supplier = DbStorage.GetSupplier(item.winner);
                    supplier.goods.Insert(0, DbStorage.GetGood(item.lotName));
                    suppliers.Add(supplier);
                }
            }

            return View(suppliers);
        }

        [HttpGet]
        public ActionResult MaxGoodSupplier() {
            return View(new Supplier());
        }

        [HttpPost]
        public ActionResult MaxGoodSupplier(string key) {
            var auction = DbStorage.GetAuctions(true).Where(a => a.lotName.ToLower().Contains(key.ToLower())).FirstOrDefault();
            if(auction != null) {
                var supplier = DbStorage.GetSupplier(auction.winner);

                if(supplier != null) return View(supplier);
            }
            return View(new Supplier());
        }
    }
}