using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KarazhiraCabinet.Models {
    public class EndedAuctionsModel:ActiveAuctionsModel {
        public decimal winningSum { get; set; }
        public decimal economy { get; set; }
        public string supplier { get; set; }
    }
}