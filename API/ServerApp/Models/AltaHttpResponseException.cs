using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ServerApp.Models
{
    public class AltaHttpResponseException : HttpResponseException
    {
        public AltaHttpResponseException(HttpStatusCode httpStatusCode) : base(httpStatusCode)
        { }

        public AltaHttpResponseException(HttpStatusCode httpStatusCode, string description) : base(new HttpResponseMessage(httpStatusCode))
        {
            Response.Headers.Add("X-Error-Description", description);
        }
    }
}