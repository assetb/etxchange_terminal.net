using System;
using SupervisionModel;

namespace SupervisionApp
{
    public class DBEventsSource : EventsSourceMonitorBase
    {
        public DBEventsSource(MonitorBO monitorArgs) : base(monitorArgs){}

        //public event EventHandler AuctionCreatedEvent;

        public override void Start()
        {

        }

        protected override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
