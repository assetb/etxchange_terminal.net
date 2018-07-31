using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using AltaMySqlDB.service;
using Newtonsoft.Json.Linq;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{

    [RoutePrefix("api/auction")]
    public class AuctionController : BaseApiController
    {
        
        #region Qualification
        [HttpGet, Route("{auctionId}/qualifications")]
        public List<Qualification> GetQualifications(int auctionId)
        {
            return DataManager.GetQualifications(auctionId);
        }
        #endregion

        #region Auction
        [HttpGet, Route("")]
        public Table<Auction> GetAll(int page = 0, int countItems = 10, string numberOrProduct = null, int? customerid = default(int?), int? supplierid = default(int?), DateTime? fromDate = null, DateTime? toDate = null, int? site = default(int?), int? statusid = default(int?), int winner = default(int), string orderby = null, bool isdesc = false, int? brokerId = default(int?), int? traderId = default(int?))
        {
            var table = new Table<Auction>() { currentPage = page, countShowItems = countItems };
            var auctions = DataManager.GetAuctions(page, countItems, numberOrProduct: numberOrProduct, customerId: customerid, fromDate: fromDate, toDate: toDate, site: site, supplierId: supplierid, statusId: statusid, winner: winner, orderBy: orderby, isdesc: isdesc,brokerId:brokerId,traderId: traderId);
            table.countItems = DataManager.GetAuctionsCount(numberOrProduct: numberOrProduct, customerId: customerid, fromDate: fromDate, toDate: toDate, site: site, supplierId: supplierid, statusId: statusid, winner: winner,brokerId:brokerId,traderId: traderId);
            var countPages = (Convert.ToDouble(table.countItems) / Convert.ToDouble(countItems));

            table.countPages = Convert.ToInt32(Math.Ceiling(countPages));
            table.rows = auctions;
            return table;
        }

        [HttpGet, Route("{auctionId}")]
        public Auction Get(int auctionId)
        {
            return DataManager.GetAuction(auctionId);
        }
        #endregion

        #region FinalReport
        /// <summary>
        /// Moved to SupplierController
        /// </summary>
        /// <param name="auctionId"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [HttpGet, Route("{auctionId}/final-reports/{supplierId}")]
        public List<FinalReport> GetFinalReportsForSupplier(int auctionId, int supplierId)
        {
            return DataManager.GetFinalReports(auctionId, supplierId);
        }
        #endregion

        #region SuppplierOrder
        [HttpGet, Route("{auctionId}/supplier-orders/{supplierId}")]
        public SupplierOrder GetSupplierOrder(int auctionId, int supplierId)
        {
            return DataManager.GetSupplierOrder(auctionId, supplierId);
        }
        #endregion

        #region Order
        [HttpGet, Route("{auctionId}/customer-order"), Authorize(Roles = "customer")]
        public List<Order> GetAll(int auctionId)
        {
            return DataManager.GetOrders(auctionId, customerId: CurrentUser.CustomerId);
        }
        #endregion

        [HttpPost, Route("{auctionId}/procuratory")]
        public HttpStatusCode CreateProcuratory(int auctionId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            var file = HttpContext.Current.Request.Files.Get("file");

            if (file == null || HttpContext.Current.Request.Form.Get("form") == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var form = JObject.Parse(HttpContext.Current.Request.Form.Get("form"));
            if (form.GetValue("lots") == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var supplierId = (int)form.GetValue("supplierId");

            var supplierOrder = DataManager.GetSupplierOrder(auctionId, supplierId);

            if (supplierOrder == null || supplierOrder.status.Id != 15)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var auction = DataManager.GetAuction(auctionId);

            List<Procuratory> procuratories = new List<Procuratory>();

            var archive = new ArchiveManager(DataManager);
            var fileId = archive.PutDocument(file.InputStream, new DocumentRequisite()
            {
                date = auction.Date,
                market = (MarketPlaceEnum)auction.SiteId,
                number = auction.Number,
                fileName = file.FileName,
                section = DocumentSectionEnum.Auction,
                type = DocumentTypeEnum.Procuratory
            });
            if (fileId < 1)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            foreach (var lot in form.GetValue("lots").ToList())
            {
                if (supplierOrder.lots.Any(l => l.Id == (int)lot["Id"]))
                {
                    var procuratory = new Procuratory()
                    {
                        SupplierId = supplierId,
                        lotId = (int)lot["Id"],
                        MinimalPrice = (decimal)lot["Sum"],
                        fileId = fileId
                    };
                    procuratories.Add(procuratory);
                }
            }

            return DataManager.AddProcuratories(auctionId, procuratories) ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
        }

        [HttpDelete, Route("{id}")]
        public HttpStatusCode Delete(int id)
        {
            return HttpStatusCode.NotImplemented;
        }

        [HttpGet, Route("statuses")]
        public List<Status> GetStatuses()
        {
            var statuses = DataManager.GetStatuses();
            return statuses;
        }

        [HttpGet, Route("sites")]
        public List<Site> GetSites()
        {
            var sites = DataManager.GetCatalogSites();
            return sites;
        }

        [HttpGet, Route("{auctionId}/applicants")]
        public List<Applicant> GetApplicants(int auctionId)
        {
            return DataManager.GetApplicants(auctionId);
        }

        //old method
        [HttpGet, Route("applicants")]
        public List<Applicant> GetApplicantsOld(int auctionId)
        {
            return DataManager.GetApplicants(auctionId);
        }

        [HttpGet, Route("{auctionId}/orders_supplier")]
        public List<SupplierOrder> GetAllSupplierApplicants(int auctionId)
        {
            return DataManager.GetSuplliersOrders(auctionId);
        }

        [HttpPost, Route("{auctionId}/orders_supplier")]
        public bool AddSupplierOrder(int auctionId, SupplierOrder supplierOrder)
        {
            return DataManager.AddSupllierOrder(auctionId, supplierOrder);
        }

        [HttpPost, Route("add_supplier")]
        public bool AddSupplier(int auctionId, int supplierId)
        {
            var auction = DataManager.GetAuction(auctionId);
            if (auction.ApplicantsDeadline < DateTime.Now)
            {
                return false;
            }
            return DataManager.AddSupplier(auctionId, supplierId);
        }

        [HttpPost, Route("reject_supplier")]
        public bool RejectSupplier(int auctionId, int supplierId)
        {
            var auction = DataManager.GetAuction(auctionId);
            if (auction.ApplicantsDeadline < DateTime.Now)
            {
                return false;
            }
            return DataManager.RejectSupplier(auctionId, supplierId);
        }

        [HttpGet, Route("auctionsresult")]
        public List<AuctionResult> GetAuctionsResult()
        {
            return DataManager.GetAuctionsResult();
        }

        [HttpPost, Route("createAuction"), Authorize]
        public bool createAuction()
        {
            
            return false;
        }
    }
}
