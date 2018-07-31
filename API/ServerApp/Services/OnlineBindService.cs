using AltaBO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ServerApp.Services
{
    public class OnlineBindService
    {
        protected string baseUrl;
        protected HttpClient client;

        public OnlineBindService(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public List<PriceOffer> GetAsync(string path)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseUrl + path);
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {

                    if (resp.StatusCode != HttpStatusCode.OK)
                    {
                        throw new HttpException((int)resp.StatusCode, "");
                    }

                    using (var reader = new StreamReader(resp.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        return (List<PriceOffer>)js.Deserialize(objText, typeof(List<PriceOffer>));
                    }

                }
            }
            catch (WebException webExeption)
            {
                throw new HttpException((int)webExeption.Status, "");
            }

        }

        public List<PriceOffer> GetPriceOffers(List<String> lots)
        {
            var builder = new StringBuilder("?");
            var separator = "";
            foreach (var kvp in lots.Where(kvp => kvp != null))
            {
                builder.AppendFormat("{0}{1}={2}", separator, WebUtility.UrlEncode("lots"), WebUtility.UrlEncode(kvp));
                separator = "&";
            }
            try
            {
                return GetAsync("/api/online/test" + builder.ToString());
            }
            catch (HttpException)
            {
                throw;
            }
        }
    }
}