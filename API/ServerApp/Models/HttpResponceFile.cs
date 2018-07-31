using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ServerApp.Models
{
    public class HttpResponceFile : HttpResponseMessage
    {
        public HttpResponceFile(HttpStatusCode status) : base(status)
        { }

        public static HttpResponceFile Create(string fileName, string pathToFile) {
            var message = new HttpResponceFile(HttpStatusCode.OK);
            var f = new FileStream(pathToFile, FileMode.Open);

            if (f == null) {
                return null;
            }

            message.Content = new StreamContent(f);
            message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            message.Content.Headers.ContentDisposition.FileName = fileName.Replace("\"", "'");
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            message.Content.Headers.ContentType.CharSet = "UTF-8";
            message.Content.Headers.ContentLength = f.Length;

            return message;
        }
    }
}