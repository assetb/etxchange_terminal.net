using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models
{
    public class FormOrder
    {
        public int siteId { get; set; }
        public string number { get; set; }
        public string fileId { get; set; }
        public string fileName { get; set; }
        public int customerid { get; set; }
    }
}