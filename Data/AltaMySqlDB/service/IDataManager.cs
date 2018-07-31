using AltaBO;
using AltaBO.archive;
using AltaBO.reports;
using AltaBO.views;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.model.views;
using System;
using System.Collections.Generic;

namespace AltaMySqlDB.service
{
    public interface IDataManager : IDisposable
    {
        #region User
        int CreateUser(int personId, string login, string pass, int roleId);
        int CreateOrder(Order order);
        List<DocumentView> ReadDocuments(int fileListId);
        List<SupplierOrderView> ReadSupplierOrders(int auctionId);
        List<Lot> ReadLots(int auctionId);
        List<Status> ReadStatuses();
        Order ReadOrder(int auctionId);
        List<Order> ReadOrders(int statusId);
        Lot ReadLot(int lotId);
        SupplierOrderView ReadSupplierOrder(int supplierOrderId);
        User GetUser(int userId);
        List<Procuratory> ReadProcuratories(int auctionId, int supplierId);
        User GetUser(string login);
        List<Section> ReadSections();
        User GetUser(string login, string password);
        Procuratory ReadProcuratory(int procuratoryId);
        List<LotsExtended> ReadLotExtended(int lotId);
        List<Auction> ReadAuctions(DateTime fromDate, DateTime toDate, int statusId);
        int CreateLot(Lot lot);
        List<AuctionType> ReadTypes();
        int CreateAuction(Auction auction);
        string GetAccessStringByUser(int userid);
        List<Supplier> ReadSuppliers();
        void UpdateLot(Lot lot);
        List<Site> ReadSites();
        List<User> GetUsersByCompany(int companyId);
        void DeleteLot(int lotId);
        List<DebtorsList> ReadDebtorsList(string customerName, string supplierName);
        void UpdateAuction(Auction auction);
        String GetPasswordUser(int userId);
        List<DebtorsList> ReadDebtorsList();
        int CreateRegulation(Regulation regulation);
        List<Customer> ReadCustomers();
        #endregion

        #region Auction
        Auction GetAuction(int id);
        List<Contract> ReadContracts(int companyId, int brokerId);
        int GetAuctionsCount(string numberOrProduct = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? site = default(int?), int? supplierId = default(int?), int? statusId = default(int?), int? winner = default(int), int? brokerId = default(int?), int? traderId = default(int?));
        List<DocumentType> ReadDocumentTypes();
        List<RatesList> ReadRatesLists(int contractId);
        List<Unit> ReadUnits();
        void UpdateRegulation(Regulation regulation, int regulationId);
        List<DebtorDetails> ReadDebtorDetails(int supplierId, int statusId);
        List<ContractsReportView> ReadContractsReport(DateTime fromDate, DateTime toDate, string searchQuery);
        void DeleteDocument(int documentId);
        List<Document> SearchDocumentsInTS(string searchQuery);

        /// <summary>
        /// Запрос на получения списка аукционов.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="countItems">Кол-во запрашиваемых элементов</param>
        /// <param name="numberOrProduct">Искомый текст. Поиск происходит по номеру аукциона и/или наименование лота.</param>
        /// <param name="customerId">Фильтрация по идентификатору заказчика</param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="site"></param>
        /// <param name="supplierId"></param>
        /// <param name="statusId"></param>
        /// <param name="winner"></param>
        /// <param name="orderBy">Сортировка
        /// </param>
        /// <param name="isdesc">Вид сортировки. false - по возврастанию, true - по убыванию.</param>
        /// <returns></returns>
        List<Auction> GetAuctions(int page, int countItems, string numberOrProduct = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? site = default(int?), int? supplierId = default(int?), int? statusId = default(int?), int? winner = default(int), string orderBy = null, bool isdesc = false, int? brokerId = default(int?), int? traderId = default(int?));

        List<Status> GetStatuses();
        List<Site> GetCatalogSites();

        List<AltaBO.AuctionResult> GetAuctionsResult();
        List<SupplierOrder> GetSuplliersOrders(int auctionId);
        bool AddSupllierOrder(int auctionId, SupplierOrder supplierOrder);

        bool AddSupplier(int auctionId, int supplierId);
        bool RejectSupplier(int auctionId, int supplierId);
        #endregion

        #region Lot
        List<Lot> GetLots(int? auctionid = default(int?));
        Lot GetLot(int lotId);
        List<LotsExtended> GetLotsExtended(int lotId);
        int UpdateLotsExtended(LotsExtended lotsExtended);
        #endregion

        #region Supplier
        int GetSuppliersCount(string textSearch = null);
        int GetSuppliersCount(string textSearch = null, int method = 1);
        List<Supplier> GetSuppliers(int page, int countItems, string textSearch = null);
        List<SupplierWithProduct> GetSuppliersWithProduct(int page, int countItems, out int count, string searchProduct = null);
        Supplier GetSupplier(int supplierId);
        Company GetCompanySupplier(int supplierId);
        int GetSupplierId(int companyId);
        Supplier GetSupplierByCompanyId(int companyId);
        Supplier GetSupplierByUserId(int userId);
        List<Supplier> GetSuppliersByParam(int page, int countItems, string textSearch = null, int method = 1);
        #endregion

        #region Product
        Product GetProduct(int idProduct);
        List<Product> GetProducts(int page, int countItems, string textSearch = null, int? companyId = default(int?), int? supplierId = default(int?));
        int GetProductsCount(string textSearch = null, int? companyId = default(int?), int? supplierId = default(int?));
        List<Product> GetAllProducts(int? companyId = default(int?));
        List<ProductCompany> GetProductsCompany(int companyId);
        bool RemoveProductCopmany(int productId, int companyid);
        bool AddProductFromCompany(string name, int companyId, int? fileid = default(int?), string description = null);
        List<ProductCompanyEF> GetCompaniesWithProduct(string queryTxt);
        #endregion

        #region Order
        List<Order> GetOrders(int? auctionId = default(int?), string searchText = null, DateTime? fromDate = null, DateTime? toDate = null, int? customerId = default(int?), int statusId = default(int));
        int GetOrdersCount(int? auctionId = default(int), string searchText = null, DateTime? fromDate = null, DateTime? toDate = null, int? customerId = default(int?), int statusId = default(int));
        Order GetOrder(int id);
        int CreateOrder(Order order, int initiatorId);
        #endregion

        #region Company
        /// <summary>
        /// Обновление данных компании
        /// </summary>
        /// <param name="company">Бизнес объект компании</param>
        /// <returns>В случае успешной операции возвращает true, иначе false</returns>
        bool UpdateCompany(Company company);
        List<Company> GetCompanies(string searchText = null, string bin = null);
        //List<Company> ReadCompanies();
        Company GetCompany(int id);
        Company GetCompanyByUserId(int userId);
        int GetCustomerId(int companyId);
        List<CompanyWithProducts> GetCompanyWithProduct(int page, int countItems, string productName = null);
        int GetCompanyWithProductCount(string productName = null);
        bool AddFileListId(int companyId, int fileListId);
        bool AddEmployee(int companyId, int personId, string position);
        List<CompaniesWithContractView> GetSuppliersWithContract();
        #endregion

        #region Archive
        #region File
        int PutDocument(DocumentRequisite documentRequisite, int? filesListId = null);
        int PutSimpleDocument(DocumentRequisite documentRequisite);
        DocumentRequisite GetFileParams(int fileId);
        #endregion

        #region FilesList
        int CreateFilesList(string description = null);
        List<DocumentRequisite> GetFilesFromList(int filesListId);
        bool RemoveDocumentInList(int listId, int fileId);
        bool AppendFileToList(int listId, int fileId);
        DocumentRequisite AddDocumentRequisite(DocumentRequisite documentRequisite);
        string GetFileListDescription(int fileListId);
        List<DocumentRequisite> GetFiles(int fileListId = 0, List<int> types = null);
        #endregion
        #endregion

        #region Applicant
        List<Applicant> GetApplicants(int auctionId);
        #endregion

        #region Site
        List<Site> GetSites();
        #endregion

        #region Customer
        Customer GetCustomer(int customerId);
        int GetCustomersCount(string search = null);
        List<Customer> GetCustomers(int skip, int countItems, string search = null);
        List<Company> GetCustomerEnum();
        #endregion

        #region Broker
        List<Broker> GetBrokersCompany(int companyId);
        Broker GetBroker(int brokerId);
        List<BrokerEF> GetBrokers();
        ConficurationsBroker GetConfiguration(int brokerId);
        List<Broker> ReadBrokers();
        #endregion

        #region Report
        List<TechSpecReportBO> GetTechSpecReport(int customerId, DateTime auctionStartDate, DateTime auctionEndDate, List<int> siteId, List<int> statusId, int limitFrom = 0, int limitTo = 0, short sortMode = 0, string sortColumnName = null);
        List<TechSpecReportBO> GetTechSpecReportByOrderDate(int customerId, DateTime orderStartDate, DateTime orderEndDate, List<int> siteId, List<int> statusId, int limitFrom = 0, int limitTo = 0, short sortMode = 0, string sortColumnName = null);
        int CountTechSpecReport(int customerId, DateTime startDate, DateTime endDate, List<int> siteId, List<int> statusId);
        FinalReport GetFinalReport(int auctionId, int lotId, int supplierId);
        List<FinalReport> GetFinalReports(int auctionId, int supplierId);
        List<FinalReportPlmtl> GetFinalReportPlmtl(DateTime startDate, DateTime endDate);
        List<AltaBO.reports.AuctionResult> ReadAuctionsResultReport();
        List<FinalReport> GetEconomyReport(int page, int countItems, string number = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? siteid = default(int?), int? brokerid = default(int?), int? typeid = default(int?));
        int GetEconomyReportCount(string number = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? siteid = default(int?), int? brokerid = default(int?), int? typeid = default(int?));
        void UpdateFinalReport(int finalReportId, int statusId);

        List<DealNumberInfo> ReadDealNumbersInfo();
        #endregion

        #region Contract
        List<Contract> GetContractsByCompany(int companyId, int brokerid = 0, int siteid = 0);
        #endregion

        #region SupplierOrder
        SupplierOrder GetSupplierOrder(int id);
        SupplierOrder GetSupplierOrder(int auctionId, int supplierId);
        Boolean UpdateStatusOnSupplierOrder(int supplierOrderId, int statusId);
        List<SupplierOrderEF> GetSupplierOrders(int auctionId, int exceptStatusId);
        SupplierOrder ReadSupplierOrder(int supplierId, int auctionId);
        #endregion

        #region SupplierJournal
        SupplierJournal GetSupplierJournal(int id);
        SupplierJournal GetSupplierJournal(int brokerId, int supplierId);
        #endregion

        #region Notification
        List<Notification> GetNotifications(int supplierId = 0, int auctionId = 0, int eventId = 0, int belon = 0);
        #endregion

        #region Procuratory
        Procuratory GetProcuratory(int auctionId, int id);
        bool AddProcuratory(int auctionId, Procuratory procuratory);
        bool AddProcuratories(int auctionId, List<Procuratory> procuratories);
        #endregion

        #region Qualifications
        List<Qualification> GetQualifications(int auctionId = default(int));
        #endregion

        #region QualificationsDictionary
        List<QualificationDictionary> GetQualificationDictionary();
        #endregion

        #region MailDeliveryException
        int AddMailDeliveryException(string lotName, DateTime date, string customer);
        MailDeliveryException GetMailDeliveryException(string lotName, DateTime date, string customer);
        #endregion

        #region Cases
        int AddCase(int year, int brokerId, string caseName);
        List<CaseEF> GetCases(int year, int brokerId);
        bool UpdateCase(int id, string caseName);
        #endregion

        #region Documents
        List<DocumentEF> GetDocumentsWithArchiveNumbersInVolume(int volumeId);
        bool UpdateDocumentWithArchiveNumber(int id, int archiveNumberId);
        DocumentEF GetDocument(int documentId);
        #endregion

        #region ArchiveNumbers
        int AddArchiveNumber(ArchiveNumberEF archiveNumberItem);
        bool UpdateArchiveNumber(ArchiveNumberEF archiveNumberInfo);
        int GetDocsCountInVolume(int volumeId);
        int GetDocsCountInCase(int caseId);
        int CheckDocInVolume(int volumeId, int archiveNumberId);
        #endregion

        #region Volumes
        List<VolumeEF> GetVolumes(int caseId);
        int AddVolume(string volumeName, int caseId);
        bool UpdateVolume(int id, string volumeName);
        bool SetVolumeStatus(int volumeId, int statusId);
        bool SetVolumeName(int volumeId, string name);
        int GetVolumeFirstDocNum(int volumeId);
        int GetVolumeLastDocNum(int volumeId);
        #endregion

        #region Person
        int CreatePerson(Person person);
        #endregion

        #region Traders
        List<Trader> ReadTraders();
        #endregion

        #region Storage Procedure
        List<AnaliticCountStatus> CustCount(int customerId, DateTime startDate, DateTime endDate);
        List<AnaliticCountStatus> SupCount(int supplierId, DateTime startDate, DateTime endDate);
        #endregion

        #region Online StockQuotes
        void CreateStockQuotes(string lotCode, decimal priceOffer, string brokerCode, DateTime offerTime);
        #endregion

        #region RatesList
        RatesList ReadRatesList(int contractId, int siteId);
        #endregion

        #region Rates
        List<Rate> ReadRates(int ratesListId);
        #endregion

        #region Regulations
        Regulation ReadRegulation(int regulationId);
#endregion
    }
}