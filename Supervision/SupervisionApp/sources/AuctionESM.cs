using System;
using System.Timers;
using AltaBO;
using SupervisionModel;

namespace SupervisionApp
{
    /// <summary>
    /// Class to monitor events being during auction.
    /// Auction should be set.
    /// </summary>
    public class AuctionESM : EventsSourceMonitorBase
    {
        public AuctionESM(MonitorBO monitorArgs) : base(monitorArgs) { }

        private Timer _timer;
        private Order _order;


        public event EventHandler OrderDeadlineEvent;
        public event EventHandler ApplicantsDeadlineEvent;
        public event EventHandler ExchangeProvisionDeadlineEvent;
        public event EventHandler AuctionStartedEvent;
        public event EventHandler AuctionEndedEvent;

        //private bool OrderDeadlineEventRaised;
        //private bool ApplicantsDeadlineEventRaised;
        //private bool ExchangeProvisionDeadlineEventRaised;
        //private bool AuctionStartedEventRaised;
        //private bool AuctionEndedEventRaised;


        public void SetAuction(Order order) { _order = order; }

        public override void Start() {
            if (_order == null) return;
            _timer = new Timer(MonitorArgs.TimerArgs.period) {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) { LookAt(); }


        private readonly TimeSpan auctionStartTime = new TimeSpan(17, 0, 0);
        private readonly TimeSpan auctionEndTime = new TimeSpan(19, 0, 0);
        private readonly TimeSpan deadlineTime = new TimeSpan(10, 0, 0);

        private void LookAt() {
            var today = DateTime.Today;
            if (DateTime.Now.CompareTo(_order.Deadline.Date + deadlineTime) < 0) {
                OrderDeadlineEvent?.Invoke(this, new EventArgs());
            }
            if ((DateTime.Now.CompareTo(_order.Deadline.Date + deadlineTime) > 0) && (DateTime.Now.CompareTo(_order.Auction.ApplicantsDeadline.Date + deadlineTime) < 0)) {
                ApplicantsDeadlineEvent?.Invoke(this, new EventArgs());
            }
            if ((DateTime.Now.CompareTo(_order.Auction.ApplicantsDeadline.Date + deadlineTime) > 0) && (DateTime.Now.CompareTo(_order.Auction.ExchangeProvisionDeadline.Date + deadlineTime) < 0)) {
                ExchangeProvisionDeadlineEvent?.Invoke(this, new EventArgs());
            }
            if ((today.CompareTo(_order.Auction.Date.Date) == 0) && (DateTime.Now.TimeOfDay.CompareTo(auctionStartTime) < 0)) {
                AuctionStartedEvent?.Invoke(this, new EventArgs());
            }
            if ((today.CompareTo(_order.Auction.Date.Date) == 0) && (DateTime.Now.TimeOfDay.CompareTo(auctionEndTime) > 0)) {
                AuctionEndedEvent?.Invoke(this, new EventArgs());
            }
        }


        protected override void Execute() { throw new NotImplementedException(); }
    }
}
