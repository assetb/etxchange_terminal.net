using System;
using altaik.baseapp.vm;
using System.Collections.ObjectModel;

namespace AltaBO
{
    [Serializable]
    public class Auction : BaseViewModel
    {
        public string View { get; set; }
        public string Direction { get; set; }

        public int Id { get; set; }

        private DateTime date;
        public DateTime Date {
            get { return date; }
            set { if (value != date) { date = value; RaisePropertyChangedEvent("Date"); } }
        }

        public int sectionId { get; set; }

        private string tradeSection;
        public string TradeSection {
            get { return tradeSection; }
            set { if (value != tradeSection) { tradeSection = value; RaisePropertyChangedEvent("TradeSection"); } }
        }

        public int typeId { get; set; }

        private string type;
        public string Type {
            get { return type; }
            set { if (value != type) { type = value; RaisePropertyChangedEvent("Type"); } }
        }

        private string number;
        public string Number {
            get { return number; }
            set { if (value != number) { number = value; RaisePropertyChangedEvent("Number"); } }
        }

        private int statusId;
        public int StatusId {
            get { return statusId; }
            set { if (value != statusId) { statusId = value; RaisePropertyChangedEvent("StatusId"); } }
        }

        private string status;
        public string Status {
            get { return status; }
            set { if (value != status) { status = value; RaisePropertyChangedEvent("Status"); } }
        }

        private string comments;
        public string Comments {
            get { return comments; }
            set { if (value != comments) { comments = value; RaisePropertyChangedEvent("Comments"); } }
        }

        public bool ndsIncluded { get; set; }
        public int signStatusId { get; set; }

        private int siteId;
        public int SiteId {
            get { return siteId; }
            set { if (value != siteId) { siteId = value; RaisePropertyChangedEvent("SiteId"); } }
        }

        private string site;
        public string Site {
            get { return site; }
            set { if (value != site) { site = value; RaisePropertyChangedEvent("Site"); } }
        }

        public int TraderId { get; set; }

        private string trader;
        public string Trader {
            get { return trader; }
            set { if (value != trader) { trader = value; RaisePropertyChangedEvent("Trader"); } }
        }

        private int customerId;
        public int CustomerId {
            get { return customerId; }
            set { if (value != customerId) { customerId = value; RaisePropertyChangedEvent("CustomerId"); } }
        }

        private string customer;
        public string Customer {
            get { return customer; }
            set { if (value != customer) { customer = value; RaisePropertyChangedEvent("Customer"); } }
        }

        private int ownerId;
        public int OwnerId {
            get { return ownerId; }
            set { ownerId = value; RaisePropertyChangedEvent("OwnerId"); }
        }

        public int BrokerId { get; set; }

        public string BrokerName { get; set; }

        private Broker broker;
        public Broker Broker {
            get { if (broker == null) broker = new Broker(); return broker; }
            set { if (value != broker) { broker = value; RaisePropertyChangedEvent("Broker"); } }
        }

        private string exchangeProvisionSize;
        public string ExchangeProvisionSize {
            get { return exchangeProvisionSize; }
            set { if (value != exchangeProvisionSize) { exchangeProvisionSize = value; RaisePropertyChangedEvent("ExchangeProvisionSize"); } }
        }

        private DateTime applicantsDeadline;
        public DateTime ApplicantsDeadline {
            get { return applicantsDeadline; }
            set { if (value != applicantsDeadline) { applicantsDeadline = value; RaisePropertyChangedEvent("ApplicantsDeadline"); } }
        }

        private DateTime exchangeProvisionDeadline;
        public DateTime ExchangeProvisionDeadline {
            get { return exchangeProvisionDeadline; }
            set { if (value != exchangeProvisionDeadline) { exchangeProvisionDeadline = value; RaisePropertyChangedEvent("ExchangeProvisionDeadline"); } }
        }

        public int RegulationId { get; set; }
        public double InvoicePercent { get; set; }
        public int FilesListId { get; set; }

        private string protocolNumber;
        public string ProtocolNumber {
            get { return protocolNumber; }
            set { if (value != protocolNumber) { protocolNumber = value; RaisePropertyChangedEvent("ProtocolNumber"); } }
        }

        private ObservableCollection<Lot> lots;
        public ObservableCollection<Lot> Lots {
            get { if (lots == null) lots = new ObservableCollection<Lot>(); return lots; }
            set { if (value != lots) { lots = value; RaisePropertyChangedEvent("Lots"); } }
        }

        private ObservableCollection<SupplierOrder> supplierOrders;
        public ObservableCollection<SupplierOrder> SupplierOrders {
            get { if (supplierOrders == null) supplierOrders = new ObservableCollection<SupplierOrder>(); return supplierOrders; }
            set { if (value != supplierOrders) { supplierOrders = value; RaisePropertyChangedEvent("SupplierOrders"); } }
        }

        private ObservableCollection<Applicant> applicants;
        public ObservableCollection<Applicant> Applicants {
            get { if (applicants == null) applicants = new ObservableCollection<Applicant>(); return applicants; }
            set { if (value != applicants) { applicants = value; RaisePropertyChangedEvent("Applicants"); } }
        }

        private ObservableCollection<Procuratory> procuratories;
        public ObservableCollection<Procuratory> Procuratories {
            get { if (procuratories == null) procuratories = new ObservableCollection<Procuratory>(); return procuratories; }
            set { if (value != procuratories) { procuratories = value; RaisePropertyChangedEvent("Procuratories"); } }
        }
    }
}
