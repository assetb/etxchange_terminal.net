using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using ServerApp.Models;
using ServerApp.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderController : BaseApiController
    {
        [HttpGet, Route("")]
        public List<Order> GetAll(int? auctionId = default(int?), int? customerId = default(int?), int statusId = default(int))
        {
            return DataManager.GetOrders(auctionId, customerId: customerId, statusId: statusId);
        }

        [HttpGet, Route("{id}")]
        public Order Get(int id)
        {
            return DataManager.GetOrder(id);
        }

        [HttpPost, Route("")]
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

            var archive = new ArchiveManager(DataManager);
            var currentDate = DateTime.Now;

            var form = new FormOrder();
            form.siteId = int.Parse(HttpContext.Current.Request.Form.Get("siteId"));
            form.number = HttpContext.Current.Request.Form.Get("number");
            form.customerid = int.Parse(HttpContext.Current.Request.Form.Get("customerId"));

            var filesListId = archive.CreateFilesList("Заявки от заказчика");

            if (archive.PutDocument(orderFile.InputStream, new DocumentRequisite()
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
                if (archive.PutDocument(orderOrigin.InputStream, new DocumentRequisite()
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
                if (archive.PutDocument(agreement.InputStream, new DocumentRequisite()
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
                            if (archive.PutDocument(sheme.InputStream, new DocumentRequisite()
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
            order.customerid = form.customerid;
            order.siteId = form.siteId;
            order.filesListId = filesListId;
            var orderId = DataManager.CreateOrder(order, CurrentUser.Id);

            if (orderId != default(int))
            {
                return order;
            }
            return null;
        }

        [HttpPost, Route("{id}")]
        public HttpStatusCode Update(int id, Order order)
        {
            return HttpStatusCode.NotImplemented;
        }

        [HttpDelete, Route("{id}")]
        public HttpStatusCode Delete(int id)
        {
            return HttpStatusCode.NotImplemented;
        }

        [HttpGet, Route("check")]
        public bool CheckOrder(string number)
        {
            var currentDate = DateTime.Now;
            return DataManager.GetOrdersCount(searchText: number, fromDate: currentDate, toDate: currentDate) > 0;
        }

        [HttpPost, Route("upload")]
        public string UploadFile()
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                var fileStore = new HttpFileStore();
                var f = HttpContext.Current.Request.Files[0];
                return fileStore.PutFile(f);
            }
            return null;
        }
    }
}