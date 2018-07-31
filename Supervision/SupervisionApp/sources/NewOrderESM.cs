using System;
using System.Collections.Generic;
using AltaBO.specifics;
using AltaLog;
using MerchantBP;
using SupervisionModel;
using AltaTransport;
using AltaTransport.model;

namespace SupervisionApp
{
    /// <summary>
    /// Monitoring new orderDocument appearance
    /// </summary>
    public class NewOrderESM: EventsSourceMonitorBase
    {
        public NewOrderESM(MonitorBO monitorArgs) : base(monitorArgs) { }

        public event EventHandler<NewOrderEventArgs> NewOrderEvent;


        public override void Start()
        {
                RunTimer();
        }


        protected override void Execute()
        {
            AppJournal.Write(GetType().Name + ":" + System.Reflection.MethodBase.GetCurrentMethod(),"Started...");

            if (!OrderTransport.HasNew()) return;

            List<OrderDocument> orderDocuments = OrderTransport.GetNew();
            foreach(var orderDocument in orderDocuments) {
                var eventArgs = new NewOrderEventArgs(orderDocument);
                if (CustomerSpecifics.IsVostokOrder(orderDocument)) eventArgs.OrderDocument.Customer = CustomersEnum.Vostok;
                if (CustomerSpecifics.IsInkayOrder (orderDocument)) eventArgs.OrderDocument.Customer = CustomersEnum.Inkay;
                OnNewOrderEvent(eventArgs);
            }
        }


        protected virtual void OnNewOrderEvent(NewOrderEventArgs e) {
            NewOrderEvent?.Invoke(this, e);
        }
    }
}
