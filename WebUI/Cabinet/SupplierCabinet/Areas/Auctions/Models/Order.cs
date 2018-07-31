using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PersonalCabinetSupplier.Areas.Auctions.Models
{
    public class Order
    {
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public double Sum { get; set; }
    }
}