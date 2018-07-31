using System;
using altaik.baseapp.ext;
using AltaWindowsNotification;

namespace SupervisionApp
{
    /// <summary>
    /// Class to handle events being during auction.
    /// </summary>
    public class AuctionEH
    {

        public void AuctionEndedEventHandler(object sender, EventArgs e)
        {
        }

        public void AuctionStartedEventHandler(object sender, EventArgs e)
        {
            var toast = new ToastNotification();
            toast.Show(AuctionNotificationsEnum.AuctionStarted.GetName());
        }

        public void ExchangeProvisionDeadlineEventHandler(object sender, EventArgs e)
        {
            var toast = new ToastNotification();
            toast.Show(AuctionNotificationsEnum.ExchangeProvisionDeadline.GetName());
        }

        public void ApplicantsDeadlineEventHandler(object sender, EventArgs e)
        {
            var toast = new ToastNotification();
            toast.Show(AuctionNotificationsEnum.ApplicantsDeadline.GetName());
        }

        public void OrderDeadlineEventHandler(object sender, EventArgs e)
        {
            var toast = new ToastNotification();
            toast.Show(AuctionNotificationsEnum.OrderDeadline.GetName());
        }
    }
}
