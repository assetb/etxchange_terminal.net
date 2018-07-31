using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaMySqlDB.model;
using AltaMySqlDB.service;
using AltaBO;

namespace ETSApp.Services
{
    public static class DBManager
    {
        private static IDataManager dataManager = new EntityContext();


        public static void CreateStockQuote(PriceOffer priceOffer)
        {            
            dataManager.CreateStockQuotes(priceOffer.lotCode,Convert.ToDecimal(priceOffer.lotPriceOffer), priceOffer.firmName, priceOffer.offerTime);
        }
    }
}
