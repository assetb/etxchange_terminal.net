using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerCabinetDemo.Models {
    public class Supplier {
        public string name { get; set; }
        public string bin { get; set; }
        public int kbe { get; set; }
        public string country { get; set; }
        public string address { get; set; }
        public string telephones { get; set; }
        public string email { get; set; }
        public string postcode { get; set; }
        public string director { get; set; }
        public string iik { get; set; }
        public List<Good> goods { get; set; }
        public string commercialFile { get; set; }

        public int memberGos { get; set; }
        public int memberSamruk { get; set; }
        public int listConscientiousSamruk { get; set; }
        public int listConscientiousGos { get; set; }
        public int listUnfairSamruk { get; set; }
    }
}