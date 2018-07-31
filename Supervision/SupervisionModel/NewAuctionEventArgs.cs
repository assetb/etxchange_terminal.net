using AltaBO;
using System;

namespace SupervisionModel
{
    public class NewAuctionEventArgs:EventArgs
    {
        public Order Order { get; set; }
    }
}
