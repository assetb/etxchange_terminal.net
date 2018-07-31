using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using AltaMySqlDB.service;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{
    /// <summary>
    /// Контроллер для работы с товарами и услугами
    /// </summary>
    [RoutePrefix("api/product")]
    public class ProductController : BaseApiController
    {
        [HttpGet, Route("")]
        public List<Product> GetProducts(int? companyId = null) {
            return DataManager.GetAllProducts(companyId);
        }
        
        /// <summary>
        /// Получение списка продукции по определенным критериям.
        /// Ответ упаковывается в промежуточный класс ServarApp.Models.Table
        /// </summary>
        /// <param name="page">Страница</param>
        /// <param name="countItems">Коли-во элементов на странице</param>
        /// <param name="text">Текст для поиска</param>
        /// <param name="companyid">Ид компании для фильтрации</param>
        /// <returns></returns>
        [HttpGet, Route("all")]
        public Table<Product> GetAll(int page = 1, int countItems = 10, string text = null, int? companyid = null)
        {
            var table = new Table<Product>() { currentPage = page, countShowItems = countItems };
            var products = DataManager.GetProducts(page, countItems, textSearch: text, companyId: companyid);
            table.countItems = DataManager.GetProductsCount(textSearch: text, companyId: companyid);
            var countPages = (Convert.ToDouble(table.countItems) / Convert.ToDouble(countItems));
            table.countPages = Convert.ToInt32(Math.Ceiling(countPages));
            table.rows = products;
            return table;
        }

        /// <summary>
        /// Получение списка продукции компании.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("company/{id}")]
        public List<ProductCompany> GetProductsCompany(int id)
        {
            return DataManager.GetProductsCompany(id);
        }

        /// <summary>
        /// Moved to CompanyController
        /// </summary>
        /// <param name="productId">Ид товара/услуги</param>
        /// <param name="companyId">Ид компании</param>
        /// <returns></returns>
        [HttpPost, Route("company/{companyId:int}/remove/{productId:int}")]
        public bool GetRemoveProductCompany(int productId, int companyId)
        {
            return DataManager.RemoveProductCopmany(productId, companyId);
        }

        /// <summary>
        /// Moved to CompanyController
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("company/{id}")]
        public bool AddProductByCompany(int id) {
            int? fileId = null;
            var currentDate = DateTime.Now;

            var name = HttpContext.Current.Request.Form.Get("name");
            var description = HttpContext.Current.Request.Form.Get("description");

            var files = HttpContext.Current.Request.Files;
            if (files.Count > 0 )
            {
                var company = DataManager.GetCompany(id);
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
            return DataManager.AddProductFromCompany(name, id, fileId, description);
        }
    }
}
