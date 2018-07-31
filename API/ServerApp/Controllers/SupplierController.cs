using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using AltaMySqlDB.service;
using AutoMapper;
using DocumentFormation;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Linq;
using ServerApp.Models;
using ServerApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/supplier"), Authorize]
    public class SupplierController : BaseApiController
    {
        public class FormTechSpec
        {
            public List<LotsExtended> lotExtepdeds { get; set; }
        }

        IMapper mapper;
        public SupplierController([Dependency(name: "SupplierMapper")] IMapper mapper)
        {
            this.mapper = mapper;
        }

        #region Supplier
        [HttpGet, Route(""), Authorize]
        public Table<Supplier> GetAll(int page = 1, int countItems = 10, string searchsupplier = null)
        {
            if (CurrentUser.PersonId != 17)
            {
                var table = new Table<Supplier>() { currentPage = page, countShowItems = countItems };
                table.rows = DataManager.GetSuppliers(page, countItems, textSearch: searchsupplier);
                table.countItems = DataManager.GetSuppliersCount(textSearch: searchsupplier);
                var countPages = (System.Convert.ToDouble(table.countItems) / System.Convert.ToDouble(countItems));
                table.countPages = System.Convert.ToInt32(Math.Ceiling(countPages));
                return table;
            }else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }
        }

        [HttpGet, Route("{supplierId}"), Authorize]
        public Supplier GetSupplier(int supplierId)
        {
            return DataManager.GetSupplier(supplierId);
        }

        [HttpGet, Route("{supplierId}/company"), Authorize]
        public Company GetCompany(int supplierId)
        {
            return DataManager.GetCompanySupplier(supplierId);
        }
        #endregion

        #region Auction
        [HttpGet, Route("auction"), Authorize(Roles = "supplier")]
        public Table<Auction> GetAuction(int page = 0, int countItems = 10, string numberOrProduct = null, int? customerid = default(int?), DateTime? fromDate = null, DateTime? toDate = null, int? site = default(int?), int? statusid = default(int?), int winner = default(int), string orderby = null, bool isdesc = false, bool all = false)
        {
            var table = new Table<Auction>() { currentPage = page, countShowItems = countItems };
            var auctions = DataManager.GetAuctions(page, countItems, numberOrProduct: numberOrProduct, customerId: customerid, fromDate: fromDate, toDate: toDate, site: site, supplierId: !all ? CurrentUser.SupplierId : default(int?), statusId: statusid, winner: winner, orderBy: orderby, isdesc: isdesc);
            table.countItems = DataManager.GetAuctionsCount(numberOrProduct: numberOrProduct, customerId: customerid, fromDate: fromDate, toDate: toDate, site: site, supplierId: !all ? CurrentUser.SupplierId : default(int?), statusId: statusid, winner: winner);
            var countPages = (Convert.ToDouble(table.countItems) / Convert.ToDouble(countItems));

            table.countPages = Convert.ToInt32(Math.Ceiling(countPages));
            table.rows = auctions;
            return table;
        }

        [HttpGet, Route("auction/{auctionId}"), Authorize(Roles = "supplier")]
        public Auction GetAuction(int auctionId)
        {
            var auction = DataManager.GetAuction(auctionId);
            return mapper.Map<Auction>(auction);
        }
        #endregion

        #region SupplierOrder
        [HttpGet, Route("auction/{auctionId}/order"), Authorize(Roles = "supplier")]
        public SupplierOrder GetSupplierOrder(int auctionId)
        {
            return DataManager.GetSupplierOrder(auctionId, CurrentUser.SupplierId);
        }

        [HttpPost, Route("auction/{auctionId}/order"), Authorize(Roles = "supplier")]
        public HttpStatusCode PutFilesInSupplierOrder(int auctionId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            if (ArchiveManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            var files = HttpContext.Current.Request.Files;

            var supplierOrder = DataManager.GetSupplierOrder(auctionId, CurrentUser.SupplierId);
            if (supplierOrder == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var auction = DataManager.GetAuction(auctionId);
            var qualifications = DataManager.GetQualifications(auctionId);

            if (files.Get("supplierOrderScan") == null || files.Count < 1)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            Dictionary<string, object> routes_list =
                   (Dictionary<string, object>)json_serializer.DeserializeObject(HttpContext.Current.Request.Form["form"]);

            var dictionaryFiles = new Dictionary<DocumentRequisite, Stream>();
            dictionaryFiles.Add(new DocumentRequisite()
            {
                date = auction.Date,
                market = (MarketPlaceEnum)auction.SiteId,
                filesListId = supplierOrder.fileListId,
                number = auction.Number,
                fileName = files.Get("supplierOrderScan").FileName,
                section = DocumentSectionEnum.Auction,
                type = DocumentTypeEnum.SupplierOrder
            }, files.Get("supplierOrderScan").InputStream);


            foreach (var qualification in qualifications.Where(q => q.file).ToList())
            {
                if (routes_list[qualification.id.ToString()] != null)
                {
                    var docReq = ArchiveManager.GetDocumentParams(int.Parse(routes_list[qualification.id.ToString()].ToString()));
                    var streamFile = ArchiveManager.GetDocument(docReq);
                    if (docReq == null || streamFile == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    dictionaryFiles.Add(docReq, streamFile);
                }
                else
                {
                    var file = files.Get(String.Format("{0}", qualification.id));

                    if (file == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    dictionaryFiles.Add(new DocumentRequisite()
                    {
                        date = auction.Date,
                        market = (MarketPlaceEnum)auction.SiteId,
                        filesListId = supplierOrder.fileListId,
                        number = auction.Number,
                        fileName = String.Format("{0}_{1}_{2}.{3}", CurrentUser.SupplierId, qualification.id, qualification.name, Regex.Replace(file.FileName, @"^.*\.", "", RegexOptions.None)),
                        section = DocumentSectionEnum.Auction,
                        type = DocumentTypeEnum.Other
                    }, file.InputStream);
                }
            }
            foreach (var item in dictionaryFiles)
            {
                if (ArchiveManager.PutDocument(item.Value, item.Key) < 1)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            DataManager.UpdateStatusOnSupplierOrder(supplierOrder.Id, 5);
            return HttpStatusCode.OK;
        }
        #endregion

        #region Document
        [HttpGet, Route("auction/{auctionId}/document"), Authorize(Roles = "supplier")]
        public List<DocumentRequisite> getDocuments(int auctionId)
        {
            var supplierOrder = DataManager.GetSupplierOrder(auctionId, CurrentUser.SupplierId);
            if (supplierOrder == null)
            {
                return new List<DocumentRequisite>();
            }

            if (ArchiveManager == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.InternalServerError, "Not connection to archive manager.");
            }

            return ArchiveManager.GetFilesFromList((int)supplierOrder.fileListId);
        }

        [HttpGet, Route("documnet/contract"), Authorize(Roles = "supplier")]
        public List<DocumentRequisite> getContractsDocuments()
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.Contract });
        }

        [HttpGet, Route("documnet/other"), Authorize(Roles = "Company")]
        public List<DocumentRequisite> getOtherDocuments()
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.Other });
        }

        [HttpGet, Route("{supplierId}/documnet/other")]
        public List<DocumentRequisite> getOtherDocumentsForCompany(int supplierId)
        {
            var company = DataManager.GetCompanySupplier(supplierId);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.Other });
        }
        #endregion

        #region Company
        [HttpGet, Route("company"), Authorize(Roles = "supplier")]
        public Company GetCompany()
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);

            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return company;
        }

        [HttpPost, Route("company"), Authorize(Roles = "supplier")]
        public HttpStatusCode UpdateCompany([FromBody]Company company)
        {
            company.id = CurrentUser.CompanyId;

            return DataManager.UpdateCompany(company) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }
        #endregion

        #region Product
        [HttpGet, Route("{supplierId}/product")]
        public List<ProductCompany> GetProductsForSupplier(int supplierId)
        {
            var company = DataManager.GetCompanySupplier(supplierId);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return DataManager.GetProductsCompany(company.id);
        }

        [HttpGet, Route("product"), Authorize(Roles = "supplier")]
        public List<ProductCompany> GetProducts()
        {
            return DataManager.GetProductsCompany(CurrentUser.CompanyId);
        }

        [HttpDelete, Route("product/{productId}"), Authorize(Roles = "supplier")]
        public HttpStatusCode RemoveProduct(int productId)
        {
            return DataManager.RemoveProductCopmany(productId, CurrentUser.CompanyId) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }


        [HttpPost, Route("product"), Authorize(Roles = "supplier")]
        public HttpStatusCode AppendProduct()
        {
            int? fileId = null;
            var currentDate = DateTime.Now;

            var name = HttpContext.Current.Request.Form.Get("name");
            var description = HttpContext.Current.Request.Form.Get("description");

            var files = HttpContext.Current.Request.Files;
            if (files.Count > 0)
            {
                var company = DataManager.GetCompany(CurrentUser.CompanyId);
                if (company == null)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                var file = files["file"];

                fileId = ArchiveManager.PutDocument(file.InputStream, new DocumentRequisite()
                {
                    date = currentDate,
                    fileName = file.FileName,
                    market = 0,
                    number = company.bin,
                    section = DocumentSectionEnum.Company,
                    type = DocumentTypeEnum.Other
                });
            }
            return DataManager.AddProductFromCompany(name, CurrentUser.CompanyId, fileId, description) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }
        #endregion

        #region FinalReport
        [HttpGet, Route("auction/{auctionId}/final-report"), Authorize(Roles = "supplier")]
        public List<FinalReport> GetFinalReport(int auctionId)
        {
            return DataManager.GetFinalReports(auctionId, CurrentUser.SupplierId);
        }
        #endregion

        #region Procuratory
        [HttpPost, Route("auction/{auctionId}/procuratory"), Authorize(Roles = "supplier")]
        public HttpStatusCode CreateProcuratory(int auctionId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            var scan = HttpContext.Current.Request.Files.Get("scan");
            var template = HttpContext.Current.Request.Files.Get("template");

            if (template == null || scan == null || HttpContext.Current.Request.Form.Get("form") == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var form = JObject.Parse(HttpContext.Current.Request.Form.Get("form"));
            if (form.GetValue("lots") == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var supplierOrder = DataManager.GetSupplierOrder(auctionId, CurrentUser.SupplierId);

            if (supplierOrder == null || supplierOrder.status.Id != 15)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            List<Procuratory> procuratories = new List<Procuratory>();
            foreach (var lot in form.GetValue("lots").ToList())
            {
                if (supplierOrder.lots.Any(l => l.Id == (int)lot["Id"]))
                {
                    var procuratory = new Procuratory()
                    {
                        SupplierId = CurrentUser.SupplierId,
                        lotId = (int)lot["Id"],
                        MinimalPrice = (decimal)lot["Sum"]
                    };
                    procuratories.Add(procuratory);
                }
            }

            var pathToScan = scan.SaveInTemplateFolder();
            var pathToTemplate = template.SaveInTemplateFolder();

            return AltaTradingSystemApp.Services.ProcuratoriesService.AppendProcuratory(DataManager, ArchiveManager, procuratories, pathToTemplate, pathToScan) ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
        }
        #endregion

        #region TechSpec
        [HttpPost, Route("auction/{auctionId}/lot/{lotId}/tech-spec"), Authorize(Roles = "supplier")]
        public HttpResponseMessage UpdateTechSpec(int auctionId, int lotId, FormTechSpec formTechSpec)
        {
            List<LotsExtended> lotsExtendets = formTechSpec.lotExtepdeds;
            var auction = DataManager.GetAuction(auctionId);
            var supplierOrder = DataManager.GetSupplierOrder(auctionId, CurrentUser.SupplierId);
            var lot = DataManager.GetLot(lotId);

            if (auction == null || supplierOrder == null || lot == null || !auction.Lots.Any(l => l.Id == lot.Id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var finalReport = DataManager.GetFinalReport(auctionId, lotId, CurrentUser.SupplierId);
            if (auction.StatusId != 2 || finalReport == null)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            if (lotsExtendets.Sum(l => l.endsum) != finalReport.finalPriceOffer)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            if (lot.LotsExtended.Any(ol => !lotsExtendets.Any(l => l.id == ol.id)))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            foreach (var lotExtend in lotsExtendets)
            {
                if (DataManager.UpdateLotsExtended(lotExtend) == 0)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpPost, Route("auction/{auctionId}/lot/{lotId}/tech-spec-use-template"), Authorize(Roles = "supplier")]
        public HttpResponseMessage UpdateTechSpecUseTemplate(int auctionId, int lotId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            var auction = DataManager.GetAuction(auctionId);
            var finalReport = DataManager.GetFinalReport(auctionId, lotId, CurrentUser.SupplierId);
            if (auction.StatusId != 2 || finalReport == null)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var file = HttpContext.Current.Request.Files.Get("file");
            var localPath = string.Format("{0}/{1}_{2}", HttpContext.Current.Server.MapPath("~/App_Data"), Guid.NewGuid(), file.FileName);

            file.SaveAs(localPath);
            var lot = DataManager.GetLot(lotId);
            if (lot == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var NewLotExtended = new List<LotsExtended>();
            if (!TechSpecService.ParseDocument(localPath, lot.LotsExtended.ToList(), out NewLotExtended))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            return UpdateTechSpec(auctionId, lotId, new FormTechSpec() { lotExtepdeds = NewLotExtended });
        }
        #endregion

        [HttpGet, Route("with-products")]
        public Table<SupplierWithProduct> GetSuppliersWithProduct(int page = 1, int countItems = 10, string searchproduct = null)
        {
            if (CurrentUser.PersonId != 17)
            {
                int count = 0;
                var table = new Table<SupplierWithProduct>() { currentPage = page, countShowItems = countItems };
                table.rows = DataManager.GetSuppliersWithProduct(page, countItems, out count, searchProduct: searchproduct);
                table.countItems = count;
                //table.countItems = DataManager.GetSuppliersWithProductCount(searchProduct: searchproduct);
                var countPages = (System.Convert.ToDouble(table.countItems) / System.Convert.ToDouble(countItems));
                table.countPages = System.Convert.ToInt32(Math.Ceiling(countPages));
                return table;
            }
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }
        }

        [HttpGet, Route("by-params")]
        public Table<Supplier> GetByParam(int page = 1, int countItems = 10, string searchText = null, int method = 1)
        {
            if (CurrentUser.PersonId != 17)
            {
                var table = new Table<Supplier>() { currentPage = page, countShowItems = countItems };
                table.rows = DataManager.GetSuppliersByParam(page, countItems, textSearch: searchText, method: method);
                table.countItems = DataManager.GetSuppliersCount(textSearch: searchText, method: method);
                var countPages = (System.Convert.ToDouble(table.countItems) / System.Convert.ToDouble(countItems));
                table.countPages = System.Convert.ToInt32(Math.Ceiling(countPages));
                return table;
            }else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }
        }

        [HttpGet, Route("analytic/general"), Authorize(Roles = "supplier")]
        public List<AnaliticCountStatus> General(DateTime startDate, DateTime endDate)
        {
            return DataManager.CustCount(CurrentUser.SupplierId, startDate, endDate);
        }
    }


}
