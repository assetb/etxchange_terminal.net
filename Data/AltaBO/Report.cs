namespace AltaBO
{
    public class Report:IAltaBO
    {
        public string CustomerName;
        public string CustomerBrokerCode;
        public string IssueCode;
        public string IssueInitialPrice;
        public string Volume;
        public string ParticipantPair;
        public string BrokerFullName;

        public string Id { get { return Commons.id; } set { Commons.id = value; } }
        public string Name { get { return Commons.name; } set { Commons.name = value; } }
        public string DateTo { get { return Commons.dateTo; } set { Commons.dateTo = value; } }
        public string BrokerName;

        public ReportCommonFields Commons;
        public string Code;
        public string Number;
        public string ProductName;
        public string Moment;
        public string Qty;
        public string Price;
        public string Amt;
        public string ClientName;
        public string Inn;
        public string ContrCode;
        public string Director;
    }
}
