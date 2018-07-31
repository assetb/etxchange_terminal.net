using SupervisionModel;
using System;
using System.Collections.Generic;

namespace SupervisionApp
{
    /// <summary>
    /// Class to handle New Auction Created Event.
    /// Monitor inside auction started.
    /// </summary>
    public class NewAuctionEH
    {
        private readonly MonitorBO monitorArgs;
        private int auctionCount;
        private List<AuctionESM> auctionSources;
        private List<AuctionEH> auctionHandlers;


        public NewAuctionEH(MonitorBO monitorArgs)
        {
            this.monitorArgs = monitorArgs;
        }


        public void NewAuctionCreatedEventHandler(object sender, NewAuctionEventArgs eventArgs)
        {
            if(auctionCount == 0) {
                auctionSources = new List<AuctionESM>();
                auctionHandlers = new List<AuctionEH>();
            }

            var source = new AuctionESM(monitorArgs);
            source.SetAuction(eventArgs.Order);
            var handler = new AuctionEH();

            source.OrderDeadlineEvent += handler.OrderDeadlineEventHandler;
            source.ApplicantsDeadlineEvent += handler.ApplicantsDeadlineEventHandler;
            source.ExchangeProvisionDeadlineEvent += handler.ExchangeProvisionDeadlineEventHandler;
            source.AuctionStartedEvent += handler.AuctionStartedEventHandler;
            source.AuctionEndedEvent += Source_AuctionEndedEvent;

            auctionSources.Add(source);
            auctionHandlers.Add(handler);
            auctionCount++;

            source.Start();
        }

        private void Source_AuctionEndedEvent(object sender, EventArgs e)
        {
            var source = sender as AuctionESM;
            var index = GetAuctionIndex(source);
            if(index > -1) {
                auctionSources[index].OrderDeadlineEvent -= auctionHandlers[index].OrderDeadlineEventHandler;
                auctionSources[index].ApplicantsDeadlineEvent -= auctionHandlers[index].ApplicantsDeadlineEventHandler;
                auctionSources[index].ExchangeProvisionDeadlineEvent -= auctionHandlers[index].ExchangeProvisionDeadlineEventHandler;
                auctionSources[index].AuctionStartedEvent -= auctionHandlers[index].AuctionStartedEventHandler;
                auctionSources[index].Close();
                auctionHandlers.RemoveAt(index);
                auctionSources.RemoveAt(index);
            }
        }


        private int GetAuctionIndex(AuctionESM source)
        {
            for(var i = 0; i < auctionCount; i++) {
                if (auctionSources[i].Equals(source)) {
                    return i;
                }
            }
            return -1;
        }


        public void Close()
        {
            for(var i = 0; i<auctionCount; i++) {
                var source = auctionSources[i];
                var handler = auctionHandlers[i];
                source.OrderDeadlineEvent -= handler.OrderDeadlineEventHandler;
                source.ApplicantsDeadlineEvent -= handler.ApplicantsDeadlineEventHandler;
                source.ExchangeProvisionDeadlineEvent -= handler.ExchangeProvisionDeadlineEventHandler;
                source.AuctionStartedEvent -= handler.AuctionStartedEventHandler;
                source.Close();
            }
            auctionSources.Clear();
            auctionHandlers.Clear();
        }

    }
}
