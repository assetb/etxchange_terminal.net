using System.Collections.Generic;
using AltaBO;

namespace DocumentFormation
{
    public interface IEnkayDocumentProvider
    {
        string GetAuctionNo();
        Broker GetBroker(Order order);
        string GetExchangeProvisionSize();
        List<Lot> GetLots();
        void CopyQualificationsToBuffer();
        void Close();
    }
}
