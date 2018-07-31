using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class BrokerCo
    {
        public BrokerCoCodeEnum Code { get; set; }
        public BrokerCoNameEnum Name { get; set; }
        public BrokerCoManagerEnum Manager { get; set; }
    }
}