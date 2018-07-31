using System;
using altaik.baseapp.vm;
using System.Collections.Generic;

namespace AltaBO
{
    [Serializable]
    public class Order : BaseViewModel
    {
        public int id { get; set; }

        private string title;
        public string Title { get { return title; } set { if (value != title) { title = value; RaisePropertyChangedEvent("Title"); } } }

        private DateTime date;
        public DateTime Date { get { return date; } set { if (value != date) { date = value; RaisePropertyChangedEvent("Date"); } } }

        private string initiator;
        public string Initiator { get { return initiator; } set { if (value != initiator) { initiator = value; RaisePropertyChangedEvent("Initiator"); } } }

        private string organizer;
        public string Organizer { get { return organizer; } set { if (value != organizer) { organizer = value; RaisePropertyChangedEvent("Organizer"); } } }

        private DateTime deadline;
        public DateTime Deadline { get { return deadline; } set { if (value != deadline) { deadline = value; RaisePropertyChangedEvent("Deadline"); } } }

        private Auction auction;
        public Auction Auction { get { if (auction == null) auction = new Auction(); return auction; } set { if (value != auction) { auction = value; RaisePropertyChangedEvent("Auction"); } } }

        private string warranty;
        public string Warranty { get { return warranty; } set { if (value != warranty) { warranty = value; RaisePropertyChangedEvent("Warranty"); } } }

        private string number;
        public string Number { get { return number; } set { if (value != number) { number = value; RaisePropertyChangedEvent("Number"); } } }

        private string status;
        public string Status { get { return status; } set { if (value != status) { status = value; RaisePropertyChangedEvent("Status"); } } }

        public int auctionId { get; set; }
        public int siteId { get; set; }
        public int statusId { get; set; }
        public int filesListId { get; set; }
        public int customerid { get; set; }
        public string exchangeName { get; set; }

        public FileList filesList { get; set; }
    }
}
