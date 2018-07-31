using Newtonsoft.Json;
using ServerApp.Models.GosRegistries;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/gosregistry")]
    public class GosRegistryController : BaseApiController
    {
        private HttpClient client;

        [HttpGet, Route("kgdduty1")]
        public async Task<HttpResponseMessage> GetKgdDuty1() {
            client = new HttpClient();
            byte[] pageResponse = await client.GetByteArrayAsync(@"http://kgd.gov.kz/ru/apps/services/culs-taxarrear-search-web/index.html");
            var src = Services.GosRegistryService.GetCaptchaImgSource(pageResponse);
            if (string.IsNullOrEmpty(src)) return null;
            var captchaResponse = await client.GetAsync(@"http://kgd.gov.kz" + src);
            return captchaResponse.IsSuccessStatusCode ? captchaResponse : null;
        }



        [HttpGet, Route("kgdduty2/{bin}/{imgcode}")]
        public async Task<string> GetKgdDuty2(int bin,string imgcode) {
            if (client == null) return null;

            var reqPars = new KgdDutyReqPars() {
                CaptchaId = "",
                CaptchaUserValue = imgcode,
                //IinBin = bin

            };
            var jsonPars = await Task.Run(() => JsonConvert.SerializeObject(reqPars));
            var content = new StringContent(jsonPars,Encoding.UTF8,"application/json");
            var pageResponse = await client.PostAsync(@"http://kgd.gov.kz/ru/app/culs-taxarrear-search-web",content);
            byte[] responseContent = await pageResponse.Content.ReadAsByteArrayAsync();
            var result = Services.GosRegistryService.GetKgdDutyContent(responseContent);
            return string.IsNullOrEmpty(result) ? null : result;
        }


        [HttpGet, Route("kgdduty3")]
        public async Task<object> GetKgdDuty3(string bin, string captchaUserValue, string captchaId)
        {
            client = null;
            client = new HttpClient();

            var reqPars = new KgdDutyReqPars()
            {
                CaptchaId = captchaId,
                CaptchaUserValue = captchaUserValue,
                IinBin = bin

            };
            var jsonPars = await Task.Run(() => JsonConvert.SerializeObject(reqPars));
            var content = new StringContent(jsonPars, Encoding.UTF8, "application/json");
            var pageResponse = await client.PostAsync(@"http://kgd.gov.kz/apps/services/culs-taxarrear-search-web/rest/search", content);

            var jsonStr = await pageResponse.Content.ReadAsAsync<object>();

            return jsonStr;
        }


        [HttpGet, Route("unreliable")]
        public async Task<object> GetUnrelible(string bin, string captchaUserValue, string captchaId)
        {
            client = null;
            client = new HttpClient();

            var reqPars = new KgdDutyReqPars()
            {
                CaptchaId = captchaId,
                CaptchaUserValue = captchaUserValue,
                IinBin = bin

            };
            var jsonPars = await Task.Run(() => JsonConvert.SerializeObject(reqPars));
            var content = new StringContent(jsonPars, Encoding.UTF8, "application/json");
            var pageResponse = await client.GetAsync(@"http://kgd.gov.kz/ru/services/taxpayer_search_unreliable");
            byte[] responseContent = await pageResponse.Content.ReadAsByteArrayAsync();
            var result = Services.GosRegistryService.GetUnreliableFormBuildId(responseContent);



            var jsonStr = await pageResponse.Content.ReadAsAsync<object>();

            return jsonStr;
        }
    }
}
