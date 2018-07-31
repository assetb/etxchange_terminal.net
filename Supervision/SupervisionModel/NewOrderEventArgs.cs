using AltaTransport.model;
using System;

namespace SupervisionModel
{
    /// <summary>
    /// Arguments for New orderDocument event
    /// </summary>
    public class NewOrderEventArgs: EventArgs
    {
        public NewOrderEventArgs(OrderDocument orderDocument)
        {
            OrderDocument = orderDocument;
        }

        public OrderDocument OrderDocument;
    }
}
