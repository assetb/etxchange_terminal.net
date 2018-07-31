using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using DocumentFormation;
using ServerApp.Models;
using ServerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/document-formation")]
    public class DocumentFormationController : BaseApiController
    {
        private const string TECH_SPEC_FILE_NAME = "Technical_specification_of_the_lot";
        private const string PROCURATORY_FILE_NAME = "Order_for_the_transaction";
        private const string SUPPLIER_ORDER_FILE_NAME = "Application_for_participation";
        private const string REPORT_OF_THE_PERIOD = "Report_for_the_period";

        public class FormProcuratory
        {
            public List<Lot> lots { get; set; }
        }
        
        [HttpGet, Route("generate-tech-spec")]
        public HttpResponseMessage GenerateProcuratory(int auctionId, int lotId)
        {
            var auction = DataManager.GetAuction(auctionId);
            var lotExtended = DataManager.GetLotsExtended(lotId);
            if (auction == null || !auction.Lots.Any(l => l.Id == lotId) || lotExtended.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var templateRequisite = ArchiveManager.GetTemplateRequisite((MarketPlaceEnum)auction.SiteId, DocumentTemplateEnum.TechSpec);
            if (templateRequisite == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }

            var localPath = ArchiveManager.LoadTemplateToLocalStorage(templateRequisite);
            var fileName = string.Format("{0}_{1}.{2}", TECH_SPEC_FILE_NAME, auction.Lots.First(l => l.Id == lotId).Number, templateRequisite.extension);

            TechSpecService.CreateDocument(localPath, lotExtended);
            var responceFile = HttpResponceFile.Create(fileName, localPath);

            if (responceFile == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return responceFile;
        }

        [HttpPost, Route("auction/{auctionId}/generate-procuratory"), Authorize(Roles = "supplier")]
        public HttpResponseMessage GenerateProcuratory(int auctionId, FormProcuratory formProcuratory, [FromUri] bool autoCouting)
        {
            var auction = DataManager.GetAuction(auctionId);
            if (auction == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.NotFound, "AUCTION_NOT_FOUND");
            }
            var supplierOrder = DataManager.GetSupplierOrder(auctionId, CurrentUser.SupplierId);

            if (supplierOrder == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.NotFound, "SUPPLIER_ORDER_NOT_FOUND");
            }

            List<Lot> lots = formProcuratory.lots;
            List<Procuratory> procuratories = new List<Procuratory>();

            lots.ForEach(l => procuratories.Add(new Procuratory()
            {
                auctionId = auctionId,
                lotId = l.Id,
                SupplierId = CurrentUser.SupplierId,
                MinimalPrice = l.Sum
            }));

            var templateRequisite = ArchiveManager.GetTemplateRequisite((MarketPlaceEnum)auction.SiteId, DocumentTemplateEnum.Procuratory);

            var pathTemplate = templateRequisite.GenerateFilePath();

            if (!AltaTradingSystemApp.Services.ProcuratoriesService.GenerateProcuratoryFile(DataManager, ArchiveManager, procuratories, pathTemplate, autoCouting))
            {
                throw new AltaHttpResponseException(HttpStatusCode.InternalServerError, "INTERNAL_ERROR");
            }
            var fileName = string.Format("{0}.{1}", PROCURATORY_FILE_NAME, templateRequisite.extension);
            return HttpResponceFile.Create(fileName, pathTemplate);
        }

        [HttpGet, Route("generate-supplier-order")]
        public HttpResponseMessage GenerateSupplierOrder(int auctionId, [FromUri]  List<int> lots)
        {
            if (CurrentUser == null || CurrentUser.SupplierId == 0)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            if (lots == null || lots.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var orders = DataManager.GetOrders(auctionId: auctionId);
            var auction = DataManager.GetAuction(auctionId);
            var supplierOrder = DataManager.GetSupplierOrder(auctionId, CurrentUser.SupplierId);

            if (orders.Count == 0 || auction == null || supplierOrder == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var order = orders[0];

            var templateRequisite = ArchiveManager.GetTemplateRequisite((MarketPlaceEnum)auction.SiteId, DocumentTemplateEnum.SupplierOrder);

            if (templateRequisite == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }

            var localPath = ArchiveManager.LoadTemplateToLocalStorage(templateRequisite);
            if (string.IsNullOrEmpty(localPath))
            {
                var responceException = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responceException.Headers.Add("X-Error-Description", "Error load template supplier order.");
                throw new HttpResponseException(responceException);
            }


            order.Auction = auction;

            foreach (var lot in order.Auction.Lots)
            {
                if (!lots.Contains(lot.Id))
                {
                    order.Auction.Lots.Remove(lot);
                    continue;
                }
            }

            order.Auction.SupplierOrders = new System.Collections.ObjectModel.ObservableCollection<SupplierOrder>() { supplierOrder };
            SupplierOrderService.CreateSupplierOrder(order, localPath);

            var fileName = string.Format("{0}.{1}", SUPPLIER_ORDER_FILE_NAME, templateRequisite.extension);
            var responceFile = HttpResponceFile.Create(fileName, localPath);
            if (responceFile == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return responceFile;
        }

        [HttpGet, Route("report-tech-spec"), Authorize]
        public HttpResponseMessage GenerateReport(DateTime startDate, DateTime endDate, string dateFilterType, int siteId = 0, int statusId = 0, short sortMode = 0, string sortColumnName = null)
        {
            List<int> sites = new List<int>();
            List<int> statuses = new List<int>();
            if (siteId > 0)
            {
                sites.Add(siteId);
            }

            switch (statusId)
            {
                case (1):
                    {
                        statuses.Add(2);
                    }
                    break;
                case (2):
                    {
                        statuses.Add(1);
                        statuses.Add(4);
                        statuses.Add(5);
                    }
                    break;
            }

            List<TechSpecReportBO> reports = null;
            switch (dateFilterType)
            {
                case ("auctionDate"):
                    {
                        reports = DataManager.GetTechSpecReport(CurrentUser.CustomerId, startDate, endDate, sites, statuses, sortMode: sortMode, sortColumnName: sortColumnName);
                    }
                    break;
                case ("orderDate"):
                    {

                        reports = DataManager.GetTechSpecReportByOrderDate(CurrentUser.CustomerId, startDate, endDate, sites, statuses, sortMode: sortMode, sortColumnName: sortColumnName);
                    }
                    break;
            }
            if (reports == null || reports.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var templateRequisite = ArchiveManager.GetTemplateRequisite(MarketPlaceEnum.ETS, DocumentTemplateEnum.TechSpecReport);
            if (templateRequisite == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }

            var localPath = ArchiveManager.LoadTemplateToLocalStorage(templateRequisite);

            TechSpecReportService.CreateDocument(reports, localPath);

            var fileName = string.Format("{0}_from_{1:dd-MM-yyyy}_to_{2:dd-MM-yyyy}.{3}", REPORT_OF_THE_PERIOD, startDate, endDate, templateRequisite.extension);
            var responceFile = HttpResponceFile.Create(fileName, localPath);

            if (responceFile == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return responceFile;

        }
    }
}
