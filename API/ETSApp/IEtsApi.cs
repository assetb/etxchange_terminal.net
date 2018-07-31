using AltaBO;
using System.Collections.Generic;

namespace EtsApp {
    public interface IEtsApi {
        void Close();
        void CloseConnection();
        void CloseTable();
        bool GetConnection();
        List<PriceOffer> GetPriceOffers();
        bool IsConnected();
        int IsConnectionTable();
        bool OpenConnection();
        int QuotesConnection();
        string GetMessage();
    }
}