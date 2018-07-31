using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupplierCabinetDemo.Models {
    public class AuctionDetails {
        public Auction auction { get; set; }
        public Lot lot { get; set; }
        public List<Supplier> suppliers { get; set; }
    }
}