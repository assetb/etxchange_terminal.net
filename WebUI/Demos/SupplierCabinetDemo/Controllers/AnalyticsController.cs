using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SupplierCabinetDemo.Services;
using SupplierCabinetDemo.Models;

namespace SupplierCabinetDemo.Controllers {
    public class AnalyticsController : Controller {
        [HttpGet]
        public ActionResult GeneralStatistics() {
            return View(DbStorage.GetAuctions(true).Where(a => !string.IsNullOrEmpty(a.winner) && a.winner.Equals(DbStorage.GetSuppliers()[0].name)));
        }

        public JsonResult GeneralStatisticChart() {
            var res = DbStorage.GetAuctions(true).Where(a => !string.IsNullOrEmpty(a.winner) && a.winner.Equals(DbStorage.GetSuppliers()[0].name));

            List<GeneralStatistic> pItems = new List<GeneralStatistic>();

            pItems.Add(new GeneralStatistic { Name = "Выигранные", Number = res.Count() });
            pItems.Add(new GeneralStatistic { Name = "Проигранные", Number = 0 });

            return Json(pItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult WinningGraph() {
            return View(DbStorage.GetAuctions(true).Where(a => !string.IsNullOrEmpty(a.winner) && a.winner.Equals(DbStorage.GetSuppliers()[0].name)));
        }

        public JsonResult WinningStatisticChart() {
            decimal spendSum = 0;
            decimal paymentSum = 0;

            foreach(var item in DbStorage.GetAuctions(true).Where(a => !string.IsNullOrEmpty(a.winner) && a.winner.Equals(DbStorage.GetSuppliers()[0].name))) {
                spendSum += item.startPrice - item.startPrice / 100 * 4;
                paymentSum += (item.startPrice / 100 * 19)-(item.startPrice / 100 * 4);
            }

            List<GeneralStatistic> pItems = new List<GeneralStatistic>();

            pItems.Add(new GeneralStatistic { Name = "Затраченное", Number = Convert.ToInt32(spendSum) });
            pItems.Add(new GeneralStatistic { Name = "Заработанное", Number = Convert.ToInt32(paymentSum) });

            return Json(pItems, JsonRequestBehavior.AllowGet);
        }
    }
}