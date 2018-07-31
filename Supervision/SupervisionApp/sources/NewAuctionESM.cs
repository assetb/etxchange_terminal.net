using AltaBO;
using AltaTransport;
using System;
using AltaLog;
using SupervisionModel;

namespace SupervisionApp
{
    /// <summary>
    /// Class to monitor appearance of New Auction 
    /// </summary>
    public class NewAuctionESM : EventsSourceMonitorBase
    {
        public NewAuctionESM(MonitorBO monitorArgs) : base(monitorArgs)
        {
        }

        public event EventHandler<NewAuctionEventArgs> NewAuctionCreatedEvent;

        public override void Start()
        {
            AppJournal.Write("Pipe server lictening stating.");

            DesktopClient.StartListening("EventsPipe", ClientRequest);
        }

        protected override void Execute()
        {
        }

        private void ClientRequest(object Obj)
        {
            var auction = Obj as Order;
            if (auction != null) NewAuctionCreatedEvent?.Invoke(this, new NewAuctionEventArgs() { Order = auction });
        }
    }
}
