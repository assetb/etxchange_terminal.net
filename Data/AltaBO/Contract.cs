using System;

namespace AltaBO
{
    public class Contract
    {
        public int id { get; set; }
        public int? companyid { get; set; }
        public string number { get; set; }
        public DateTime? agreementdate { get; set; }
        public DateTime? terminationdate { get; set; }
        public int? bankid { get; set; }
        public int? documentId { get; set; }
        public DateTime? updatedate { get; set; }
        public DateTime? createdate { get; set; }
        public int? contracttypeid { get; set; }
        public int? currencyid { get; set; }
        public int? brokerid { get; set; }
        public int? siteid { get; set; }
        public int? authorid { get; set; }
        public int? scantype { get; set; }
    }
}
