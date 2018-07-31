using AltaBO;
using AltaMySqlDB.service;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace ServerApp.Controllers
{

    [RoutePrefix("api/lot")]
    public class LotController : BaseApiController {

        [HttpGet]
        public List<Lot> GetAll(int? auctionid = default(int?)) {
            return DataManager.GetLots(auctionid: auctionid);
        }

        [HttpGet, Route("{lotId}")]
        public Lot Get(int lotId) {
            return DataManager.GetLot(lotId);
        }

        [HttpGet]
        public List<LotsExtended> GetLotsExtended(int lotId) {
            return DataManager.GetLotsExtended(lotId);
        }

        [HttpPost]
        public HttpStatusCode Insert(Lot lot) {
            return HttpStatusCode.NotImplemented;
        }

        [HttpPost]
        public HttpStatusCode Update(int id, Lot lot) {
            return HttpStatusCode.NotImplemented;
        }

        [HttpDelete]
        public HttpStatusCode Delete(int lotId) {
            return HttpStatusCode.NotImplemented;
        }

        [HttpPost, Route("lotex")]
        public string UpdateLotEx(LotsExtended lotsExtended) {
            if(DataManager.UpdateLotsExtended(lotsExtended) == 1) return "Данные успешно сохранены";
            else return "Ошибка при сохранении";
        }
    }
}
