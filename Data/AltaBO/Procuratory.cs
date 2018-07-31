using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class Procuratory {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal MinimalPrice { get; set; }
        public int? fileId { get; set; }
        public int? auctionId { get; set; }
        public int? lotId { get; set; }
        public string lotNumber { get; set; }
        public int? fileListId { get; set; }
    }
}
