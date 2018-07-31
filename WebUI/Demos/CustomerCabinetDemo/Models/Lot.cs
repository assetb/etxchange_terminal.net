using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerCabinetDemo.Models {
    public class Lot {
        public string name { get; set; }
        public decimal startPrice { get; set; }
        public decimal quantity { get; set; }
        public string paymentTerms { get; set; }
        public string deliveryTerms { get; set; }
        public string deliveryPlace { get; set; }
        public string deliveryTime { get; set; }
    }
}