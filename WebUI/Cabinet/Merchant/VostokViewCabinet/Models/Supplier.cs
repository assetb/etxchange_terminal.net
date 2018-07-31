using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VostokViewCabinet.Models
{
    public class Supplier
    {
        public int id { get; set; }
        public string bin { get; set; }
        public string name { get; set; }
        public List<Good> goods { get; set; }
    }
}