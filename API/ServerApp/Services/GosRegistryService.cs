using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace ServerApp.Services
{
    public static class GosRegistryService
    {
        public static string GetCaptchaImgSource(byte[] page) {
            if (page == null || page.Length == 0) return string.Empty;

            var source = Encoding.GetEncoding("utf-8").GetString(page, 0, page.Length - 1);
            source = WebUtility.HtmlDecode(source);

            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            List<HtmlNode> imgList = doc.DocumentNode.Descendants().Where(x => (x.Name == "img" && x.Attributes["id"] != null && x.Attributes["id"].Value.Contains("imageCaptcha"))).ToList();
            if (imgList.Count == 0) return string.Empty;

            var src = imgList[0].GetAttributeValue("src", null);
            if (string.IsNullOrEmpty(src)) return string.Empty;

            return @"http://kgd.gov.kz" + src;
        }


        public static string GetKgdDutyContent(byte[] page) {
            if (page == null || page.Length == 0) return string.Empty;

            var source = Encoding.GetEncoding("utf-8").GetString(page, 0, page.Length - 1);
            source = WebUtility.HtmlDecode(source);

            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            List<HtmlNode> resultList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["id"] != null && x.Attributes["id"].Value.Contains("result"))).ToList();
            if (resultList.Count == 0) return string.Empty;

            List<HtmlNode> tableList = resultList[0].Descendants("div").ToList();
            return tableList.Count == 0 ? string.Empty : tableList[0].InnerHtml;
        }


        public static string GetUnreliableFormBuildId(byte[] page) {
            if (page == null || page.Length == 0) return string.Empty;

            var source = Encoding.GetEncoding("utf-8").GetString(page, 0, page.Length - 1);
            source = WebUtility.HtmlDecode(source);

            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            List<HtmlNode> resultList = doc.DocumentNode.Descendants().Where(x => x.Name == "input" && x.Attributes["name"] != null && x.Attributes["name"].Value.Contains("form_build_id")).ToList();

            return resultList.Count == 0 ? string.Empty : resultList[0].GetAttributeValue("value",string.Empty);
        }
    }
}