using AltaMySqlDB.service;
using ServerApp.Models.Analytic;
using System;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/analytic"), Authorize(Roles = "customer")]
    public class AnalyticController : BaseApiController
    {
        private const int EXPECTED_STATUS = 4;
        private const int FINISHED_STATUS = 2;
        private const int NOT_HELD_STATUS = 3;

        [HttpGet, Route("general")]
        public GeneralInfo General(int? customerId = default(int?))
        {

            var generalInfo = new GeneralInfo();
            generalInfo.count = DataManager.GetAuctionsCount(customerId: CurrentUser.CustomerId);
            var auctions = DataManager.GetAuctions(1, generalInfo.count, customerId: CurrentUser.CustomerId);


            foreach (var auction in auctions)
            {
                if (auction.StatusId == FINISHED_STATUS)
                {
                    generalInfo.finished += 1;
                }
                if (auction.StatusId == NOT_HELD_STATUS)
                {
                    generalInfo.notHeld += 1;
                }
                if (auction.StatusId == EXPECTED_STATUS)
                {
                    generalInfo.expected += 1;
                }
            }

            return generalInfo;
        }

        [HttpGet, Route("saving")]
        public string Saving(int? customerId = default(int?))
        {
            return "";
        }

    }
}
