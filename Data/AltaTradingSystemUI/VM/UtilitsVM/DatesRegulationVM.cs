using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;
using AltaBO;
using AltaTradingSystemApp.Services;

namespace AltaTradingSystemUI.VM.UtilitsVM
{
    public class DatesRegulationVM : BaseViewModel
    {
        #region Variables
        public bool IsAuto { get; set; }
        #endregion

        #region Methods
        public DatesRegulationVM(Auction auction)
        {
            if (auction.Id == 0) Init(DateTime.Now);
            else {
                Init();
                UploadExistDates(auction.RegulationId, auction.Date);
            }
        }


        private void Init()
        {
            Order = new Order();
            Order.Auction = new Auction();
            IsAuto = true;

            AutoDate();
        }


        private void Init(DateTime orderDate)
        {
            Init();

            Order.Date = orderDate;
            ProcessingDate = GetShiftedDay(Order.Date, Order.Date.Hour > 12 ? 1 : 0);
        }


        public void AutoDate()
        {
            if (IsAuto) {
                Order.PropertyChanged += Order_PropertyChanged;
                Order.Auction.PropertyChanged += Auction_PropertyChanged;
            } else {
                Order.PropertyChanged -= Order_PropertyChanged;
                Order.Auction.PropertyChanged -= Auction_PropertyChanged;
            }
        }


        private void Auction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ApplicantsDeadline")) Order.Auction.ExchangeProvisionDeadline = GetShiftedDay(Order.Auction.ApplicantsDeadline, 3);

            if (e.PropertyName.Equals("ExchangeProvisionDeadline")) Order.Auction.Date = GetShiftedDay(Order.Auction.ExchangeProvisionDeadline, 1);
        }


        private void Order_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Date")) ProcessingDate = GetShiftedDay(Order.Date, Order.Date.Hour > 17 ? 1 : 0);

            if (e.PropertyName.Equals("Deadline")) Order.Auction.ApplicantsDeadline = GetShiftedDay(Order.Deadline, 1);
        }


        private DateTime GetShiftedDay(DateTime curDate, int nDates)
        {
            if (nDates <= 0) return curDate;
            else {
                var nextDate = curDate;
                nextDate = curDate.AddDays(1);

                if (nextDate.DayOfWeek == DayOfWeek.Saturday) nextDate = nextDate.AddDays(2);
                else if (nextDate.DayOfWeek == DayOfWeek.Sunday) nextDate = nextDate.AddDays(1);

                while (Globals.Holydays.Count(h => h.Value.Year == nextDate.Year && h.Value.Month == nextDate.Month && h.Value.Day == nextDate.Day) > 0) nextDate = nextDate.AddDays(1);

                return GetShiftedDay(nextDate, nDates - 1);
            }
        }


        public void UploadExistDates(int regulationId, DateTime auctionDate)
        {
            var regulation = AuctionService.ReadRegulation(regulationId);

            if (regulation == null) return;

            IsAuto = false;

            AutoDate();

            Order.Date = regulation.openDate;
            Order.Deadline = regulation.applyDeadLine;

            SetOrderDeadLine(16, 0);

            Order.Auction.Date = auctionDate;
            Order.Auction.ApplicantsDeadline = regulation.applicantsDeadLine;
            ProcessingDate = regulation.openDate;
            Order.Auction.ExchangeProvisionDeadline = regulation.provisionDeadLine;

            IsAuto = true;

            AutoDate();
        }


        private void SetOrderDeadLine(int hours, int minutes)
        {
            Order.Deadline = Order.Deadline.AddHours(-Order.Deadline.Hour);
            Order.Deadline = Order.Deadline.AddHours(hours);
            Order.Deadline = Order.Deadline.AddMinutes(-Order.Deadline.Minute);
            Order.Deadline = Order.Deadline.AddMinutes(minutes);
        }


        public int CreateRegulation()
        {
            return AuctionService.CreateRegulation(FillRegulation());
        }


        public void UpdateRegulation(int regulationId)
        {
            AuctionService.UpdateRegulation(FillRegulation(), regulationId);
        }


        private Regulation FillRegulation()
        {
            Regulation regulation = new Regulation() {
                openDate = Order.Date,
                closeDate = Order.Auction.Date,
                applyDate = Order.Date,
                applyDeadLine = Order.Deadline,
                applicantsDeadLine = Order.Auction.ApplicantsDeadline,
                provisionDeadLine = Order.Auction.ExchangeProvisionDeadline
            };

            return regulation;
        }
        #endregion

        #region Bindings
        private DateTime _processingDate;
        public DateTime ProcessingDate {
            get { return _processingDate; }
            set {
                if (value != _processingDate) {
                    _processingDate = value;

                    RaisePropertyChangedEvent("ProcessingDate");

                    if (IsAuto) {
                        Order.Deadline = GetShiftedDay(ProcessingDate, 3);

                        SetOrderDeadLine(16, 0);
                    }
                }
            }
        }


        private Order _order;
        public Order Order {
            get { return _order; }
            set { _order = value; RaisePropertyChangedEvent("Order"); }
        }
        #endregion
    }
}