using System.Collections.Generic;

namespace AltaBO
{
    public class EntryOrder
    {
        public string fromDate { get; set; }
        public string brokerClient { get; set; }
        public string lotNumber { get; set; }
        public string memberCode { get; set; }
        public string fullMemberName { get; set; }
        public string clientCode { get; set; }
        public string clientFullName { get; set; }
        public string clientAddress { get; set; }
        public string clientBIN { get; set; }
        public string clientPhones { get; set; }
        public string clientBankIIK { get; set; }
        public string clientBankBIK { get; set; }
        public string clientBankName { get; set; }
        public List<RequestedDoc> requestedDocs { get; set; }
    }
}
