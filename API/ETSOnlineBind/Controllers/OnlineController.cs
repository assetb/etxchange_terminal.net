using AltaBO;
using EtsApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace ETSOnlineBind.Controllers
{
    [RoutePrefix("api/online")]
    public class OnlineController : ApiController
    {
        private IEtsApi etsApi;

        public OnlineController(IEtsApi etsApi)
        {
            this.etsApi = etsApi;
        }

        [HttpGet, Route("")]
        public List<PriceOffer> OnlineEts([FromUri] List<String> lots)
        {
            if (lots != null)
            {
                if (etsApi.GetConnection())
                {
                    int idConnectionQuotes = etsApi.QuotesConnection();

                    if (idConnectionQuotes > 0)
                    {
                        var priceOffers = etsApi.GetPriceOffers();
                        return priceOffers.Where(p => lots.Any(lot => lot.Equals(p.lotCode))).ToList();
                    }
                    else
                    {
                        throw new HttpResponseException(System.Net.HttpStatusCode.GatewayTimeout);
                    }
                }
                else
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.ServiceUnavailable);
                }
            }
            else
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
            }
            throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
        }

        [HttpGet, Route("test")]
        public List<PriceOffer> OnlineEtsTest([FromUri] List<String> lots)
        {
            return new List<PriceOffer>() {
                new PriceOffer() {
                   firmName = "test",
                   lotCode = "0T00001",
                   lotPriceOffer = "0",
                   offerTime = DateTime.Now
                }
            };
        }
    }
}
