using SupervisionModel;

namespace SupervisionApp
{
    public class SupervisionProxy
    {
        private MonitorBO _monitorArgs;
        private NewOrderESM _newOrderMonitor;
        private NewOrderEH _newOrderHandler;
        private NewReportESM _newReportMonitor;
        private NewReportEH _newReportHandler;
        private NewAuctionESM _newAuctionMonitor;
        private NewAuctionEH _newAuctionHandler;

        public void Start()
        {
            _monitorArgs = new MonitorBO {
                TimerArgs = new TimerArgs(0, 300000)
            };

            _newOrderMonitor = new NewOrderESM(_monitorArgs);
            _newOrderHandler = new NewOrderEH();
            _newOrderMonitor.NewOrderEvent += _newOrderHandler.NewOrderEventHandler;
            _newOrderMonitor.Start();

            _newReportMonitor = new NewReportESM(_monitorArgs);
            _newReportHandler = new NewReportEH();
            _newReportMonitor.NewReportEvent += _newReportHandler.NewReportEventHandler;
            _newReportMonitor.Start();

            _newAuctionMonitor = new NewAuctionESM(_monitorArgs);
            _newAuctionHandler = new NewAuctionEH(_monitorArgs);
            _newAuctionMonitor.NewAuctionCreatedEvent += _newAuctionHandler.NewAuctionCreatedEventHandler;
            _newAuctionMonitor.Start();
        }


        private bool _isClosed;

        public void Close()
        {            
            _newAuctionMonitor.NewAuctionCreatedEvent -= _newAuctionHandler.NewAuctionCreatedEventHandler;
            _newAuctionMonitor = null;
            _newAuctionHandler = null;
            _newReportMonitor.NewReportEvent -= _newReportHandler.NewReportEventHandler;
            _newReportMonitor.Close();
            _newReportMonitor = null;
            _newReportHandler = null;
            _newOrderMonitor.NewOrderEvent -= _newOrderHandler.NewOrderEventHandler;
            _newOrderMonitor.Close();
            _newOrderMonitor = null;
            _newOrderHandler = null;
            _isClosed = true;
        }


        public bool IsClosed()
        {
            return _isClosed;
        }

    }
}
