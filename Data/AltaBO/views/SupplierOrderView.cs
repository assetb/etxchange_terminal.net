using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.views
{
    public class SupplierOrderView
    {
        public int id { get; set; }
        public int supplierId { get; set; }
        public string name { get; set; }
        public int auctionId { get; set; }
        public int? contractId { get; set; }
        public string contractNum { get; set; }
        public int? brokerId { get; set; }
        public string brokerName { get; set; }
        public DateTime date { get; set; }
        public int statusId { get; set; }
        public int fileListId { get; set; }
    }
}
