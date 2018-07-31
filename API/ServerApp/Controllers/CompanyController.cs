using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using AltaMySqlDB.service;
using AltaTransport;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{
    /// <summary>
    /// Контроллер определяет методы доступные в контексте компании.
    /// </summary>
    [RoutePrefix("api/company"), Authorize]
    public class CompanyController : BaseApiController
    {
        #region Company
        [HttpGet, Route("")]
        public List<Company> GetCompanies()
        {
            return DataManager.GetCompanies();
        }

        [HttpGet, Route("{companyId}")]
        public Company GetCompanyById(int companyId)
        {
            return DataManager.GetCompany(companyId);
        }

        [HttpPost, Route("")]
        public HttpStatusCode UpdateCompany([FromBody]Company company)
        {
            company.id = CurrentUser.Id;

            return DataManager.UpdateCompany(company) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }
        #endregion

        #region Broker
        [HttpGet, Route("broker")]
        public List<Broker> GetBrokers()
        {
            return DataManager.GetBrokersCompany(CurrentUser.CompanyId);
        }
        #endregion

        #region Document
        [HttpPost, Route("document")]
        public HttpStatusCode PutDocument([FromUri] int docTypeId)
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);

            if (company == null)
                return HttpStatusCode.NotFound;

            var currentDate = DateTime.Now;
            var description = HttpContext.Current.Request.Form.Get("description");

            var files = HttpContext.Current.Request.Files;
            if (files.Count > 0)
            {
                var file = files["file"];

                if (ArchiveManager.PutDocument(file.InputStream, new DocumentRequisite()
                {
                    date = currentDate,
                    fileName = file.FileName,
                    market = MarketPlaceEnum.ForAll,
                    description = description,
                    number = company.bin,
                    section = DocumentSectionEnum.Company,
                    type = (DocumentTypeEnum)docTypeId
                }, company.filesListId) < 1)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            return HttpStatusCode.Created;
        }

        [HttpDelete, Route("document/{documentId}")]
        public HttpStatusCode RemoveDocument(int documentId)
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.InternalServerError, "Not found \"company\".");
            }

            var documentList = ArchiveManager.GetFilesFromList(company.filesListId);
            if (!documentList.Any(d => d.id == documentId))
            {
                throw new AltaHttpResponseException(HttpStatusCode.NotFound, "Not found document in list company");
            }

            return DataManager.RemoveDocumentInList(company.filesListId, documentId) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }

        [HttpGet, Route("document/contract")]
        public List<DocumentRequisite> getContractsDocuments()
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.Contract });
        }

        [HttpGet, Route("document/other")]
        public List<DocumentRequisite> getOtherDocuments()
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.Other });
        }

        [HttpGet, Route("{companyId}/document/other")]
        public List<DocumentRequisite> getOtherDocuments(int companyId)
        {
            var company = DataManager.GetCompany(companyId);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.Other });
        }

        [HttpGet, Route("document/commertical")]
        public List<DocumentRequisite> GetCommerticals()
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.NotFound, "Not found \"company\".");
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.CommercialOffer });
        }

        [HttpGet, Route("{companyId}/document/commertical")]
        public List<DocumentRequisite> GetCommerticals(int companyId)
        {
            var company = DataManager.GetCompany(companyId);
            if (company == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.NotFound, "Not found \"company\".");
            }

            return ArchiveManager.GetFilesInfo(company.filesListId, new List<int>() { (int)DocumentTypeEnum.CommercialOffer });
        }
        #endregion

        #region Reconciliation
        [HttpGet, Route("reconciliation/{brokerId}")]
        public List<ReconcilationReport> GetReconciliationReports(int brokerId, [FromUri] DateTime startDate, [FromUri] DateTime toDate)
        {
            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.InternalServerError, "Fatal error! Not found \"company\"");
            }

            if (DataManager.GetBroker(brokerId) == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.NotFound, "Not found \"broker\"");
            }

            var configuration = DataManager.GetConfiguration(brokerId);

            if (configuration == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }

            try
            {
                //var res = _1CTransport.GetReconcilation(configuration.url, configuration.user, configuration.pass, company.bin, null, startDate, toDate);
                var res = _1CTransport.GetReconcilation(brokerId, company.bin, null, startDate, toDate);
                return res;
            }
            catch(Exception e)
            {
                

                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Product
        [HttpGet, Route("product")]
        public List<ProductCompany> GetProducts()
        {
            return DataManager.GetProductsCompany(CurrentUser.CompanyId);
        }

        [HttpPost, Route("product")]
        public HttpStatusCode AddProductByCompany()
        {
            int? fileId = null;
            var currentDate = DateTime.Now;


            var company = DataManager.GetCompany(CurrentUser.CompanyId);
            if (company == null)
            {
                throw new AltaHttpResponseException(HttpStatusCode.InternalServerError, "Fatal error! Not found company");
            }

            var name = HttpContext.Current.Request.Form.Get("name");
            var description = HttpContext.Current.Request.Form.Get("description");

            var files = HttpContext.Current.Request.Files;
            if (files.Count > 0)
            {
                var file = files["file"];

                var archive = new ArchiveManager(DataManager);
                fileId = archive.PutDocument(file.InputStream, new DocumentRequisite()
                {
                    date = currentDate,
                    fileName = file.FileName,
                    market = 0,
                    number = company.bin,
                    section = DocumentSectionEnum.Company,
                    type = DocumentTypeEnum.Other
                });
            }

            var orderFile = files["Order"];
            var orderOrigin = files["OrderOrigin"];
            var agreement = files["Agreement"];
            return DataManager.AddProductFromCompany(name, CurrentUser.CompanyId, fileId, description) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }

        [HttpDelete, Route("product/{productId}")]
        public HttpStatusCode DeleteProduct(int productId)
        {
            return DataManager.RemoveProductCopmany(productId, CurrentUser.CompanyId) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }
        #endregion

        #region Old functions

        [HttpGet, Route("with-products")]
        public Table<CompanyWithProducts> GetCompaniesWithProduct(int page, int countItems, string productName = null)
        {
            var table = new Table<CompanyWithProducts>() { currentPage = page, countShowItems = countItems };
            table.rows = DataManager.GetCompanyWithProduct(page, countItems, productName: productName);
            table.countItems = DataManager.GetCompanyWithProductCount(productName: productName);
            var countPages = (System.Convert.ToDouble(table.countItems) / System.Convert.ToDouble(countItems));
            table.countPages = System.Convert.ToInt32(Math.Ceiling(countPages));

            return table;
        }
        #endregion
    }
}
