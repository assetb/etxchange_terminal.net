using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SupplierCabinetDemo.Services;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace SupplierCabinetDemo.Controllers {
    public class AuctionsController : Controller {
        [HttpGet]
        public ActionResult Index() {
            return View();
        }

        [HttpGet]
        public ActionResult Success() {
            return View();
        }

        [HttpGet]
        public ActionResult Active() {
            return View(DbStorage.GetAuctions(false));
        }

        [HttpPost]
        public ActionResult Active(DateTime dateF, DateTime dateT) {
            return View(DbStorage.GetAuctions(false).Where(a => a.auctionDate >= (dateF == null ? DateTime.Now : dateF) && a.auctionDate <= (dateT == null ? DateTime.Now : dateT)));
        }

        [HttpGet]
        public ActionResult Details(string auctionNumber) {
            return View(DbStorage.GetAuctionDetails(auctionNumber));
        }

        [HttpGet]
        public ActionResult History() {
            return View(DbStorage.GetAuctions(true));
        }

        [HttpPost]
        public ActionResult History(DateTime dateF, DateTime dateT) {
            return View(DbStorage.GetAuctions(true).Where(a => a.auctionDate >= (dateF == null ? DateTime.Now : dateF) && a.auctionDate <= (dateT == null ? DateTime.Now : dateT)));
        }

        [HttpPost]
        public ActionResult UploadOrder(string auctionNumber, HttpPostedFileBase file) {
            var path = HttpContext.Server.MapPath("~/Resources/Trash/");
            var fileName = file.FileName;

            if(System.IO.File.Exists(string.Format("{0}{1}", path, fileName))) {
                System.IO.File.Delete(string.Format("{0}{1}", path, fileName));
            }

            file.SaveAs(string.Format("{0}{1}", path, fileName));

            DbStorage.PutFileToAuction(auctionNumber, "/Resources/Trash/" + fileName);

            return RedirectToAction("Details", new { auctionNumber = auctionNumber });
        }

        [HttpGet]
        public ActionResult Played() {
            return View(DbStorage.GetAuctions(true).Where(a => !string.IsNullOrEmpty(a.winner) && a.winner.Equals(DbStorage.GetSuppliers()[0].name)));
        }

        public ActionResult Export(string exportType) {
            GridView gv = new GridView();

            if(exportType.Equals("1")) {
                var dataSource = DbStorage.GetAuctions(false);

                gv.DataSource = dataSource;
            } else {
                var dataSource = DbStorage.GetAuctions(true);

                gv.DataSource = dataSource;
            }

            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Список аукционов.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Active");
        }
    }
}