using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/customer"), Authorize]
    public class CustomerController : BaseApiController
    {

        #region Customer
        [HttpGet, Route("")]
        public Table<Customer> GetCustomers(int page, int countItems = 10, string query = null)
        {           
                var skip = (page - 1) * countItems;
                var table = new Table<Customer>() { currentPage = page, countShowItems = countItems };
                var auctions = DataManager.GetCustomers(skip, countItems, search: query);
                table.countItems = DataManager.GetCustomersCount(query);

                table.countPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(table.countItems) / Convert.ToDouble(countItems)));

                table.rows = auctions;

                return table;
           
        }

        [HttpGet, Route("{customerId}")]
        public Customer GetCustomer(int customerId)
        {
            if (CurrentUser.PersonId != 17)
            {
                var customer = DataManager.GetCustomer(customerId);
                if (customer == null)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
                }
                return customer;
            }
            else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }

        }
        #endregion

        [HttpGet, Route("analytic/general"), Authorize(Roles = "customer")]
        public List<AnaliticCountStatus> General(DateTime startDate, DateTime endDate)
        {
            if (CurrentUser.PersonId != 17)
            {
                return DataManager.CustCount(CurrentUser.CustomerId, startDate, endDate);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }
        }

        #region Auction
        [HttpGet, Route("auction"), Authorize(Roles = "customer")]
        public List<Auction> GetAuctions(int page = 0, int countItems = 10, string numberOrProduct = null, DateTime? fromDate = null, DateTime? toDate = null, int? site = default(int?), int? statusid = default(int?), int? winner = default(int), string orderby = null, bool isdesc = false)
        {
            if (CurrentUser.PersonId != 17)
            {
                var auctions = DataManager.GetAuctions(page, countItems,
                numberOrProduct: numberOrProduct,
                customerId: CurrentUser.CustomerId,
                fromDate: fromDate,
                toDate: toDate,
                site: site,
                statusId: statusid,
                winner: winner,
                orderBy: orderby,
                isdesc: isdesc);
                var countElements = DataManager.GetAuctionsCount(numberOrProduct: numberOrProduct,
                    customerId: CurrentUser.CustomerId,
                    fromDate: fromDate,
                    toDate: toDate,
                    site: site,
                    statusId: statusid,
                    winner: (int)winner);
                //var countPages = (Convert.ToDouble(table.countItems) / Convert.ToDouble(countItems));
                HttpContext.Current.Response.AddHeader("X-Count-Elements", countElements.ToString());
                return auctions;
            }
            else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }
        }
        #endregion

        #region Document
        [HttpGet, Route("auction/{auctionId}/protocol"), Authorize(Roles = "customer")]
        public DocumentRequisite GetProtocol(int auctionId)
        {
            var auction = DataManager.GetAuction(auctionId);
            if (auction == null || auction.CustomerId != CurrentUser.CustomerId)
            {
                throw new AltaHttpResponseException(System.Net.HttpStatusCode.Forbidden, "Not found auction");
            }

            var protocols = ArchiveManager.GetFilesInfo(auction.FilesListId, AltaBO.specifics.DocumentTypeEnum.Protocol);
            if (protocols.Count == 0)
            {
                throw new AltaHttpResponseException(System.Net.HttpStatusCode.NotFound, "Not found procolol");
            }
            return protocols.First();
        }

        [HttpGet, Route("auction/{auctionId}/reports"), Authorize(Roles = "customer")]
        public List<DocumentRequisite> GetReports(int auctionId)
        {
            var auction = DataManager.GetAuction(auctionId);
            if (auction == null || auction.CustomerId != CurrentUser.CustomerId)
            {
                throw new AltaHttpResponseException(System.Net.HttpStatusCode.Forbidden, "Not found auction");
            }

            return ArchiveManager.GetFilesInfo(auction.FilesListId, AltaBO.specifics.DocumentTypeEnum.CustomerReport);
        }

        [HttpGet, Route("auction/{auctionId}/documents"), Authorize(Roles = "customer")]
        public List<DocumentRequisite> GetDocuments(int auctionId)
        {
            var auction = DataManager.GetAuction(auctionId);
            if (auction == null || auction.CustomerId != CurrentUser.CustomerId)
            {
                throw new AltaHttpResponseException(System.Net.HttpStatusCode.Forbidden, "Not found auction");
            }

            var documents = ArchiveManager.GetFilesFromList(auction.FilesListId);

            return documents;
        }
        #endregion

        [HttpGet, Route("economy"), Authorize(Roles = "customer")]
        public List<FinalReport> GetEconomyReport(int page, int countItems, string number = null, DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? siteid = default(int?), int? brokerid = default(int?), int? typeid = default(int?))
        {
            if (CurrentUser.PersonId != 17) {
                var count = DataManager.GetEconomyReportCount(number, CurrentUser.CustomerId, fromDate, toDate, siteid, brokerid, typeid);
                var report = DataManager.GetEconomyReport(page, countItems, number, CurrentUser.CustomerId, fromDate, toDate, siteid, brokerid, typeid);
                HttpContext.Current.Response.AddHeader("x-count-items", Convert.ToString(count));
                return report;
            }
            else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }

        }

        #region Order
        [HttpGet, Route("order"), Authorize(Roles = "customer")]
        public List<Order> GetAll(int? auctionId = default(int?), int statusId = default(int))
        {
           
                List<Order> temp = DataManager.GetOrders(auctionId, customerId: CurrentUser.CustomerId, statusId: statusId);
                return temp;           
        }

        [HttpPost,Route("kaspyOrder"),Authorize(Roles = "customer")]
        public Order KaspyInsert()
        {
            var files = HttpContext.Current.Request.Files;
            if (!Request.Content.IsMimeMultipartContent() || files.Count == 0 || HttpContext.Current.Request.Form.Count == 0)
            {
                return null;
            }

            var assignment = files["assignment"];
            var specification = files["specification"];
            var suppliers = files["suppliers"];
            if (assignment == null)
            {
                return null;
            }

            var currentDate = DateTime.Now;

            var form = new FormOrder();
            form.siteId = int.Parse(HttpContext.Current.Request.Form.Get("siteId"));
            form.number = HttpContext.Current.Request.Form.Get("number");
            //form.customerid = int.Parse(HttpContext.Current.Request.Form.Get("customerId"));

            var filesListId = ArchiveManager.CreateFilesList("Заявки от заказчика");

            if (ArchiveManager.PutDocument(assignment.InputStream, new DocumentRequisite()
            {
                date = currentDate,
                fileName = assignment.FileName,
                market = (MarketPlaceEnum)form.siteId,
                number = form.number,
                section = DocumentSectionEnum.Order,
                type = DocumentTypeEnum.OrderSource
            }, filesListId) < 1)
            {
                return null;
            }
            if (specification != null)
            {
                if (ArchiveManager.PutDocument(specification.InputStream, new DocumentRequisite()
                {
                    date = currentDate,
                    fileName = specification.FileName,
                    market = (MarketPlaceEnum)form.siteId,
                    number = form.number,
                    section = DocumentSectionEnum.Order,
                    type = DocumentTypeEnum.TechSpecs
                }, filesListId) < 1)
                {
                    return null;
                }
            }
            if (suppliers != null)
            {
                if (ArchiveManager.PutDocument(suppliers.InputStream, new DocumentRequisite()
                {
                    date = currentDate,
                    fileName = suppliers.FileName,
                    market = (MarketPlaceEnum)form.siteId,
                    number = form.number,
                    section = DocumentSectionEnum.Order,
                    type = DocumentTypeEnum.ApplicantsFromCustomer,
                }, filesListId) < 1)
                {
                    return null;
                }
            }
            var order = new Order();
            order.Date = currentDate;
            order.Deadline = currentDate;
            order.Number = form.number;
            order.statusId = 1;
            order.customerid = CurrentUser.CustomerId;
            order.siteId = form.siteId;
            order.filesListId = filesListId;
            var orderId = DataManager.CreateOrder(order, CurrentUser.Id);

            if (orderId != default(int))
            {
                return order;
            }
            return null;
        }


        //------------------------------------------------------

        [HttpPost, Route("order"), Authorize(Roles = "customer")]
        public Order Insert()
        {
          
                var files = HttpContext.Current.Request.Files;
                if (!Request.Content.IsMimeMultipartContent() || files.Count == 0 || HttpContext.Current.Request.Form.Count == 0)
                {
                    return null;
                }

                var orderFile = files["Order"];
                var orderOrigin = files["OrderOrigin"];
                var agreement = files["Agreement"];
                if (orderFile == null)
                {
                    return null;
                }

                var currentDate = DateTime.Now;

                var form = new FormOrder();
                form.siteId = int.Parse(HttpContext.Current.Request.Form.Get("siteId"));
                form.number = HttpContext.Current.Request.Form.Get("number");
                //form.customerid = int.Parse(HttpContext.Current.Request.Form.Get("customerId"));

                var filesListId = ArchiveManager.CreateFilesList("Заявки от заказчика");

                if (ArchiveManager.PutDocument(orderFile.InputStream, new DocumentRequisite()
                {
                    date = currentDate,
                    fileName = orderFile.FileName,
                    market = (MarketPlaceEnum)form.siteId,
                    number = form.number,
                    section = DocumentSectionEnum.Order,
                    type = DocumentTypeEnum.OrderSource
                }, filesListId) < 1)
                {
                    return null;
                }
                if (orderOrigin != null)
                {
                    if (ArchiveManager.PutDocument(orderOrigin.InputStream, new DocumentRequisite()
                    {
                        date = currentDate,
                        fileName = orderOrigin.FileName,
                        market = (MarketPlaceEnum)form.siteId,
                        number = form.number,
                        section = DocumentSectionEnum.Order,
                        type = DocumentTypeEnum.OrderOriginal
                    }, filesListId) < 1)
                    {
                        return null;
                    }
                }
                if (agreement != null)
                {
                    if (ArchiveManager.PutDocument(agreement.InputStream, new DocumentRequisite()
                    {
                        date = currentDate,
                        fileName = agreement.FileName,
                        market = (MarketPlaceEnum)form.siteId,
                        number = form.number,
                        section = DocumentSectionEnum.Order,
                        type = DocumentTypeEnum.AgreementSource
                    }, filesListId) < 1)
                    {
                        return null;
                    }
                }
                var countShemes = HttpContext.Current.Request.Form.Get("count_shemes");
                if (countShemes != null)
                {
                    var countShemesFiles = int.Parse(countShemes);
                    if (countShemesFiles > 0)
                    {
                        for (var i = 0; i < countShemesFiles; i++)
                        {
                            var sheme = files[string.Format("Sheme{0}", i)];
                            if (sheme != null)
                            {
                                if (ArchiveManager.PutDocument(sheme.InputStream, new DocumentRequisite()
                                {
                                    date = currentDate,
                                    fileName = sheme.FileName,
                                    market = (MarketPlaceEnum)form.siteId,
                                    number = form.number,
                                    section = DocumentSectionEnum.Order,
                                    type = DocumentTypeEnum.Scheme
                                }, filesListId) < 1)
                                {
                                    return null;
                                }
                            }
                        }
                    }
                }
                var order = new Order();
                order.Date = currentDate;
                order.Deadline = currentDate;
                order.Number = form.number;
                order.statusId = 1;
                //order.customerid = form.customerid;
                order.customerid = CurrentUser.CustomerId;
                order.siteId = form.siteId;
                order.filesListId = filesListId;
                var orderId = DataManager.CreateOrder(order, CurrentUser.Id);

                if (orderId != default(int))
                {
                    return order;
                }
                return null;
          
            #endregion
        }
    }
}
