using AltaBO;
using ServerApp.Models;
using System;
using System.Web.Http;
using AltaTradingSystemUI.VM.UtilitsVM;
using System.Globalization;
using System.Collections.Generic;
using System.Web;
using ServerApp.Services;
using DocumentFormation;
using System.IO;
using System.Collections.ObjectModel;

namespace ServerApp.Controllers
{

    [RoutePrefix("api/broker")]
    public class BrokerController : BaseApiController
    {

        Auction auction;



        //Парсин и формирование Заявки на Каспи
        private void createOrder(int[] id)
        {
            var procuratoryRequisite = ArchiveManager.GetDocumentParams(id[0]);
            var techSpecRequisite = ArchiveManager.GetDocumentParams(id[1]);
            var procuratoryPath = procuratoryRequisite.GenerateFilePath();
            var techSpecPath = techSpecRequisite.GenerateFilePath();
            if (ArchiveManager.GetDocument(procuratoryRequisite, procuratoryPath) && ArchiveManager.GetDocument(techSpecRequisite, techSpecPath))
            {               
                var order = ProcuratoryWithTechSpecService.ParseKaspiProcuratory(procuratoryPath);
                var techSpec = ProcuratoryWithTechSpecService.ParseKaspiTechSpec(techSpecPath);
                order.Auction.Lots[0].LotsExtended = techSpec;
            } else
            {
                throw new IOException();
            }
           
        }

        [HttpPost,Route("confirmOrder")]
        public void createOrders()
        {
            var data = HttpContext.Current.Request.Params;
            var ids = data["data"];
            //ids.ForEach(id => createOrder(id));
        }

        [HttpGet,Route("orderList")]
        public List<Order> gerOrderList()
        {   
            return DataManager.GetOrders(statusId:1);   
        }


        [HttpGet,Route("init")]
        public void init()
        {
            var requisite = ArchiveManager.GetTemplateRequisite(AltaBO.specifics.MarketPlaceEnum.Caspy, AltaBO.specifics.DocumentTemplateEnum.Order);
            if (HttpContext.Current.Session["Auction"] == null)
            {
                this.auction = new Auction();
                HttpContext.Current.Session.Add("Auction", this.auction);
            }
        }    

    
        [HttpGet,Route("dateRglament")]
        public List<KeyValuePair<string, DateTime>> getOrder(DateTime orderDate)
        {
           
            Auction auc = new Auction();
            auc.Id = 0;
            DatesRegulationVM dateRegulation = new DatesRegulationVM(auc);
            var list = new List<KeyValuePair<string, DateTime>>() {
            new KeyValuePair<string, DateTime>("AuctionDate", dateRegulation.Order.Auction.Date),
            new KeyValuePair<string, DateTime>("ApplicantsDeadline", dateRegulation.Order.Auction.ApplicantsDeadline),
            new KeyValuePair<string, DateTime>("ExchangeProvisionDeadline", dateRegulation.Order.Auction.ExchangeProvisionDeadline),
            new KeyValuePair<string, DateTime>("OrderDate", dateRegulation.Order.Date),
            new KeyValuePair<string, DateTime>("OrderDeadline", dateRegulation.Order.Deadline),
            };
            return list;

        }

        [HttpGet ,Route("customerEnum")]
        public List<Company> getCustomerEnum()
        {
            return DataManager.GetCustomerEnum();
        }
    }
}
