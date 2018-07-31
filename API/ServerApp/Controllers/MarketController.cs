using AltaBO;
using AltaMySqlDB.service;
using System.Collections.Generic;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/market")]
    public class MarketController : BaseApiController
    {
        
        [HttpGet, Route("")]
        public List<Site> GetAll()
        {
            return DataManager.GetSites();
        }
    }
}
