using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/supplier-order")]
    public class SupplierOrderController : BaseApiController
    {
        [HttpGet, Route("available-brokers")]
        public List<Broker> GetAvailableBrokers(int supplierId, int auctionId, [FromUri] List<int> lots)
        {
            var company = DataManager.GetCompanySupplier(supplierId);
            if (company == null || lots == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var brokers = DataManager.GetBrokersCompany(company.id);
            var supplierOrders = DataManager.GetSuplliersOrders(auctionId);

            foreach (var broker in new List<Broker>(brokers))
            {
                if (supplierOrders.Any(so => so.brokerid == broker.Id && so.lots.Any(l => lots.Contains(l.Id))))
                {
                    brokers.Remove(broker);
                }
            }

            return brokers;
        }

        [HttpPost, Route("")]
        public HttpStatusCode CreateSupplierOrder()
        {

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            var files = HttpContext.Current.Request.Files;
            var form = HttpContext.Current.Request.Form;

            if (files == null || files["file"] == null || files.Count == 0 || form == null || form.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var scanOrder = files["file"];
            var formData = JObject.Parse(form.Get("form"));

            var auction = DataManager.GetAuction((int)formData["auctionId"]);
            var supplier = DataManager.GetSupplier((int)formData["supplierId"]);
            var contract = DataManager.GetContractsByCompany(supplier.companyId, (int)formData["brokerId"], auction.SiteId);

            if (auction == null || auction.StatusId != 4 || supplier == null || contract.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var archive = new ArchiveManager(DataManager);
            var fileListId = archive.CreateFilesList("Заявка от поставщика");

            if (archive.PutDocument(scanOrder.InputStream, new DocumentRequisite()
            {
                date = auction.Date,
                market = (MarketPlaceEnum)auction.SiteId,
                filesListId = fileListId,
                number = auction.Number,
                fileName = scanOrder.FileName,
                section = DocumentSectionEnum.Auction,
                type = DocumentTypeEnum.SupplierOrder
            }) < 1)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            var supplierOrder = new SupplierOrder()
            {
                fileListId = fileListId,
                contractId = contract[0].id,
                status = new Status()
                {
                    Id = (int)StatusEnum.Declared
                },
                lots = new List<Lot>(),
                SupplierId = (int)formData["supplierId"]
            };

            foreach (var lotId in formData["lots"].ToList())
            {
                supplierOrder.lots.Add(DataManager.GetLot((int)lotId));
            }

            if (!DataManager.AddSupllierOrder((int)formData["auctionId"], supplierOrder))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return HttpStatusCode.Created;
        }
    }
}
