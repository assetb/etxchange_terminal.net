﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VostokViewCabinet.Models
{
    public class OrderForReport
    {
        public int id { get; set; }
        public string number { get; set; }
        public decimal sum { get; set; }
        public decimal sumFinal { get; set; }
        public string status { get; set; }
        public int statusId { get; set; }
    }
}