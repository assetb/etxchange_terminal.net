using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;

namespace AltaTradingSystemApp.Services
{
    public static class DictionariesService
    {
        public static List<Status> ReadStatuses()
        {
            return DataManagerService.TradingInstance().ReadStatuses();
        }


        public static List<Section> ReadSections()
        {
            return DataManagerService.TradingInstance().ReadSections();
        }


        public static List<AuctionType> ReadTypes()
        {
            return DataManagerService.TradingInstance().ReadTypes();
        }


        public static List<Site> ReadSites()
        {
            return DataManagerService.TradingInstance().ReadSites();
        }


        public static List<Customer> ReadCustomers()
        {
            return DataManagerService.TradingInstance().ReadCustomers();
        }


        public static List<Supplier> ReadSuppliers()
        {
            return DataManagerService.TradingInstance().ReadSuppliers();
        }


        public static List<Broker> ReadBrokers()
        {
            return DataManagerService.TradingInstance().ReadBrokers();
        }


        public static List<Trader> ReadTraders()
        {
            return DataManagerService.TradingInstance().ReadTraders();
        }


        public static List<Unit> ReadUnits()
        {
            return DataManagerService.TradingInstance().ReadUnits();
        }


        public static List<Contract> ReadContracts(int companyId, int brokerId)
        {
            return DataManagerService.TradingInstance().ReadContracts(companyId, brokerId);
        }


        public static List<RatesList> ReadRatesLists(int contractId)
        {
            return DataManagerService.TradingInstance().ReadRatesLists(contractId);
        }


        public static List<DocumentType> ReadDocumentTypes()
        {
            return DataManagerService.TradingInstance().ReadDocumentTypes();
        }
    }
}
