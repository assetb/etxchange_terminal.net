using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class RequestHeader
    {
        public int No { get; set; }
        public DateTime Date { get; set; }
        public string VersionNo { get; set; }
        public int LastRqNo { get; set; }
        public int StanNo { get; set; }
        public int RqType { get; set; }
    }
}