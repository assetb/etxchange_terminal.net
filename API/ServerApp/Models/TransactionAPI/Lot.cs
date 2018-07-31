using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class Lot
    {
        public int No { get; set; }
        public string Name { get; set; }
        public float Quantity { get; set; }
        public string Characteristics { get; set; }
        public string DeliveryTerms { get; set; }
        public string DeliveryTime { get; set; }
        public string PayTerms { get; set; }
        public string Description { get; set; }
        public decimal StartPrice { get; set; }
        public string LocalMinRequirement { get; set; }
        public Attachments Attachments { get; set; }
    }
}
