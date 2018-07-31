using System;

namespace AltaBO
{
    public class SupplierJournal
    {
        public int id { get; set; }
        public int supplierid { get; set; }
        public int brokerid { get; set; }
        public string code { get; set; }
        public DateTime? regdate { get; set; }
        public int serialnumber { get; set; }
        public virtual Supplier supplier { get; set; }
        public virtual Broker broker { get; set; }
    }
}
