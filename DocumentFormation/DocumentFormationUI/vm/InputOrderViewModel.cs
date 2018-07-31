using System;
using System.Collections.Generic;
using altaik.baseapp.vm;
using AltaBO;

namespace DocumentFormation.vm
{
    public class InputOrderViewModel : BaseAppViewModel
    {
        public InputOrderViewModel()
        {
            Order.Date = DateTime.Now;

            Order.PropertyChanged += Order_PropertyChanged;
            Order.Auction.PropertyChanged += Auction_PropertyChanged;

            ProcessingDate = GetShiftedDay(Order.Date, Order.Date.Hour > 12 ? 1 : 0);
        }


        private void Auction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ApplicantsDeadline")) Order.Auction.ExchangeProvisionDeadline = GetShiftedDay(Order.Auction.ApplicantsDeadline, 3);
            if (e.PropertyName.Equals("ExchangeProvisionDeadline")) Order.Auction.Date = GetShiftedDay(Order.Auction.ExchangeProvisionDeadline, 1);
        }


        private void Order_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Date")) ProcessingDate = GetShiftedDay(Order.Date, Order.Date.Hour > 12 ? 1 : 0);
            if (e.PropertyName.Equals("Deadline")) Order.Auction.ApplicantsDeadline = GetShiftedDay(Order.Deadline, 1);
        }


        private Order order;
        public Order Order {
            get { if (order == null) order = new Order(); return order; }
            set { if (value != order) { order = value; RaisePropertyChangedEvent("Order"); } }
        }


        public DateTime processingDate;
        public DateTime ProcessingDate {
            get { return processingDate; }
            set {
                if (value != processingDate) {
                    processingDate = value;
                    RaisePropertyChangedEvent("ProcessingDate");
                    Order.Deadline = GetShiftedDay(ProcessingDate, 3);
                }
            }
        }


        private DateTime GetShiftedDay(DateTime curDate, int nDates)
        {
            if (nDates <= 0) return curDate;
            else {
                var nextDate = curDate;
                nextDate = curDate.AddDays(1);

                if (nextDate.DayOfWeek == DayOfWeek.Saturday) nextDate = nextDate.AddDays(2);
                else if (nextDate.DayOfWeek == DayOfWeek.Sunday) nextDate = nextDate.AddDays(1);
                while (Globals.Holydays.ContainsValue(nextDate)) nextDate = nextDate.AddDays(1);

                return GetShiftedDay(nextDate, nDates - 1);
            }
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>();
        }
    }
}
