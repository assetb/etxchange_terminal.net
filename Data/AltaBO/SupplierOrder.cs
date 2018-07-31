using altaik.baseapp.vm;
using System;
using System.Collections.Generic;

namespace AltaBO
{
    public class SupplierOrder : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BIN { get; set; }
        public string Kbe { get; set; }
        public string CompanyDirector { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string Phones { get; set; }
        public string BrokerName { get; set; }
        public string BrokerBIN { get; set; }
        public string BrokerAddress { get; set; }
        public string BrokerPhones { get; set; }
        public string BrokerIIK { get; set; }
        public string BrokerKbe { get; set; }
        public string BrokerCode { get; set; }
        public string BrokerBankName { get; set; }
        public string BrokerBIK { get; set; }
        public string Trader { get; set; }
        public string ContractNum { get; set; }
        public DateTime ContractDate { get; set; }
        public int SupplierId { get; set; }
        public decimal MinimalPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string IIK { get; set; }
        public string BIK { get; set; }
        public string BankName { get; set; }
        public int brokerid { get; set; }
        public int? contractId { get; set; }
        public int auctionId { get; set; }
        public string auctionNumber { get; set; }
        public int? fileListId { get; set; }
        public DateTime date { get; set; }

        public List<RequestedDoc> RequestedDocs { get; set; }
        public Status status { get; set; }
        public Supplier supplier { get; set; }
        public List<Lot> lots { get; set; }
    }
}
