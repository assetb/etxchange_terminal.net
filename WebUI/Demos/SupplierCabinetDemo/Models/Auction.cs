using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupplierCabinetDemo.Models {
    public class Auction {
        public DateTime auctionDate { get; set; }
        public string number { get; set; }
        public bool status { get; set; }
        public string lotName { get; set; }
        public decimal startPrice { get; set; }
        public DateTime orderDate { get; set; }
        public string winner { get; set; }
        public string supplierOrder { get; set; }
        public string customer { get; set; }
        public string source { get; set; }
    }
}