using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class ResponseHeader
    {
        public int RsCode { get; set; }
        public string RsKey { get; set; }
    }
}