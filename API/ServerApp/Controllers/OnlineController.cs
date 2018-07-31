using AltaBO;
using EtsApp;
using ServerApp.Models;
using ServerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/online")]
    public class OnlineController : BaseApiController
    {
        //OnlineBindService onlineBind;

        //public OnlineController(OnlineBindService onlineBind)
        //{
        //    this.onlineBind = onlineBind;
        //}

        //[HttpGet, Route("ets")]
        //public List<PriceOffer> OnlineEts([FromUri] List<String> lots)
        //{
        //    try
        //    {
        //        return onlineBind.GetPriceOffers(lots);
        //    }
        //    catch(HttpException httpException) {
        //        throw new HttpResponseException((System.Net.HttpStatusCode)httpException.GetHttpCode());
        //    }
        //}

        IEtsApi etsApi;
        public OnlineController(IEtsApi etsApi)
        {
            this.etsApi = etsApi;
        }

        [HttpGet, Route("ets")]
        public Message<List<PriceOffer>> OnlineEts([FromUri] List<String> lots)
        {
            var message = new Message<List<PriceOffer>>() { code = 200, data = new List<PriceOffer>() };


            if (lots != null)
            {
                if (etsApi.GetConnection())
                {
                    int idConnectionQuotes = etsApi.QuotesConnection();

                    if (idConnectionQuotes > 0)
                    {
                        var priceOffers = etsApi.GetPriceOffers();
                        message.data = priceOffers.Where(p => lots.Any(lot => lot.Equals(p.lotCode))).ToList();
                    }
                    else
                    {
                        message.code = 603;
                        message.description = string.Format("Id connection: {0}. Error connecting to the table of price offers. Error: {1}", idConnectionQuotes, etsApi.GetMessage()); ;
                    }
                }
                else
                {
                    message.code = 602;
                    message.description = string.Format("Error connecting to ETS. Error: {0}", etsApi.GetMessage());
                }
            }
            else
            {
                message.code = 601;
                message.description = "Do not all request parameters";
            }
            return message;
        }

    }
}