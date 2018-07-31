using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class Deadlines
    {
        public DateTime Publish { get; set; }
        public DateTime Bid { get; set; }
        public DateTime Applicant { get; set; }
        public DateTime Provision { get; set; }
    }
}
