using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KarazhiraCabinet.Models {
    public class ActiveAuctionsModel {
        public DateTime date { get; set; }
        public string number { get; set; }
        public string status { get; set; }
        public string lotName { get; set; }
        public decimal sum { get; set; }
        public DateTime orderDate { get; set; }
    }
}