using AltaBO;
using AltaBO.specifics;
using AltaTransport;
using AuthorizationApp.Services;
using KarazhiraCabinet.Models;
using KarazhiraCabinet.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KarazhiraCabinet.Controllers
{
    public class HomeController : Controller {
        private List<Order> ordersList;
        private List<Supplier> suppliersList;


        [HttpGet]
        public ActionResult Index() {
            LogService.LogInfo("Index page opened");
            return View();
        }


        [HttpGet]
        public ActionResult AuctionOrder() {
            var auctionOrderModel = new AuctionOrderModel();
            // Default settings
            auctionOrderModel.Count = "0";
            auctionOrderModel.Price = "0";
            auctionOrderModel.Amount = "0";
            auctionOrderModel.MinimalSum = "0";
            auctionOrderModel.Step = "1";
            auctionOrderModel.TradeWarranty = "1";
            auctionOrderModel.Percent = "0";
            auctionOrderModel.DeliveryTerm = "DDP";
            auctionOrderModel.DeliveryPlace = "г. Семей, ул. Би Боронбай, 93";
            auctionOrderModel.DeliveryTime = "Согласно договору";
            auctionOrderModel.Payment = "По факту";

            try {
                auctionOrderModel.units = DataBaseClient.GetUnits();
                LogService.LogInfo("AuctionOrder page opened");
            } catch(Exception ex) {
                LogService.LogInfo("AuctionOrder page opened with error: " + ex);
                return null;
            }

            return View(auctionOrderModel);
        }


        [HttpPost]
        public ActionResult AuctionOrder(AuctionOrderModel auctionOrderModel) {
            if(ModelState.IsValid) {
                Order order = new Order();

                order.Date = DateTime.Now;
                order.Auction.Lots.Add(new Lot {
                    Name = auctionOrderModel.LotName,
                    Quantity = Convert.ToDecimal(auctionOrderModel.Count.Replace(".", ",")),
                    UnitId = auctionOrderModel.UnitId,
                    Price = Convert.ToDecimal(auctionOrderModel.Price.Replace(".", ",")),
                    Sum = Convert.ToDecimal(auctionOrderModel.Amount.Replace(".", ",")),
                    Step = Convert.ToDecimal(auctionOrderModel.Step.Replace(".", ",")),
                    Warranty = Convert.ToDecimal(auctionOrderModel.TradeWarranty.Replace(".", ",")),
                    LocalContent = Convert.ToInt32(auctionOrderModel.Percent),
                    DeliveryPlace = auctionOrderModel.DeliveryPlace + "|" + auctionOrderModel.DeliveryTerm,
                    DeliveryTime = auctionOrderModel.DeliveryTime,
                    PaymentTerm = auctionOrderModel.Payment
                });

                try {
                    order.Auction.OwnerId = DataBaseClient.GetUserByLogin(HttpContext.User.Identity.Name).Id;
                } catch(Exception) { order.Auction.OwnerId = 1; }

                order.Auction.Comments = auctionOrderModel.MinimalSum + "|" + auctionOrderModel.Comments;

                var statusModel = new StatusModel();

                try {
                    DataBaseClient.SetNewKarazhiraOrder(order);
                    statusModel.Status = 1;
                    EmailSender.Send("altkbroker@gmail.com", "ablrzaieneayisqa", "alta-liya@mail.ru", "Новая заявка Каражыры от " + DateTime.Now.ToShortDateString() + " по лоту " + order.Auction.Lots[0].Name, "Не забудьте обработать!");
                    LogService.LogInfo("Order created successfuly");
                } catch(Exception ex) {
                    statusModel.Status = 0;
                    LogService.LogInfo("Order created with error: " + ex);
                }

                return View("CreateStatus", statusModel);
            } else {
                try {
                    auctionOrderModel.units = DataBaseClient.GetUnits();
                    LogService.LogInfo("AuctionOrder page re-opened because validation summary error");
                } catch(Exception ex) {
                    LogService.LogInfo("AuctionOrder page re-opened with error: " + ex);
                    return null;
                }

                return View(auctionOrderModel);
            }
        }


        [HttpGet]
        public ActionResult CreateStatus() {
            try {
                LogService.LogInfo("CreateStatus page opened");
            } catch(Exception) { }

            return View();
        }


        [HttpGet]
        public ActionResult AuctionsList() {
            try {
                /*ordersList = DataBaseClient.GetKarazhiraAuctions(1);
                ordersList.AddRange(DataBaseClient.GetKarazhiraAuctions(4));*/

                // Some new realisation
                var auctionsList = DataBaseClient.ReadAuctions();

                if(auctionsList != null) {
                    ordersList = new List<Order>();

                    foreach(var item in auctionsList.Where(a => a.siteid == 5 && a.statusid == 4 && a.customerid == 2)) {
                        Order order = new Order();
                        order.Auction = new Auction() {
                            Id = item.id,
                            Date = item.date,
                            Number = item.number,
                            Status = item.status.name
                        };

                        order.Date = item.regulation.opendate;

                        var lot = DataBaseClient.ReadLots(item.id);
                        order.Auction.Lots = new ObservableCollection<Lot>();

                        order.Auction.Lots.Add(new Lot() {
                            Name = lot != null ? lot[0].description : "",
                            Sum = lot != null ? lot[0].sum : 0
                        });

                        ordersList.Add(order);
                    }
                }
                //

                LogService.LogInfo("AuctionList page opened");
            } catch(Exception ex) {
                LogService.LogInfo("AuctionList page opened with error: " + ex);
                return null;
            }

            return View(ordersList);
        }


        [HttpGet]
        public ActionResult AuctionDetails(int auctionId) {
            return View(DataBaseClient.GetKarazhiraAuction(auctionId));
        }


        [HttpGet]
        public ActionResult EndedAuctionsList() {
            try {
                /*ordersList = DataBaseClient.GetKarazhiraAuctions(2);
                ordersList.AddRange(DataBaseClient.GetKarazhiraAuctions(3));*/

                // Some new realisation
                var auctionsList = DataBaseClient.ReadAuctions();

                if(auctionsList != null) {
                    ordersList = new List<Order>();

                    foreach(var item in auctionsList.Where(a => a.siteid == 5 && a.statusid == 2 && a.customerid == 2)) {
                        Order order = new Order();
                        order.Auction = new Auction() {
                            Id = item.id,
                            Date = item.date,
                            Number = item.number,
                            Status = item.status.name
                        };

                        order.Date = item.regulation.opendate;

                        var lot = DataBaseClient.ReadLots(item.id);
                        order.Auction.Lots = new ObservableCollection<Lot>();

                        order.Auction.Lots.Add(new Lot() {
                            Name = (lot != null && lot.Count > 0) ? lot[0].description : "",
                            Sum = (lot != null && lot.Count > 0) ? lot[0].sum : 0
                        });

                        var procuratory = DataBaseClient.ReadProcuratories(item.id);

                        order.Auction.Procuratories = new ObservableCollection<Procuratory>();

                        decimal minimalPrice = 0;
                        string supplierName = "";

                        try {
                            minimalPrice = (procuratory != null && procuratory.Count > 0) ? procuratory.FirstOrDefault(p => p.supplierid != 3).minimalprice : 0;
                            supplierName = (procuratory != null && procuratory.Count > 0) ? procuratory.FirstOrDefault(p => p.supplierid != 3).supplier.company.name : "Нет данных";
                        } catch(Exception) {
                            minimalPrice = 0;
                            supplierName = "";
                        }

                        order.Auction.Procuratories.Add(new Procuratory() {
                            MinimalPrice = minimalPrice,
                            SupplierName = supplierName.Replace("Товарищество с ограниченной ответственностью", "ТОО").Replace("Индивидуальный предприниматель", "ИП")
                        });

                        ordersList.Add(order);
                    }
                }
                //

                LogService.LogInfo("EndedAuctionList page opened");
            } catch(Exception ex) {
                LogService.LogInfo("EndedAuctionList page opened with error: " + ex);
                return null;
            }

            return View(ordersList);
        }


        [HttpGet]
        public ActionResult CompaniesList() {
            try {
                suppliersList = DataBaseClient.GetKarazhiraSuppliers();
                LogService.LogInfo("CompaniesList page opened");
            } catch(Exception ex) {
                LogService.LogInfo("CompaniesList page opened with error: " + ex);
                return null;
            }

            return View(suppliersList);
        }


        [HttpGet]
        public FileResult GetOrderFile(string auctionDate, string auctionNumber) {
            try {
                var filePath = @"Y:\Auctions\KazETS\" + auctionDate + "\\" + auctionNumber.Replace("/", "_") + "\\";

                string[] files = Directory.GetFiles(filePath, "заявка*.pdf");

                filePath = files[0];

                if(System.IO.File.Exists(filePath)) {
                    LogService.LogInfo("Order file for auction date " + auctionDate + " with № " + auctionNumber + " successfuly downloaded");
                    return File(filePath, System.Net.Mime.MediaTypeNames.Application.Pdf, "заявка №" + auctionNumber.Replace("/", "_") + ".pdf");
                } else {
                    LogService.LogInfo("Order file loading error");
                    return null;
                }
            } catch(Exception) { return null; }
        }


        [HttpGet]
        public FileResult GetReportFile(string auctionDate, string auctionNumber) {
            try {
                var filePath = @"Y:\Auctions\KazETS\" + auctionDate + "\\" + auctionNumber.Replace("/", "_") + "\\";

                string[] files = Directory.GetFiles(filePath, "ОЗ*.pdf");

                filePath = files[0];

                if(System.IO.File.Exists(filePath)) {
                    LogService.LogInfo("Report file for auction date " + auctionDate + " with № " + auctionNumber + " successfuly downloaded");
                    return File(filePath, System.Net.Mime.MediaTypeNames.Application.Pdf, "Отчет заказчику по аукциону №" + auctionNumber.Replace("/", "_") + ".pdf");
                } else {
                    LogService.LogInfo("Report file loading error");
                    return null;
                }
            } catch(Exception) { return null; }
        }


        [HttpGet]
        public FileResult GetProtocolFile(string auctionDate, string auctionNumber) {
            var protocolFile = DataBaseClient.ReadDocument((int)DataBaseClient.ReadAuctions().FirstOrDefault(a => a.siteid == 5 && a.number == auctionNumber).fileslistid, (int)DocumentTypeEnum.Protocol);

            if(protocolFile != null) {
                var filePath = @"Y:\Auctions\KazETS\" + auctionDate + "\\" + auctionNumber.Replace("/", "_") + "\\";

                string[] files = Directory.GetFiles(filePath, protocolFile.name + "." + protocolFile.extension);
                if(files.Length > 0) {
                    filePath = files[0];

                    if(System.IO.File.Exists(filePath)) {
                        LogService.LogInfo("Report file for auction date " + auctionDate + " with № " + auctionNumber + " successfuly downloaded");
                        return File(filePath, System.Net.Mime.MediaTypeNames.Application.Octet, "Протокол по аукциону №" + auctionNumber.Replace("/", "_") + "." + protocolFile.extension);
                    } else {
                        LogService.LogInfo("Report file loading error");
                        return null;
                    }
                }
            }

            return null;
        }


        [HttpGet]
        public FileResult GetProtocolFiles(string auctionDate, string auctionNumber) {
            try {
                var filePath = @"Y:\Auctions\KazETS\" + auctionDate + "\\" + auctionNumber.Replace("/", "_") + "\\";

                string[] files = Directory.GetFiles(filePath, "ОЗ*.pdf");

                filePath = files[0];

                if(System.IO.File.Exists(filePath)) {
                    LogService.LogInfo("Report file for auction date " + auctionDate + " with № " + auctionNumber + " successfuly downloaded");
                    return File(filePath, System.Net.Mime.MediaTypeNames.Application.Pdf, "Отчет заказчику по аукциону №" + auctionNumber.Replace("/", "_") + ".pdf");
                } else {
                    LogService.LogInfo("Report file loading error");
                    return null;
                }
            } catch(Exception) { return null; }
        }


        [HttpGet]
        public ActionResult WaitingEndedGraph() {
            LogService.LogInfo("WaitingEndedGraph page opened");
            return View();
        }


        public JsonResult PieChart() {
            var res = DataBaseClient.GetKarazhiraWaitingEndedStatistic();

            List<AuctionStatusStaitstic> pItems = new List<AuctionStatusStaitstic>();

            pItems.Add(new AuctionStatusStaitstic { Name = "Ожидаемые", Number = res[0] });
            pItems.Add(new AuctionStatusStaitstic { Name = "Состоявшиеся", Number = res[1] });
            pItems.Add(new AuctionStatusStaitstic { Name = "Не состоявшиеся", Number = res[2] });

            return Json(pItems, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult EconomySumGraph() {
            ordersList = DataBaseClient.GetKarazhiraAuctions(2);

            LogService.LogInfo("EconomySumGraph page opened");

            return View(ordersList);
        }


        public JsonResult ColumnChart() {
            ordersList = DataBaseClient.GetKarazhiraAuctions(2);

            return Json(ordersList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.LogOut();
            return new RedirectResult("Index");
        }


        public ActionResult Export(string exportType) {
            GridView gv = new GridView();


            if(exportType.Equals("1")) {
                var dataSource = OrderToExcelModelConverter.ConvertActiveAuctions(DataBaseClient.GetKarazhiraAuctions(4));
                dataSource.AddRange(OrderToExcelModelConverter.ConvertActiveAuctions(DataBaseClient.GetKarazhiraAuctions(1)));

                gv.DataSource = dataSource;
            } else {
                var dataSource = OrderToExcelModelConverter.ConvertEndedAuctions(DataBaseClient.GetKarazhiraAuctions(2));
                dataSource.AddRange(OrderToExcelModelConverter.ConvertEndedAuctions(DataBaseClient.GetKarazhiraAuctions(3)));

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

            return RedirectToAction("AuctionsList");
        }
    }
}