using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomerCabinetDemo.Services;
using CustomerCabinetDemo.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace CustomerCabinetDemo.Controllers
{
    public class AuctionsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Auction());
        }


        [HttpPost]
        public ActionResult Create(Auction auction)
        {
            auction.orderDate = DateTime.Now;
            auction.auctionDate = DateTime.Now.AddDays(-1);
            auction.status = false;
            auction.winner = "";

            DbStorage.CreateAuction(auction);

            return View("Success");
        }

        [HttpGet]
        public ActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Active()
        {
            return View(DbStorage.GetAuctions(false));
        }

        [HttpPost]
        public ActionResult Active(DateTime dateF, DateTime dateT) {
            return View(DbStorage.GetAuctions(false).Where(a => a.auctionDate >= (dateF == null ? DateTime.Now : dateF) && a.auctionDate <= (dateT == null ? DateTime.Now : dateT)));
        }

        [HttpGet]
        public ActionResult Details(string auctionNumber)
        {
            return View(DbStorage.GetAuctionDetails(auctionNumber));
        }

        [HttpGet]
        public ActionResult History()
        {
            return View(DbStorage.GetAuctions(true));
        }

        [HttpPost]
        public ActionResult History(DateTime dateF, DateTime dateT) {
            return View(DbStorage.GetAuctions(true).Where(a => a.auctionDate >= (dateF == null ? DateTime.Now : dateF) && a.auctionDate <= (dateT == null ? DateTime.Now : dateT)));
        }

        public ActionResult UploadOrder(string number, HttpPostedFileBase order)
        {
            if (Request.RequestType == "POST" && number != null && order != null)
            {
                var auction = new Auction() { number = number, auctionDate = DateTime.Now, orderDate = DateTime.Now, status = false, lotName = "" };

                var path = Server.MapPath("~/Resources/Trash");

                var fullPath = string.Format("{0}/{1}", path, order.FileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                order.SaveAs(fullPath);
                auction.orderFile = "/Resources/Trash/" + order.FileName;

                DbStorage.AddAuction(auction);
                return RedirectToAction("Active");
            }
            return View();
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