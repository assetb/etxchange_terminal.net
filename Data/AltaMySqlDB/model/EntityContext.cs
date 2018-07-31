using AltaBO;
using AltaBO.archive;
using AltaBO.reports;
using AltaBO.views;
using AltaMySqlDB.Helpers;
using AltaMySqlDB.model.catalogs;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.model.views;
using AltaMySqlDB.service;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace AltaMySqlDB.model
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class EntityContext : DbContext, IDataManager, IDisposable
    {
        #region Initialization EntityContext

        #region AutoMapping
        private static void InitializeAutoMapper()
        {
            Mapper.Initialize(config => {
                #region map User
                var userMap = config.CreateMap<UserEF, User>();
                userMap.ForMember(desc => desc.FullName, opt => opt.MapFrom(src => src.person != null ? src.person.name : ""));
                userMap.ForMember(desc => desc.Role, opt => opt.MapFrom(src => src.role.name));
                var userReverseMap = userMap.ReverseMap();
                userReverseMap.ForMember(desc => desc.person, opt => opt.Ignore());
                #endregion

                #region map Auction
                var auctionMap = config.CreateMap<AuctionEF, Auction>();
                auctionMap.ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.id));
                auctionMap.ForMember(desc => desc.Site, opt => opt.MapFrom(src => src.site != null ? src.site.name : null));
                auctionMap.ForMember(desc => desc.Type, opt => opt.MapFrom(src => src.type != null ? src.type.name : null));
                auctionMap.ForMember(desc => desc.StatusId, opt => opt.MapFrom(src => src.statusid));
                auctionMap.ForMember(desc => desc.Status, opt => opt.MapFrom(src => src.status.name));
                auctionMap.ForMember(desc => desc.Trader, opt => opt.MapFrom(src => src.trader != null && src.trader.person != null ? src.trader.person.name : null));
                auctionMap.ForMember(desc => desc.CustomerId, opt => opt.MapFrom(src => src.customer != null ? src.customer.id : 0));
                auctionMap.ForMember(desc => desc.Customer, opt => opt.MapFrom(src => src.customer != null && src.customer.company != null ? src.customer.company.name : null));
                auctionMap.ForMember(desc => desc.Broker, opt => opt.MapFrom(src => src.broker));
                auctionMap.ForMember(desc => desc.ApplicantsDeadline, opt => opt.MapFrom(src => src.regulation.applicantsdeadline));
                auctionMap.ForMember(desc => desc.ExchangeProvisionDeadline, opt => opt.MapFrom(src => src.regulation.provisiondeadline));
                auctionMap.ForMember(desc => desc.Comments, opt => opt.Ignore());
                auctionMap.ForMember(desc => desc.SupplierOrders, opt => opt.Ignore());
                var auctionViewMap = config.CreateMap<AuctionsView, Auction>();

                var auctionsResultView = config.CreateMap<AuctionsResultView, AltaBO.AuctionResult>();
                #endregion

                #region map BrokerJournal
                var brokerJournal = config.CreateMap<BrokersJournalEF, BrokerJournal>();
                #endregion

                #region map Broker
                var brokerMap = config.CreateMap<BrokerEF, Broker>();
                brokerMap.ForMember(desc => desc.Code, opt => opt.MapFrom(src => src.brokersJournal.code));
                brokerMap.ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.name));
                brokerMap.ForMember(desc => desc.Requisites, opt => opt.MapFrom(src => src.requisites));
                #endregion

                #region map Lot
                var lotMap = config.CreateMap<LotEF, Lot>();
                lotMap.ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.description));
                lotMap.ForMember(desc => desc.Quantity, opt => opt.MapFrom(src => src.amount));
                lotMap.ForMember(desc => desc.Unit, opt => opt.MapFrom(src => src.unit != null ? src.unit.name : null));
                var lotReverseMap = lotMap.ReverseMap();
                lotReverseMap.ForMember(desc => desc.description, opt => opt.MapFrom(src => src.Name));
                lotReverseMap.ForMember(desc => desc.amount, opt => opt.MapFrom(src => src.Quantity));
                lotReverseMap.ForMember(desc => desc.unit, opt => opt.Ignore());
                lotReverseMap.ForMember(desc => desc.lotsextended, opt => opt.Ignore());

                var lotsExtended = config.CreateMap<LotsExtendedEF, LotsExtended>();
                #endregion

                #region map Supplier
                var supplierMap = config.CreateMap<SupplierEF, Supplier>();
                supplierMap.ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.company != null ? src.company.name : null));
                supplierMap.ForMember(desc => desc.BIN, opt => opt.MapFrom(src => src.company != null ? src.company.bin : null));
                supplierMap.ForMember(desc => desc.Address, opt => opt.MapFrom(src => src.company != null ? src.company.addressactual : null));
                supplierMap.ForMember(desc => desc.Country, opt => opt.MapFrom(src => src.company != null && src.company.countries != null ? src.company.countries.name : null));
                supplierMap.ForMember(desc => desc.Contacts, opt => opt.MapFrom(src => src.company != null ? src.company.telephone.Trim() : null));
                supplierMap.ForMember(desc => desc.Emails, opt => opt.MapFrom(src => src.company != null ? src.company.email.Trim() : null));

                var supplierWithProductsMap = config.CreateMap<SupplierEF, SupplierWithProduct>();
                supplierWithProductsMap.ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.company != null ? src.company.name : null));
                supplierWithProductsMap.ForMember(desc => desc.BIN, opt => opt.MapFrom(src => src.company != null ? src.company.bin : null));
                supplierWithProductsMap.ForMember(desc => desc.companyId, opt => opt.MapFrom(src => src.company != null ? src.company.id : default(int)));
                supplierWithProductsMap.ForMember(desc => desc.Address, opt => opt.MapFrom(src => src.company != null ? src.company.addressactual : null));
                supplierWithProductsMap.ForMember(desc => desc.Country, opt => opt.MapFrom(src => src.company != null && src.company.countries != null ? src.company.countries.name : null));
                supplierWithProductsMap.ForMember(desc => desc.Contacts, opt => opt.MapFrom(src => src.company != null ? src.company.telephone.Trim() : null));
                supplierWithProductsMap.ForMember(desc => desc.Emails, opt => opt.MapFrom(src => src.company != null ? src.company.email.Trim() : null));
                supplierWithProductsMap.ForMember(desc => desc.products, opt => opt.MapFrom(src => src.company.products));

                var supplierWithProductsFromCompanyMap = config.CreateMap<CompanyEF, SupplierWithProduct>();
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.name));
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.BIN, opt => opt.MapFrom(src => src.bin));
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.companyId, opt => opt.MapFrom(src => src.id));
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.Address, opt => opt.MapFrom(src => src.addressactual));
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.Country, opt => opt.MapFrom(src => src.countries != null ? src.countries.name : null));
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.Contacts, opt => opt.MapFrom(src => src.telephone.Trim()));
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.Emails, opt => opt.MapFrom(src => src.email.Trim()));
                supplierWithProductsFromCompanyMap.ForMember(desc => desc.products, opt => opt.MapFrom(src => src.products));
                #endregion

                #region map Customer
                var customerMap = config.CreateMap<CustomerEF, Customer>();
                customerMap.ForMember(desc => desc.companyId, opt => opt.MapFrom(src => src.companiesid));
                customerMap.ForMember(desc => desc.name, opt => opt.MapFrom(src => src.company != null ? src.company.name : null));
                customerMap.ForMember(desc => desc.bin, opt => opt.MapFrom(src => src.company != null ? src.company.bin : null));
                customerMap.ForMember(desc => desc.addressLegal, opt => opt.MapFrom(src => src.company != null ? src.company.addressactual : null));
                customerMap.ForMember(desc => desc.country, opt => opt.MapFrom(src => src.company != null && src.company.countries != null ? src.company.countries.name : null));
                customerMap.ForMember(desc => desc.telephone, opt => opt.MapFrom(src => src.company != null ? src.company.telephone.Trim() : null));
                customerMap.ForMember(desc => desc.email, opt => opt.MapFrom(src => src.company != null ? src.company.email.Trim() : null));
                #endregion

                #region map Contract
                var contractMap = config.CreateMap<ContractEF, Contract>();
                var contractMapReverse = contractMap.ReverseMap();
                #endregion

                #region map Product
                var productMap = config.CreateMap<ProductEF, Product>();
                productMap.ForMember(desc => desc.utin, opt => opt.MapFrom(src => src.unit != null ? src.unit.name : null));

                var productCompanyMap = config.CreateMap<ProductCompanyEF, ProductCompany>();
                productCompanyMap.ForMember(desc => desc.fileId, opt => opt.MapFrom(src => src.documentId));
                productCompanyMap.ForMember(desc => desc.utin, opt => opt.MapFrom(src => src.unit != null ? src.unit.name : null));
                #endregion

                #region map Order
                var orderMap = config.CreateMap<OrderEF, Order>();
                orderMap.ForMember(desc => desc.Initiator, opt => opt.MapFrom(src => src.customer != null && src.customer.company != null ? src.customer.company.name : null));
                orderMap.ForMember(desc => desc.Deadline, opt => opt.MapFrom(src => src.auction != null ? src.auction.regulation.applydeadline : new DateTime()));
                orderMap.ForMember(desc => desc.Title, opt => opt.MapFrom(src => src.auction != null ? String.Format("{0} {1}", src.auction.date.ToShortDateString(), src.auction.number) : ""));
                orderMap.ForMember(desc => desc.Status, opt => opt.MapFrom(src => src.status != null ? src.status.name : null));

                var orderMapReverse = orderMap.ReverseMap();
                orderMapReverse.ForMember(desc => desc.auction, opt => opt.Ignore());
                orderMapReverse.ForMember(desc => desc.status, opt => opt.Ignore());
                orderMapReverse.ForMember(desc => desc.filesList, opt => opt.Ignore());
                #endregion

                #region map Status
                var statusMap = config.CreateMap<StatusEF, Status>();
                #endregion

                #region map Company
                var companyMap = config.CreateMap<CompanyEF, Company>();
                companyMap.ForMember(desc => desc.country, opt => opt.MapFrom(src => src.countries != null ? src.countries.name : null));
                companyMap.ReverseMap();

                var companyWithProductMap = config.CreateMap<CompanyEF, CompanyWithProducts>();
                companyMap.ForMember(desc => desc.country, opt => opt.MapFrom(src => src.countries != null ? src.countries.name : null));
                #endregion

                #region map Site
                var siteMap = config.CreateMap<SiteEF, Site>();
                #endregion

                #region map FilesList
                var fileListMap = config.CreateMap<FilesListEF, FileList>();

                fileListMap.ForMember(desc => desc.files, opt => opt.MapFrom(src => src.documents));
                #endregion

                #region map Document
                var documentMap = config.CreateMap<DocumentRequisite, DocumentEF>();
                documentMap.ForMember(desc => desc.name, opt => opt.MapFrom(src => src.fileName.Substring(0, src.fileName.LastIndexOf("."))));
                documentMap.ForMember(desc => desc.extension, opt => opt.MapFrom(src => src.fileName.Substring(src.fileName.LastIndexOf(".") + 1)));
                documentMap.ForMember(desc => desc.documenttypeid, opt => opt.MapFrom(src => src.type));
                documentMap.ForMember(desc => desc.siteid, opt => opt.MapFrom(src => src.market));
                documentMap.ForMember(desc => desc.filesectionid, opt => opt.MapFrom(src => src.section));

                var documentMapReverse = documentMap.ReverseMap();
                documentMapReverse.ForMember(desc => desc.fileName, opt => opt.MapFrom(src => string.Format("{0}.{1}", src.name, src.extension)));
                documentMapReverse.ForMember(desc => desc.type, opt => opt.MapFrom(src => src.documenttypeid));
                documentMapReverse.ForMember(desc => desc.market, opt => opt.MapFrom(src => src.siteid));
                documentMapReverse.ForMember(desc => desc.section, opt => opt.MapFrom(src => src.filesectionid));
                #endregion

                #region map Applicant
                var applicantMap = config.CreateMap<ApplicantEF, Applicant>();
                applicantMap.ForMember(desc => desc.SupplierName, opt => opt.MapFrom(src => src.supplierorderid != null && src.supplierorder.supplierid != null ? src.supplierorder.supplier.company.name : ""));
                applicantMap.ForMember(desc => desc.SupplierId, opt => opt.MapFrom(src => src.supplierorder != null && src.supplierorder.supplier != null ? src.supplierorder.supplier.id : default(int)));
                #endregion

                #region map Procuratory
                var procuratoryMap = config.CreateMap<ProcuratoryEF, Procuratory>();
                procuratoryMap.ForMember(desc => desc.MinimalPrice, opt => opt.MapFrom(src => src.minimalprice));
                procuratoryMap.ForMember(desc => desc.SupplierId, opt => opt.MapFrom(src => src.supplierid));
                procuratoryMap.ForMember(desc => desc.SupplierName, opt => opt.MapFrom(src => src.supplier != null && src.supplier.company != null ? src.supplier.company.name : ""));
                var procuratoryReversMap = procuratoryMap.ReverseMap();
                #endregion

                #region map SupplierOrders
                var supplierOrdersMap = config.CreateMap<SupplierOrderEF, SupplierOrder>();
                supplierOrdersMap.ForMember(desc => desc.brokerid, opt => opt.MapFrom(src => src.contract.brokerid));
                supplierOrdersMap.ForMember(desc => desc.BrokerCode, opt => opt.MapFrom(src => src.contract.broker.brokersJournal.code));
                supplierOrdersMap.ForMember(desc => desc.BrokerName, opt => opt.MapFrom(src => src.contract.broker.shortname));
                supplierOrdersMap.ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.supplier.company.name));
                supplierOrdersMap.ForMember(desc => desc.auctionNumber, opt => opt.MapFrom(src => src.auction.number));
                supplierOrdersMap.ForMember(desc => desc.Address, opt => opt.MapFrom(src => src.supplier.company.addresslegal));
                supplierOrdersMap.ForMember(desc => desc.BIK, opt => opt.MapFrom(src => src.contract.bank.company.bik));
                supplierOrdersMap.ForMember(desc => desc.BIN, opt => opt.MapFrom(src => src.supplier.company.bin));
                supplierOrdersMap.ForMember(desc => desc.Phones, opt => opt.MapFrom(src => src.supplier.company.telephone));
                supplierOrdersMap.ForMember(desc => desc.IIK, opt => opt.MapFrom(src => src.supplier.company.iik));
                supplierOrdersMap.ForMember(desc => desc.BankName, opt => opt.MapFrom(src => src.contract.bank.name));

                var supplierOrderReverse = supplierOrdersMap.ReverseMap();
                supplierOrderReverse.ForMember(desc => desc.statusid, opt => opt.MapFrom(src => src.status.Id));
                supplierOrderReverse.ForMember(desc => desc.supplierid, opt => opt.MapFrom(src => src.SupplierId));
                supplierOrderReverse.ForMember(desc => desc.supplier, opt => opt.Ignore());
                supplierOrderReverse.ForMember(desc => desc.auction, opt => opt.Ignore());
                supplierOrderReverse.ForMember(desc => desc.contract, opt => opt.Ignore());
                supplierOrderReverse.ForMember(desc => desc.status, opt => opt.Ignore());
                supplierOrderReverse.ForMember(desc => desc.fileList, opt => opt.Ignore());
                supplierOrderReverse.ForMember(desc => desc.applicants, opt => opt.Ignore());
                #endregion

                #region map SupplierJournal
                var supplierJournalMap = config.CreateMap<SuppliersJournalEF, SupplierJournal>();
                var supplierJournalReverse = supplierJournalMap.ReverseMap();
                #endregion

                #region map Notifications
                var notificationsMap = config.CreateMap<NotificationEF, Notification>();
                notificationsMap.ForMember(desc => desc.eventDescription, opt => opt.MapFrom(src => src.event_.description));
                #endregion

                #region map FinalReport
                var finalReportMap = config.CreateMap<FinalReportEF, FinalReport>();
                var economyReoprtMap = config.CreateMap<EconomyViewEF, FinalReport>();
                #endregion

                #region map Qualifications
                var qualificationsMap = config.CreateMap<QualificationEF, Qualification>();
                qualificationsMap.ForMember(desc => desc.description, opt => opt.MapFrom(src => src.qualificationDictionary.description));
                qualificationsMap.ForMember(desc => desc.name, opt => opt.MapFrom(src => src.qualificationDictionary.name));
                #endregion

                #region map QualificationsDictionary
                var qualificationsDictionaryMap = config.CreateMap<QualificationDictionaryEF, QualificationDictionary>();
                #endregion

                #region map Person
                var personMap = config.CreateMap<PersonEF, Person>();
                var personReverseMap = personMap.ReverseMap();
                #endregion

                #region Views
                var analiticCountStatusMap = config.CreateMap<AnaliticCountStatusView, AnaliticCountStatus>();
                analiticCountStatusMap.ForMember(desc => desc.StatusId, opt => opt.MapFrom(src => src.Status));
                analiticCountStatusMap.ForMember(desc => desc.Status, opt => opt.MapFrom(src => src.StatusEf));
                #endregion
            });
        }
        #endregion

        #region Constructors
        //public EntityContext() : this("server=10.1.2.11;port=3306;database=brokerbase;uid=broker;password=KorPas$77&db;charset=utf8") { }
        //public EntityContext() : this("server=88.204.230.203;port=3306;database=brokerbase;uid=broker;password=KorPas$77&db;charset=utf8") { }
        public EntityContext() : this("server=88.204.230.204;port=50505;database=brokerbase;uid=Broker;password=v6emU!#TdKQ1ve;charset=utf8") { }
        //public EntityContext() : this("server=88.204.230.204;port=50505;database=brokerbaseSlava;uid=PaydaDB;password=LKg244)5uTX<P}G;charset=utf8") { } // Slava base

        public EntityContext(DbConnection conn) : base(conn, false)
        {
            InitializeAutoMapper();
        }

        public EntityContext(string connString) : base(connString)
        {
            InitializeAutoMapper();
        }
        #endregion

        #region ModelCreating
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder.HasDefaultSchema(this.Database.Connection.Database);
            modelBuilder.Entity<CompanyEF>()
                .HasMany(c => c.products)
                .WithMany(p => p.companies)
                .Map(mc => {
                    mc.ToTable("goodscompanies");
                    mc.MapLeftKey("companyid");
                    mc.MapRightKey("goodid");
                });


            var configurationBroker = modelBuilder.Entity<ConficurationsBroker>();
            configurationBroker.ToTable("configurations");
            configurationBroker.HasKey(c => c.id);

            var mailDeliveryException = modelBuilder.Entity<MailDeliveryException>();
            mailDeliveryException.ToTable("mail_delivery_exceptions");
            mailDeliveryException.HasKey(m => m.id);

            var techSpecReportBO = modelBuilder.Entity<TechSpecReportBO>();
            techSpecReportBO.ToTable("tech_spec_report_view2");
            techSpecReportBO.HasKey(t => t.id);

            var finalReportPlmtlBO = modelBuilder.Entity<FinalReportPlmtl>();
            finalReportPlmtlBO.ToTable("report_plmtl_view");
            finalReportPlmtlBO.HasKey(t => t.id);

            var supplierorders = modelBuilder.Entity<SupplierOrderEF>();
            supplierorders.ToTable("supplierorders");
            supplierorders.HasKey(so => so.id);
            supplierorders.Property(s => s.auctionid).IsRequired();
            supplierorders.HasRequired(so => so.supplier).WithMany().HasForeignKey(so => so.supplierid);
            supplierorders.HasOptional(so => so.status).WithMany().HasForeignKey(so => so.statusid);
            supplierorders.HasOptional(so => so.contract).WithMany().HasForeignKey(so => so.contractid);
            supplierorders.HasOptional(so => so.fileList).WithMany().HasForeignKey(so => so.fileListId);
            supplierorders.HasMany(c => c.lots)
                .WithMany()
                .Map(mc => {
                    mc.ToTable("supplier_order_lots");
                    mc.MapLeftKey("supplierorderid");
                    mc.MapRightKey("lotid");
                });

            var configurationEvent = modelBuilder.Entity<EventEF>();
            configurationEvent.ToTable("events");
            configurationEvent.HasKey(e => e.id);

            var configurationNotification = modelBuilder.Entity<NotificationEF>();
            configurationNotification.ToTable("notifications");
            configurationNotification.HasKey(n => n.id);
            configurationNotification.HasRequired(n => n.event_).WithMany().HasForeignKey(n => n.eventId);
            configurationNotification.HasRequired(n => n.auction).WithMany().HasForeignKey(n => n.auctionId);
            configurationNotification.HasRequired(n => n.supplier).WithMany().HasForeignKey(n => n.supplierId);

            var configurationQualificationDictionary = modelBuilder.Entity<QualificationDictionaryEF>();
            configurationQualificationDictionary.ToTable("qualification_dictionary");
            configurationQualificationDictionary.HasKey(qd => qd.id);

            var configurationQualification = modelBuilder.Entity<QualificationEF>();
            configurationQualification.ToTable("qualifications");
            configurationQualification.HasKey(q => q.id);
            configurationQualification.HasRequired(q => q.auction).WithMany().HasForeignKey(q => q.auctionId);
            configurationQualification.HasRequired(q => q.qualificationDictionary).WithMany().HasForeignKey(q => q.qualification_dictionary_id);

            //var configurationFinalReport = modelBuilder.Entity<FinalReportEF>();
            //configurationFinalReport.ToTable("finalreport");
            //configurationFinalReport.HasKey(f => f.id);
            //configurationFinalReport.HasRequired(f => f.auction).WithMany().HasForeignKey(f => f.auctionId);
            //configurationFinalReport.HasRequired(f => f.broker).WithMany().HasForeignKey(f => f.brokerId);
            //configurationFinalReport.HasRequired(f => f.lot).WithMany().HasForeignKey(f => f.lotId);
            //configurationFinalReport.HasRequired(f => f.supplier).WithMany().HasForeignKey(f => f.supplierId);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        #endregion

        #endregion

        #region Implement interface IDataManager

        #region Auction
        public Auction GetAuction(int id)
        {
            var auction = auctions.Find(id);
            return Mapper.Map<Auction>(auction);
        }

        private IQueryable<AuctionEF> GetAuctionQuery(string numberOrProduct = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? site = default(int?), int? supplierId = default(int?), int? statusId = default(int?), int? winner = default(int), int? brokerId = default(int?), int? traderId = default(int?))
        {
            var query = auctions.Include(a => a.broker);

            if (!string.IsNullOrEmpty(numberOrProduct)) {
                query = query.Where(a => (a.number.Contains(numberOrProduct) || lots.Any(l => l.auctionid == a.id && l.description.Contains(numberOrProduct))));
            }

            if (customerId != default(int?)) {
                query = query.Where(a => a.customerid == customerId);
            }

            if (fromDate != default(DateTime?) && toDate != default(DateTime?))
                query = query.Where(a => fromDate <= a.date && toDate >= a.date);

            if (site > 0)
                query = query.Where(a => site == a.siteid);

            if (statusId != default(int?)) {
                query = query.Where(a => a.statusid == statusId);
            }

            if (winner != default(int))
                query = query.Where(a => winner == 1 ? (a.statusid != 2 && a.statusid != 3) : (a.statusid == 2 || a.statusid == 3));

            if (supplierId != default(int?))
                query = query.Where(a => applicants.Any(ap => ap.supplierorder != null ? ap.auctionid == a.id && ap.supplierorder.supplierid == supplierId : false));

            if (brokerId != default(int?)) {
                query = query.Where(a => a.brokerid == brokerId);
            }

            if (traderId != default(int?)) {
                query = query.Where(a => a.traderid == traderId);
            }

            return query.AsNoTracking();
        }

        public List<Auction> GetAuctions(int page, int countItems, string numberOrProduct = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? site = default(int?), int? supplierId = default(int?), int? statusId = default(int?), int? winner = default(int), string orderBy = null, bool isdesc = false, int? brokerId = default(int?), int? traderId = default(int?))
        {
            var skip = (page - 1) * countItems;
            var resAuctions = GetAuctionQuery(numberOrProduct, customerId, fromDate, toDate, site, supplierId, statusId, winner, brokerId, traderId);

            switch (orderBy) {
                case ("number"): resAuctions = isdesc ? resAuctions.OrderByDescending(a => a.number) : resAuctions.OrderBy(a => a.number); break;
                case ("status"): resAuctions = isdesc ? resAuctions.OrderByDescending(a => a.statusid) : resAuctions.OrderBy(a => a.statusid); break;
                case ("sum"): resAuctions = isdesc ? resAuctions.OrderByDescending(a => a.lots.Sum(l => l.sum)) : resAuctions.OrderBy(a => a.lots.Sum(l => l.sum)); break;
                case ("applicantsDeadline"): resAuctions = isdesc ? resAuctions.OrderByDescending(a => a.regulation.applicantsdeadline) : resAuctions.OrderBy(a => a.regulation.applicantsdeadline); break;
                case ("date"): default: resAuctions = isdesc ? resAuctions.OrderByDescending(a => a.regulation.applicantsdeadline) : resAuctions.OrderBy(a => a.regulation.applicantsdeadline); break;
            }

            resAuctions = resAuctions.Skip(() => skip).Take(() => countItems).AsNoTracking();
            return Mapper.Map<List<Auction>>(resAuctions.ToList());
        }

        public int GetAuctionsCount(string numberOrProduct = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? site = default(int?), int? supplierId = default(int?), int? statusId = default(int?), int? winner = default(int), int? brokerId = default(int?), int? traderId = default(int?))
        {
            var auctionsCount = GetAuctionQuery(numberOrProduct, customerId, fromDate, toDate, site, supplierId, statusId, winner, brokerId, traderId).Count();
            return auctionsCount;
        }


        public List<Auction> ReadAuctions(DateTime fromDate, DateTime toDate, int statusId)
        {
            return Database.SqlQuery<Auction>("select * from auctionsFullView where date>=@fromDate and date<=@toDate and statusid=@statusId",
                new MySqlParameter("fromDate", fromDate),
                new MySqlParameter("toDate", toDate),
                new MySqlParameter("statusId", statusId)).ToList();
        }


        public List<AltaBO.AuctionResult> GetAuctionsResult()
        {
            return Mapper.Map<List<AltaBO.AuctionResult>>(auctionsresultview.AsNoTracking().ToList());
        }

        public bool AddApplicat(int auctionId, int supplierId)
        {

            if (!auctions.Any(a => a.id == auctionId)) {
                return false;
            }

            if (applicants.Any(a => a.auctionid == auctionId && a.supplierorder.supplierid == supplierId)) {
                return false;
            }
            return false;
        }


        public int CreateAuction(Auction auction)
        {
            return Database.SqlQuery<int>("insert into auctions(date, sectionid, typeid, number, statusid, ndsincluded, ownerid, siteid, regulationid, traderid, customerid, brokerid, signstatusid, fileslistid)" +
                "values(@date, @sectionId, @typeId, @number, @statusId, @ndsIncluded, @ownerId, @siteId, @regulationId, @traderId, @customerId, @brokerId, @signStatusId, @filesListId);select last_insert_id()",
                new MySqlParameter("date", auction.Date),
                new MySqlParameter("sectionId", auction.sectionId),
                new MySqlParameter("typeId", auction.typeId),
                new MySqlParameter("number", auction.Number),
                new MySqlParameter("statusId", auction.StatusId),
                new MySqlParameter("ndsIncluded", auction.ndsIncluded ? 1 : 0),
                new MySqlParameter("siteId", auction.SiteId),
                new MySqlParameter("ownerId", auction.OwnerId),
                new MySqlParameter("regulationId", auction.RegulationId),
                new MySqlParameter("traderId", auction.TraderId),
                new MySqlParameter("customerId", auction.CustomerId),
                new MySqlParameter("brokerId", auction.BrokerId),
                new MySqlParameter("signStatusId", auction.signStatusId),
                new MySqlParameter("filesListId", auction.FilesListId)).First();
        }


        public void UpdateAuction(Auction auction)
        {
            Database.SqlQuery<int>("update auctions set date=@date, sectionid=@sectionId, typeid=@typeId, number=@number, statusid=@statusId, ndsincluded=@ndsIncluded," +
                " siteid=@siteId, regulationid=@regulationId, traderid=@traderId, customerid=@customerId, brokerid=@brokerId where id=@id;select last_insert_id()",
                new MySqlParameter("date", auction.Date),
                new MySqlParameter("sectionId", auction.sectionId),
                new MySqlParameter("typeId", auction.typeId),
                new MySqlParameter("number", auction.Number),
                new MySqlParameter("statusId", auction.StatusId),
                new MySqlParameter("ndsIncluded", auction.ndsIncluded ? 1 : 0),
                new MySqlParameter("siteId", auction.SiteId),
                new MySqlParameter("regulationId", auction.RegulationId),
                new MySqlParameter("traderId", auction.TraderId),
                new MySqlParameter("customerId", auction.CustomerId),
                new MySqlParameter("brokerId", auction.BrokerId),
                new MySqlParameter("id", auction.Id)).First();
        }


        public List<SupplierOrder> GetSuplliersOrders(int auctionId)
        {
            var supplierOrdersQuery = supplierorders.Where(s => s.auctionid == auctionId);
            return Mapper.Map<List<SupplierOrder>>(supplierOrdersQuery.ToList());
        }

        /// <summary>
        /// Добавление заявки на участи в аукцион
        /// </summary>
        /// <param name="auctionId">Идентификатор аукциона</param>
        /// <param name="supplierId">Идентификатор участника</param>
        /// <returns></returns>
        public bool AddSupplier(int auctionId, int supplierId)
        {
            if (supplierorders.Any(s => s.auctionid == auctionId && s.supplierid == supplierId)) {
                return false;
            }
            var auction = auctions.Find(auctionId);
            var supplier = suppliers.Find(supplierId);
            if (auction == null || supplier == null)
                return false;

            var supplierOrder = new SupplierOrderEF() {
                auctionid = auction.id,
                supplierid = supplier.id,
                statusid = 1,
                date = DateTime.Today
            };

            supplierorders.Add(supplierOrder);
            SaveChanges();
            return true;
        }

        public bool RejectSupplier(int auctionId, int supplierId)
        {
            var ordersOrder = supplierorders.FirstOrDefault(o => o.auctionid == auctionId && o.supplierid == supplierId);
            if (ordersOrder == null)
                return false;
            supplierorders.Remove(ordersOrder);
            SaveChanges();
            return true;
        }

        public bool AddSupllierOrder(int auctionId, SupplierOrder supplierOrder)
        {
            var order = Mapper.Map<SupplierOrderEF>(supplierOrder);

            if (supplierorders.Any(so => so.auctionid == auctionId && so.supplierid == order.supplierid && order.contractid == so.contractid)) {
                return false;
            }

            var lotsOrder = new List<LotEF>();
            foreach (var lot in order.lots) {
                lotsOrder.Add(lots.Find(lot.id));
            }

            order.lots = lotsOrder;
            order.auctionid = auctionId;
            order.date = DateTime.Now;
            supplierorders.Add(order);
            SaveChanges();
            return true;
        }
        #endregion

        #region Lot
        public List<Lot> GetLots(int? auctionid = default(int?))
        {
            var lots = this.lots.Include(l => l.unit).Where(l => (
                (auctionid != null ? l.auctionid == auctionid : true)
            )).AsNoTracking();
            return Mapper.Map<List<Lot>>(lots.ToList());
        }

        public Lot GetLot(int lotId)
        {
            var lot = lots.Find(lotId);
            return Mapper.Map<Lot>(lot);
        }

        public List<LotsExtended> GetLotsExtended(int lotId)
        {
            var lotsextended = this.lotsextended.Where(l => l.lotid == lotId).AsNoTracking();

            return Mapper.Map<List<LotsExtended>>(lotsextended.ToList());
        }

        public int UpdateLotsExtended(LotsExtended lotsExtended)
        {
            try {
                var lotEx = lotsextended.Find(lotsExtended.id);
                lotEx.endprice = lotsExtended.endprice;
                lotEx.endsum = lotsExtended.endsum;

                SaveChanges();
            } catch (Exception) { return 0; }

            return 1;
        }


        public List<Lot> ReadLots(int auctionId)
        {
            return Database.SqlQuery<Lot>("select * from lotsListView where auctionid=@auctionId",
                new MySqlParameter("auctionId", auctionId)).ToList();
        }


        public Lot ReadLot(int lotId)
        {
            return Database.SqlQuery<Lot>("select * from lotsListView where id=@lotId",
                new MySqlParameter("lotId", lotId)).FirstOrDefault();
        }


        public int CreateLot(Lot lot)
        {
            return Database.SqlQuery<int>("insert into lots(auctionId, number, description, unitid, amount, price, sum, paymentterm, deliverytime, deliveryplace, dks, contractnumber, step, warranty, localcontent)" +
                "values(@auctionId, @number, @description, @unitId, @quantity, @price, @sum, @paymentTerm, @deliveryTime, @deliveryPlace, @dks, @contractNumber, @step, @warranty, @localContent);" +
                "select last_insert_id()",
                new MySqlParameter("auctionId", lot.auctionId != null ? lot.auctionId : 0),
                new MySqlParameter("number", lot.Number != null ? lot.Number : ""),
                new MySqlParameter("description", lot.Name != null ? lot.Name : ""),
                new MySqlParameter("unitId", lot.UnitId != null ? lot.UnitId : 15),
                new MySqlParameter("quantity", lot.Quantity != null ? lot.Quantity : 1),
                new MySqlParameter("price", lot.Price != null ? lot.Price : 1),
                new MySqlParameter("sum", lot.Sum != null ? lot.Sum : 1),
                new MySqlParameter("paymentTerm", lot.PaymentTerm != null ? lot.PaymentTerm : ""),
                new MySqlParameter("deliveryTime", lot.DeliveryTime != null ? lot.DeliveryTime : ""),
                new MySqlParameter("deliveryPlace", lot.DeliveryPlace != null ? lot.DeliveryPlace : ""),
                new MySqlParameter("dks", lot.Dks != null ? lot.Dks : 0),
                new MySqlParameter("contractNumber", lot.ContractNumber != null ? lot.ContractNumber : ""),
                new MySqlParameter("step", lot.Step != null ? lot.Step : 1),
                new MySqlParameter("warranty", lot.Warranty != null ? lot.Warranty : 0),
                new MySqlParameter("localContent", lot.LocalContent != null ? lot.LocalContent : 0)).FirstOrDefault();
        }


        public void UpdateLot(Lot lot)
        {
            Database.SqlQuery<int>("update lots set number=@number, description=@description, unitid=@unitId, amount=@quantity, price=@price, sum=@sum, paymentterm=@paymentTerm," +
                " deliverytime=@deliveryTime, deliveryplace=@deliveryPlace, dks=@dks, contractnumber=@contractNumber, step=@step, warranty=@warranty, localcontent=@localContent " +
                "where id=@lotId;" +
                "select last_insert_id()",
                new MySqlParameter("lotId", lot.Id),
                new MySqlParameter("number", lot.Number),
                new MySqlParameter("description", lot.Name),
                new MySqlParameter("unitId", lot.UnitId),
                new MySqlParameter("quantity", lot.Quantity),
                new MySqlParameter("price", lot.Price),
                new MySqlParameter("sum", lot.Sum),
                new MySqlParameter("paymentTerm", lot.PaymentTerm),
                new MySqlParameter("deliveryTime", lot.DeliveryTime),
                new MySqlParameter("deliveryPlace", lot.DeliveryPlace),
                new MySqlParameter("dks", lot.Dks),
                new MySqlParameter("contractNumber", lot.ContractNumber),
                new MySqlParameter("step", lot.Step),
                new MySqlParameter("warranty", lot.Warranty),
                new MySqlParameter("localContent", lot.LocalContent)).FirstOrDefault();
        }


        public void DeleteLot(int lotId)
        {
            Database.SqlQuery<int>("delete from lots where id=@lotId;select last_insert_id()",
                new MySqlParameter("lotId", lotId)).FirstOrDefault();
        }
        #endregion

        #region LotExtended
        public List<LotsExtended> ReadLotExtended(int lotId)
        {
            return Database.SqlQuery<LotsExtended>("select * from lotsextended where lotid=@lotId",
                new MySqlParameter("lotId", lotId)).ToList();
        }
        #endregion

        #region Supplier
        private IQueryable<SupplierEF> SelectSupplierByProducts(string searchProduct = null)
        {
            IQueryable<SupplierEF> query;
            if (!string.IsNullOrEmpty(searchProduct)) {
                query = (from pc in productsCompany
                         from s in suppliers.Where(supplier => supplier.companyid == pc.companyId).DefaultIfEmpty()
                         where pc.name.Contains(searchProduct) && pc.companyId > 0
                         select s).Distinct();
            } else {
                query = suppliers.AsQueryable();
            }
            return query.AsNoTracking();
        }

        public List<SupplierWithProduct> GetSuppliersWithProduct(int page, int countItems, out int count, string searchProduct = null)
        {
            var suppliers = SelectSupplierByProducts(searchProduct);
            count = suppliers.Count();
            suppliers = suppliers.OrderBy(s => s.company.name).Skip(countItems * (page - 1)).Take(countItems);
            return Mapper.Map<List<SupplierWithProduct>>(suppliers.ToList());
        }

        public List<Supplier> GetSuppliersByParam(int page, int countItems, string textSearch = null, int method = 1)
        {

            IQueryable<SupplierEF> suppliers;
            switch (method) {
                case 1:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.name.Contains(textSearch) : true select s;
                    suppliers = suppliers.AsNoTracking().OrderBy(s => s.company.name).Skip(countItems * (page - 1)).Take(countItems);
                    return Mapper.Map<List<Supplier>>(suppliers.ToList());
                case 2:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.bin.Contains(textSearch) : true select s;
                    suppliers = suppliers.AsNoTracking().OrderBy(s => s.company.bin).Skip(countItems * (page - 1)).Take(countItems);
                    return Mapper.Map<List<Supplier>>(suppliers.ToList());
                case 3:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.addressactual.Contains(textSearch) : true select s;
                    suppliers = suppliers.AsNoTracking().OrderBy(s => s.company.addressactual).Skip(countItems * (page - 1)).Take(countItems);
                    return Mapper.Map<List<Supplier>>(suppliers.ToList());
                default:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.name.Contains(textSearch) : true select s;
                    suppliers = suppliers.AsNoTracking().OrderBy(s => s.company.name).Skip(countItems * (page - 1)).Take(countItems);
                    return Mapper.Map<List<Supplier>>(suppliers.ToList());
            }
        }

        public int GetSuppliersCount(string textSearch = null, int method = 1)
        {
            IQueryable<SupplierEF> suppliers;
            switch (method) {
                case 1:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.name.Contains(textSearch) : true select s;
                    return suppliers.Count();
                case 2:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.bin.Contains(textSearch) : true select s;
                    return suppliers.Count();
                case 3:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.addressactual.Contains(textSearch) : true select s;
                    return suppliers.Count();
                default:
                    suppliers = from s in this.suppliers where textSearch != null ? s.company.name.Contains(textSearch) : true select s;
                    return suppliers.Count();
            }
        }



        public List<Supplier> GetSuppliers(int page, int countItems, string textSearch = null)
        {
            var suppliers = from s in this.suppliers where textSearch != null ? (s.company.name.Contains(textSearch) || s.company.bin.Contains(textSearch)) : true select s;
            suppliers = suppliers.AsNoTracking().OrderBy(s => s.company.name).Skip(countItems * (page - 1)).Take(countItems);
            return Mapper.Map<List<Supplier>>(suppliers.ToList());
        }

        public int GetSuppliersCount(string textSearch = null)
        {
            var suppliers = from s in this.suppliers where textSearch != null ? (s.company.name.Contains(textSearch) || s.company.bin.Contains(textSearch)) : true select s;
            return suppliers.Count();
        }

        public int GetSupplierId(int companyId)
        {
            var supplier = suppliers.FirstOrDefault(c => c.companyid == companyId);
            return supplier != null ? supplier.id : 0;
        }

        public Supplier GetSupplierByUserId(int userId)
        {
            SupplierEF supplier = null;
            var user = users.Find(userId);
            if (user != null && user.person != null) {
                var employee = employees.FirstOrDefault(e => e.personid == user.personid);
                if (employee != null) {
                    supplier = suppliers.FirstOrDefault(s => s.companyid == employee.companyid);
                }
            }
            return Mapper.Map<Supplier>(supplier);
        }

        public Supplier GetSupplier(int supplierId)
        {
            var supplier = suppliers.Find(supplierId);
            return Mapper.Map<Supplier>(supplier);
        }

        public Supplier GetSupplierByCompanyId(int companyId)
        {
            var supplier = suppliers.FirstOrDefault(s => s.companyid == companyId);
            return Mapper.Map<Supplier>(supplier);
        }


        public List<Supplier> ReadSuppliers()
        {
            return Database.SqlQuery<Supplier>("select * from suppliersListView", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region SupplierOrder
        public List<SupplierOrderView> ReadSupplierOrders(int auctionId)
        {
            return Database.SqlQuery<SupplierOrderView>("select * from supplierOrdersListView where auctionid=@auctionId",
                new MySqlParameter("auctionId", auctionId)).ToList();
        }


        public SupplierOrderView ReadSupplierOrder(int supplierOrderId)
        {
            return Database.SqlQuery<SupplierOrderView>("select * from supplierOrdersListView where id=@supplierOrderId",
                new MySqlParameter("supplierOrderId", supplierOrderId)).FirstOrDefault();
        }
        #endregion

        #region Customer
        private IQueryable<CustomerEF> SelectCustomers(string search = null)
        {
            var query = customers.AsNoTracking().AsQueryable();
            if (search != null) {
                query = query.Where(q => q.company.name.Contains(search));
            }
            return query;
        }

        public Customer GetCustomer(int customerId)
        {
            var customer = customers.Find(customerId);
            return Mapper.Map<Customer>(customer);
        }

        public int GetCustomersCount(string search = null)
        {
            return SelectCustomers(search).Count();
        }

        public List<Customer> GetCustomers(int skip, int countItems, string search = null)
        {
            var query = SelectCustomers(search);
            var rows = query.OrderBy(c => c.company.name).Skip(() => skip).Take(() => countItems);
            return Mapper.Map<List<Customer>>(rows.ToList());
        }

        public int GetCustomerId(int companyId)
        {
            var customer = customers.FirstOrDefault(c => c.companiesid == companyId);
            return customer != null ? customer.id : 0;
        }


        public List<Company> GetCustomerEnum()
        {
            var customersEnum = customers
                .Select(x => new Company {
                    name = x.company.name,
                    id = x.id,
                }).ToList();
            return customersEnum;
        }


        public List<Customer> ReadCustomers()
        {
            return Database.SqlQuery<Customer>("select cus.*, com.name from customers as cus left join companies as com on com.id=cus.companiesid", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region Product
        //TODO remove
        public List<Product> GetAllProducts(int? companyId = default(int?))
        {
            //var prods = products.Where(p => companyId != default(int?) ? p.companies.Any(c => c.id == companyId) : true).AsNoTracking();
            var company = companies.Find(companyId);

            return company != null ? Mapper.Map<List<Product>>(company.products.ToList()) : new List<Product>();
        }

        // get product by id
        public Product GetProduct(int id)
        {
            return Mapper.Map<Product>(products.Find(id));
        }

        // find products by parameters: suplierid, text, companyid
        public List<Product> GetProducts(int page, int countItems, string textSearch = null, int? companyId = default(int?), int? supplierId = default(int?))
        {
            if (companyId == default(int?) && supplierId != default(int?)) {
                var supplier = suppliers.FirstOrDefault(s => s.id == supplierId);
                if (supplier != null) {
                    companyId = supplier.companyid;
                } else { return null; }
            }

            var products = this.products.Where(p =>
                (companyId != default(int?) ? p.companies.Any(c => c.id == companyId) : true) &&
                (!string.IsNullOrEmpty(textSearch) ? p.name.Contains(textSearch) : true)
            )
            .AsNoTracking()
            .OrderBy(s => s.name)
            .Skip(countItems * (page - 1))
            .Take(countItems);

            return Mapper.Map<List<Product>>(products.ToList());
        }

        public int GetProductsCount(string textSearch = null, int? companyId = default(int?), int? supplierId = default(int?))
        {
            if (companyId == default(int?) && supplierId != default(int?)) {
                var supplier = suppliers.FirstOrDefault(s => s.id == supplierId);
                if (supplier != null) {
                    companyId = supplier.companyid;
                } else { return 0; }
            }

            var products = this.products.Where(p =>
                (companyId != default(int?) ? p.companies.Any(c => c.id == companyId) : true) &&
                (!string.IsNullOrEmpty(textSearch) ? p.name.Contains(textSearch) : true)
            )
            .AsNoTracking();

            return products.Count();
        }

        public bool AddProductFromCompany(string name, int companyId, int? documentid = default(int?), string description = null)
        {
            var product = products.FirstOrDefault(p => p.name.Equals(name));
            if (product == null) {
                product = products.Add(new ProductEF() { name = name });
                SaveChanges();
            }
            if (product == null) {
                return false;
            }

            if (!companies.Any(c => c.id == companyId) || !products.Any(p => p.id == product.id))
                return false;

            var company = companies.Find(companyId);
            if (company == null) return false;

            company.products.Add(product);
            SaveChanges();

            Database.ExecuteSqlCommand(string.Format("UPDATE goodscompanies SET documentid = {0}, description ={1} WHERE goodid={2} and companyid ={3};", documentid != null ? documentid : 0, description.Replace("'", "\'"), product.id, company.id));
            SaveChanges();

            return true;

        }

        public List<Product> GetProducts()
        {
            return Mapper.Map<List<Product>>(products.ToList());
        }

        public List<ProductCompany> GetProductsCompany(int companyId)
        {
            var prodsCopmanies = productsCompany.AsNoTracking().Where(p => p.companyId == companyId);
            return Mapper.Map<List<ProductCompany>>(prodsCopmanies.ToList());
        }

        public List<ProductCompanyEF> GetCompaniesWithProduct(string queryTxt)
        {
            return productsCompany.Where(p => p.name.ToLower().Contains(queryTxt.ToLower())).ToList();
        }

        public bool RemoveProductCopmany(int productId, int companyId)
        {
            if (productsCompany.Any(p => p.companyId == companyId && p.productId == productId)) {
                var company = companies.Find(companyId);
                var product = company.products.First(p => p.id == productId);
                company.products.Remove(product);
                SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Order
        private IQueryable<OrderEF> SelectOrders(int? auctionId = default(int?), string searchText = null, DateTime? fromDate = null, DateTime? toDate = null, int? customerId = default(int?), int statusId = default(int))
        {
            var query = orders.AsQueryable();
            if (auctionId != default(int?))
                query = query.Where(o => o.auctionid == auctionId);
            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(o => o.number.Contains(searchText));
            if (fromDate != null)
                query = query.Where(o => fromDate <= o.date);
            if (toDate != null)
                query = query.Where(o => toDate >= o.date);
            if (customerId != default(int?))
                query = query.Where(o => o.customerid == customerId);
            if (statusId != default(int))
                query = query.Where(o => o.statusid == statusId);

            return query.AsNoTracking();
        }


        public List<Order> GetOrders(int? auctionId = default(int?), string searchText = null, DateTime? fromDate = null, DateTime? toDate = null, int? customerId = default(int?), int statusId = default(int))
        {
            var query = SelectOrders(auctionId, searchText, fromDate, toDate, customerId, statusId);

            return Mapper.Map<List<Order>>(query.AsNoTracking().ToList());
        }

        public int GetOrdersCount(int? auctionId = default(int), string searchText = null, DateTime? fromDate = null, DateTime? toDate = null, int? customerId = default(int?), int statusId = default(int))
        {
            var query = SelectOrders(auctionId, searchText, fromDate, toDate, customerId, statusId);
            return query.Count();
        }

        public Order GetOrder(int id)
        {
            var order = orders.Find(id);
            return Mapper.Map<Order>(order);
        }

        public int CreateOrder(Order order, int initiatorId, int? filesListId = default(int))
        {
            var orderEF = Mapper.Map<OrderEF>(order);

            if (filesListId != default(int)) orderEF.fileslistid = filesListId;
            orderEF.initiatorid = initiatorId;

            orders.Add(orderEF);
            SaveChanges();

            return orderEF.id;
        }


        public List<Order> ReadOrders(int statusId = 1)
        {
            return Database.SqlQuery<Order>("select * from ordersView where statusid=@statusId",
                new MySqlParameter("statusId", statusId)).ToList();
        }


        public Order ReadOrder(int auctionId)
        {
            return Database.SqlQuery<Order>("select * from ordersView where auctionid=@auctionId",
                new MySqlParameter("auctionId", auctionId)).FirstOrDefault();
        }


        public int CreateOrder(Order order)
        {
            return Database.SqlQuery<int>("insert into order (initiatorid, auctionid, statusid, number, siteid, date, customerid, fileslistid)" +
                "values(@initiatorId, @auctionId, @statusId, @number, @stieId, @date, @customerId, @filesListId);select last_insert_id()",
                new MySqlParameter("initiatorId", order.customerid),
                new MySqlParameter("auctionId", order.auctionId),
                new MySqlParameter("statusId", order.statusId),
                new MySqlParameter("number", order.Number),
                new MySqlParameter("siteId", order.siteId),
                new MySqlParameter("date", order.Date),
                new MySqlParameter("customerId", order.customerid),
                new MySqlParameter("filesListId", order.filesListId != null ? order.filesListId : 0)).FirstOrDefault();
        }
        #endregion

        #region QualificationDictionary
        public List<QualificationDictionary> GetQualificationDictionary()
        {
            var qualificationDictionary = qualificationdictionary.ToList();

            return Mapper.Map<List<QualificationDictionary>>(qualificationDictionary);
        }
        #endregion

        #region Qualifications
        public List<Qualification> GetQualifications(int auctionId = default(int))
        {


            var qualificationInfo = qualifications.AsQueryable();

            if (auctionId != default(int)) {
                qualificationInfo = qualificationInfo.Where(q => q.auctionId == auctionId);
            }

            return Mapper.Map<List<Qualification>>(qualificationInfo.ToList());
        }

        public int CreateQualification(Qualification qualification)
        {
            var qualificationEF = Mapper.Map<QualificationEF>(qualification);

            qualifications.Add(qualificationEF);
            SaveChanges();

            return qualificationEF.id;
        }
        #endregion

        #region Status
        public List<Status> GetStatuses()
        {
            return Mapper.Map<List<Status>>(statuses.AsNoTracking().ToList());
        }


        public List<Status> ReadStatuses()
        {
            return Database.SqlQuery<Status>("select * from statuses", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region Company
        /// <summary>
        /// Обновление данных компании
        /// </summary>
        /// <param name="company">Бизнес объект компании</param>
        /// <returns>В случае успешной операции возвращает true, иначе false</returns>
        public bool UpdateCompany(Company company)
        {
            try {
                var companyEf = Mapper.Map<CompanyEF>(company);
                if (companies.Any(c => c.id == companyEf.id)) {
                    companies.Attach(companyEf);
                    Entry(companyEf).State = EntityState.Modified;
                    SaveChanges();
                    return true;
                } else return false;
            } catch (Exception) {
                return false;
            }
        }


        /*public List<Company> ReadCompanies() {
            return Mapper.Map<List<Company>>(companies).ToList();
        }*/


        public bool AddFileListId(int companyId, int fileListId)
        {
            var company = companies.Find(companyId);
            if (company != null) {
                company.filesListId = fileListId;
                companies.Attach(company);
                Entry(company).State = EntityState.Modified;
                SaveChanges();
                return true;
            }
            return false;
        }

        public List<Company> GetCompanies(string searchText = null, string bin = null)
        {
            var comps = companies.AsQueryable();

            if (!string.IsNullOrEmpty(searchText)) {
                comps = comps.Where(c => c.name.Contains(searchText));
            }

            if (!string.IsNullOrEmpty(bin)) {
                comps = comps.Where(c => c.bin.Equals(bin));
            }

            return comps.AsNoTracking().ProjectTo<Company>().ToList();
        }

        public Company GetCompany(int id)
        {
            var company = companies.Find(id);
            return Mapper.Map<Company>(company);
        }

        public Company GetCompanySupplier(int supplierId)
        {
            var supplier = suppliers.Find(supplierId);
            return supplier != null ? Mapper.Map<Company>(supplier.company) : null;
        }

        public Company GetCompanyByUserId(int userId)
        {
            var user = users.FirstOrDefault(u => u.id == userId);
            if (user != null) {
                var employee = employees.FirstOrDefault(e => e.personid == user.personid);
                if (employee != null) {
                    return Mapper.Map<Company>(employee.company);
                }
            }
            return null;
        }

        private IQueryable<CompanyEF> SelectCompanyWithProduct(string productName = null)
        {
            var query = products.AsQueryable();
            if (!string.IsNullOrEmpty(productName)) {
                query = query.Where(p => p.name.Contains(productName));
            }
            return query.AsNoTracking().SelectMany(p => p.companies).Distinct();
        }

        public List<CompanyWithProducts> GetCompanyWithProduct(int page, int countItems, string productName = null)
        {
            var comps = SelectCompanyWithProduct(productName);
            comps.OrderBy(s => s.name).Skip(countItems * (page - 1)).Take(countItems);

            return Mapper.Map<List<CompanyWithProducts>>(comps.ToList());
        }

        public int GetCompanyWithProductCount(string productName = null)
        {
            var companies = SelectCompanyWithProduct(productName);
            return companies.Count();
        }

        public bool AddEmployee(int companyId, int personId, string position)
        {
            if (companies.Any(c => c.id == companyId) && persons.Any(p => p.id == personId) && !employees.Any(e => e.companyid == companyId && e.personid == personId) && !string.IsNullOrEmpty(position)) {
                employees.Add(new EmployeeEF() { companyid = companyId, personid = personId, position = position });
                SaveChanges();
                return true;
            }
            return false;
        }


        public List<CompaniesWithContractView> GetSuppliersWithContract()
        {
            return Database.SqlQuery<CompaniesWithContractView>("call companiesWithContract()", new MySqlParameter("", null)).ToList();
        }

        #endregion

        #region User
        public int CreateUser(int personId, string login, string pass, int roleId)
        {
            if (personId > 0 && !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(pass) && roleId > 0) {
                var user = new UserEF() {
                    login = login,
                    pass = pass,
                    personid = personId,
                    roleid = roleId,
                    isactive = true
                };
                users.Add(user);
                SaveChanges();
                return user.id;
            }
            return 0;
        }

        public User GetUser(int userId)
        {
            var user = users.Find(userId);

            var userBO = Mapper.Map<User>(user);
            var company = employees.Where(e => e.personid == user.personid).Select(e => e.company).FirstOrDefault();
            if (company != null) {
                userBO.CompanyId = company.id;
                userBO.SupplierId = GetSupplierId(company.id);
                userBO.CustomerId = GetCustomerId(company.id);
                userBO.BrokerId = GetBrokerId(company.id);
            }
            return userBO;
        }

        public User GetUser(string login, string password)
        {
            var user = users.AsNoTracking().FirstOrDefault(u => (u.login.Equals(login) && u.pass.Equals(password)));
            if (user != null) {
                return GetUser(user.id);
            }
            return null;
        }

        public User GetUser(string login)
        {
            var user = users.AsNoTracking().FirstOrDefault(u => u.login.Equals(login) && u.isactive);
            if (user != null) {
                return GetUser(user.id);
            }
            return null;
        }

        public string GetAccessStringByUser(int userid)
        {
            var user = users.Find(userid);
            if (user != null) {
                return user.role != null ? user.role.accessString : null;
            }
            return null;
        }

        public List<User> GetUsersByCompany(int companyId)
        {
            var usersCompany = employees.Where(e => e.companyid == companyId).SelectMany(e => users.Where(u => u.personid == e.personid));

            return Mapper.Map<List<User>>(usersCompany.ToList());
        }

        public String GetPasswordUser(int userId)
        {
            string password = null;
            var user = users.Find(userId);
            if (user != null) {
                password = user.pass;
            }
            return password;
        }
        #endregion

        #region Site
        public List<Site> GetCatalogSites()
        {
            var sites = this.sites.ToList();
            return Mapper.Map<List<Site>>(sites);
        }


        public List<Site> ReadSites()
        {
            return Database.SqlQuery<Site>("select * from sites", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region Archive
        public int PutDocument(DocumentRequisite documentRequisite, int? filesListId = default(int?))
        {
            var document = Mapper.Map<DocumentEF>(documentRequisite);
            if (filesListId != default(int?)) {
                document.fileslistid = filesListId;
            }
            documents.Add(document);
            SaveChanges();

            return document.id;
        }

        public int PutSimpleDocument(DocumentRequisite documentRequisite)
        {
            var document = Mapper.Map<DocumentEF>(documentRequisite);

            documents.Add(document);
            SaveChanges();

            return document.id;
        }


        public DocumentRequisite GetFileParams(int documentId)
        {
            var document = documents.Find(documentId);
            return Mapper.Map<DocumentRequisite>(document);
        }


        public int CreateFilesList(string description = "")
        {
            /*var filesList = new FilesListEF();
            if (!string.IsNullOrEmpty(description))
            {
                filesList.description = description;
            }
            fileslists.Add(filesList);
            SaveChanges();

            return filesList.id;*/

            return Database.SqlQuery<int>("insert into fileslist (description)values(@description);select last_insert_id()",
                new MySqlParameter("description", description)).FirstOrDefault();
        }


        public List<DocumentRequisite> GetFiles(int fileListId = 0, List<int> types = null)
        {
            var filesQuery = documents.AsQueryable();

            if (fileListId > 0) {
                filesQuery = filesQuery.Where(f => f.fileslistid == fileListId);
            }

            if (types != null && types.Count > 0) {
                filesQuery = filesQuery.Where(f => types.Contains(f.documenttypeid));
            }

            return Mapper.Map<List<DocumentRequisite>>(filesQuery.ToList());
        }

        public List<DocumentRequisite> GetFilesFromList(int filesListId)
        {
            var files = this.documents.AsNoTracking().Where(f => f.fileslistid == filesListId);
            return Mapper.Map<List<DocumentRequisite>>(files.ToList());
        }

        public int CreateOrder(Order order, int initiatorId)
        {
            var orderEF = Mapper.Map<OrderEF>(order);
            orderEF.initiatorid = initiatorId;

            orders.Add(orderEF);
            SaveChanges();
            return orderEF.id;
        }

        public List<Applicant> GetApplicants(int auctionId)
        {
            var applicants = this.applicants.AsNoTracking().Where(a => a.auctionid == auctionId);
            List<ApplicantEF> applicantsList = applicants.ToList();
            return Mapper.Map<List<Applicant>>(applicantsList);
        }

        public List<Site> GetSites()
        {
            return Mapper.Map<List<Site>>(this.sites.ToList());
        }

        public bool RemoveDocumentInList(int fileListId, int documentId)
        {
            if (fileslists.Find(fileListId) != null) {
                var document = documents.FirstOrDefault(d => d.id == documentId && d.fileslistid == fileListId);
                if (document != null) {
                    document.fileslistid = null;
                    //documents.Attach(document);
                    SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool AppendFileToList(int listId, int fileId)
        {
            var file = documents.Find(fileId);
            if (file != null) {
                file.fileslistid = listId;
                documents.Attach(file);
                SaveChanges();
                return true;
            }
            return false;
        }

        public DocumentRequisite AddDocumentRequisite(DocumentRequisite documentRequisite)
        {
            var file = Mapper.Map<DocumentEF>(documentRequisite);
            if (documentRequisite.filesListId != default(int?) && !fileslists.Any(l => l.id == documentRequisite.filesListId))
                return null;
            if (documents.Find(file.id) != null)
                return null;
            var newFile = documents.Add(file);
            SaveChanges();
            return Mapper.Map<DocumentRequisite>(newFile);
        }

        public string GetFileListDescription(int fileListId)
        {
            var fileList = fileslists.Find(fileListId);
            return fileList != null ? fileList.description : null;
        }
        #endregion

        #region Broker
        public List<Broker> GetBrokersCompany(int companyId)
        {
            var company = companies.Find(companyId);
            if (company == null)
                return new List<Broker>();
            var brokersRes = company.contracts.Select(c => c.broker).Distinct();
            return Mapper.Map<List<Broker>>(brokersRes);
        }

        public Broker GetBroker(int brokerId)
        {
            return Mapper.Map<Broker>(brokers.Find(brokerId));
        }

        public int GetBrokerId(int companyId)
        {
            var broker = brokers.FirstOrDefault(b => b.companyId == companyId);
            return broker != null ? broker.id : 0;
        }

        public List<BrokerEF> GetBrokers()
        {
            return brokers.ToList();
        }

        public List<Broker> ReadBrokers()
        {
            return Database.SqlQuery<Broker>("select * from brokers", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region Configuration
        public ConficurationsBroker GetConfiguration(int brokerId)
        {
            return configurations.FirstOrDefault(c => c.brokerId == brokerId);
        }
        #endregion

        #region Contract
        public List<Contract> GetContractsByCompany(int companyId, int brokerid = 0, int siteid = 0)
        {
            var query = contracts.Where(c => c.companyid == companyId);
            if (brokerid > 0)
                query = query.Where(c => c.brokerid == brokerid);
            if (siteid > 0)
                query = query.Where(c => rateslists.Any(rl => rl.siteid == siteid && rl.contractid == c.id));

            return Mapper.Map<List<Contract>>(query.ToList());
        }


        public List<Contract> ReadContracts(int companyId, int brokerId)
        {
            return Database.SqlQuery<Contract>("select * from contracts where companyid=@companyId and brokerid=@brokerId",
                new MySqlParameter("companyId", companyId),
                new MySqlParameter("brokerId", brokerId)).ToList();
        }


        public List<ContractsReportView> ReadContractsReport(DateTime fromDate, DateTime toDate, string searchQuery)
        {
            return Database.SqlQuery<ContractsReportView>("select * from contractsReportView where agreementdate>=@FromDate and agreementdate<=@ToDate" +
                " and (number like @SearchQuery or companyName like @SearchQuery)",
                new MySqlParameter("FromDate", fromDate),
                new MySqlParameter("ToDate", toDate),
                new MySqlParameter("SearchQuery", string.Format("%{0}%", searchQuery))).ToList();
        }
        #endregion

        #region Report
        private IQueryable<TechSpecReportBO> GetTechSpecReportQuery(int customerId, List<int> siteId, List<int> statusId, DateTime auctionStartDate = default(DateTime), DateTime auctionEndDate = default(DateTime), DateTime orderStartDate = default(DateTime), DateTime orderEndDate = default(DateTime))
        {
            if (siteId == null) {
                siteId = new List<int>();
            }
            if (statusId == null) {
                statusId = new List<int>();
            }

            var query = techSpecReportsView.Where(t => t.customerId == customerId);

            if (auctionStartDate != default(DateTime) && auctionEndDate != default(DateTime))
                query = query.Where(t => t.auctionDate >= auctionStartDate && t.auctionDate <= auctionEndDate);

            if (orderStartDate != default(DateTime) && orderEndDate != default(DateTime))
                query = query.Where(t => t.orderDate >= orderStartDate && t.orderDate <= orderEndDate);

            if (siteId.Count > 0)
                query = query.Where(t => siteId.Any(s => s == t.siteId));

            if (statusId.Count > 0)
                query = query.Where(t => statusId.Any(s => s == t.statusId));

            return query;
        }

        public List<TechSpecReportBO> GetTechSpecReportByOrderDate(int customerId, DateTime orderStartDate, DateTime orderEndDate, List<int> siteId, List<int> statusId, int limitFrom = 0, int limitTo = 0, short sortMode = 0, string sortColumnName = null)
        {
            var query = GetTechSpecReportQuery(customerId, siteId, statusId, orderStartDate: orderStartDate, orderEndDate: orderEndDate);

            if (sortMode != 0 && !String.IsNullOrEmpty(sortColumnName)) {

                switch (sortMode) {
                    case (1):
                        query = query.OrderByProperty(sortColumnName);
                        break;
                    case (-1):
                        query = query.OrderByPropertyDescending(sortColumnName);
                        break;
                }

                query = ((IOrderedQueryable<TechSpecReportBO>)query).ThenBy(o => o.auctionNumber).ThenBy(o => o.id);

                if (limitFrom > 0 && limitTo > 0) {
                    query = query.Skip(limitFrom).Take(limitTo);
                }
            }

            return query.ToList();
        }

        public List<TechSpecReportBO> GetTechSpecReport(int customerId, DateTime startDate, DateTime endDate, List<int> siteId, List<int> statusId, int limitFrom = 0, int limitTo = 0, short sortMode = 0, string sortColumnName = null)
        {
            var query = GetTechSpecReportQuery(customerId, siteId, statusId, startDate, endDate);

            if (sortMode != 0 && !String.IsNullOrEmpty(sortColumnName)) {
                switch (sortMode) {
                    case (1):
                        query = query.OrderByProperty(sortColumnName);
                        break;
                    case (-1):
                        query = query.OrderByPropertyDescending(sortColumnName);
                        break;
                }

                query = ((IOrderedQueryable<TechSpecReportBO>)query).ThenBy(o => o.auctionNumber).ThenBy(o => o.id);

                if (limitFrom > 0 && limitTo > 0) {
                    query = query.Skip(limitFrom).Take(limitTo);
                }
            }

            return query.ToList();
        }

        public List<FinalReportPlmtl> GetFinalReportPlmtl(DateTime startDate, DateTime endDate)
        {
            return finalReportPlmtlView.Where(f => f.auctionDate >= startDate && f.auctionDate <= endDate).ToList();
        }

        public int CountTechSpecReport(int customerId, DateTime startDate, DateTime endDate, List<int> siteId, List<int> statusId)
        {
            var query = GetTechSpecReport(customerId, startDate, endDate, siteId, statusId);
            return query.Count();
        }

        public FinalReport GetFinalReport(int auctionId, int lotId, int supplierId)
        {
            var finalReport = finalreports.FirstOrDefault(f => f.auctionId == auctionId && f.lotId == lotId && f.supplierId == supplierId);
            return Mapper.Map<FinalReport>(finalReport);
        }


        public List<FinalReport> GetFinalReports(int auctionId, int supplierId)
        {
            return Mapper.Map<List<FinalReport>>(finalreports.Where(f => f.auctionId == auctionId && f.supplierId == supplierId).ToList());
        }


        public List<AltaBO.reports.AuctionResult> ReadAuctionsResultReport()
        {
            return Database.SqlQuery<AltaBO.reports.AuctionResult>("SELECT * FROM auctionsResultReportView", new MySqlParameter("", null)).ToList();
        }


        private IQueryable<EconomyViewEF> GetEconomyReportQuery(string number = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? siteid = default(int?), int? brokerid = default(int?), int? typeid = default(int?))
        {
            var query = economy.Include(a => a.supplier);

            if (!string.IsNullOrEmpty(number)) {
                query = query.Where(a => (a.aucNumber.Contains(number) || a.lotNumber.Contains(number)));
            }

            if (customerId != default(int?)) {
                query = query.Where(a => a.customerid == customerId);
            }

            if (fromDate != default(DateTime?) && toDate != default(DateTime?))
                query = query.Where(a => fromDate <= a.date && toDate >= a.date);

            if (siteid > 0)
                query = query.Where(a => siteid == a.siteid);

            if (brokerid != default(int?)) {
                query = query.Where(a => a.brokerid == brokerid);
            }
            if (typeid != default(int?)) {
                query = query.Where(a => a.typeid == typeid);
            }
            return query.AsNoTracking();
        }

        public int GetEconomyReportCount(string number = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? siteid = default(int?), int? brokerid = default(int?), int? typeid = default(int?))
        {
            var economyCount = GetEconomyReportQuery(number, customerId, fromDate, toDate, siteid, brokerid, typeid).Count();
            return economyCount;
        }

        public List<FinalReport> GetEconomyReport(int page, int countItems, string number = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? siteid = default(int?), int? brokerid = default(int?), int? typeid = default(int?))
        {
            var skip = (page - 1) * countItems;
            var economyCount = GetEconomyReportQuery(number, customerId, fromDate, toDate, siteid, brokerid, typeid);

            economyCount = economyCount.OrderByDescending(a => a.date);
            economyCount = economyCount.Skip(() => skip).Take(() => countItems).AsNoTracking();
            return Mapper.Map<List<FinalReport>>(economyCount.AsNoTracking().ToList());
        }

        public List<DealNumberInfo> ReadDealNumbersInfo()
        {
            return Database.SqlQuery<DealNumberInfo>("select * from dealNumbersView", new MySqlParameter("", null)).ToList();
        }

        #endregion

        #region SupplierJournal
        public SupplierJournal GetSupplierJournal(int id)
        {
            return Mapper.Map<SupplierJournal>(suppliersjournal.Find(id));
        }

        public SupplierJournal GetSupplierJournal(int brokerId, int supplierId)
        {
            var query = suppliersjournal.FirstOrDefault(sj => sj.brokerid == brokerId && sj.supplierid == supplierId);
            return Mapper.Map<SupplierJournal>(query);
        }

        public bool CreateSupplierOrder(int auctionId, int supplierId, int contractId, List<int> lotsIds)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Notification
        public List<Notification> GetNotifications(int supplierId = 0, int auctionId = 0, int eventId = 0, int belon = 0)
        {
            var query = notifications.AsQueryable();
            if (supplierId > 0)
                query = query.Where(n => n.supplierId == supplierId);
            if (auctionId > 0)
                query = query.Where(n => n.auctionId == auctionId);
            if (eventId > 0)
                query = query.Where(n => n.eventId == eventId);
            if (belon > 0)
                query = query.Where(n => n.event_.belongs == belon);
            return Mapper.Map<List<Notification>>(query.ToList());
        }
        #endregion

        #region SupplierOrder
        public SupplierOrder GetSupplierOrder(int id)
        {
            return Mapper.Map<SupplierOrder>(supplierorders.Find(id));
        }

        public SupplierOrder GetSupplierOrder(int auctionId, int supplierId)
        {
            SupplierOrderEF supplierOrder = null;
            var auction = auctions.Find(auctionId);
            if (auction != null) supplierOrder = auction.supplierOrders.FirstOrDefault(s => s.supplierid == supplierId);

            return Mapper.Map<SupplierOrder>(supplierOrder);
        }

        public List<SupplierOrderEF> GetSupplierOrders(int auctionId, int exceptStatusId)
        {
            return supplierorders.Where(s => s.auctionid == auctionId && (s.statusid != exceptStatusId || s.statusid == null)).ToList();
        }


        public Boolean UpdateStatusOnSupplierOrder(int supplierOrderId, int statusId)
        {
            var supplierOrder = supplierorders.Find(supplierOrderId);
            if (supplierOrder != null) {
                supplierOrder.statusid = statusId;
                SaveChanges();
                return false;
            }
            return false;
        }


        public SupplierOrder ReadSupplierOrder(int supplierId, int auctionId)
        {
            return Database.SqlQuery<SupplierOrder>("select * from supplierorders where supplierid=@supplierId and auctionid=@auctionId",
                new MySqlParameter("supplierId", supplierId),
                new MySqlParameter("auctionId", auctionId)).FirstOrDefault();
        }
        #endregion

        #region Procuratory
        public Procuratory GetProcuratory(int auctionId, int id)
        {
            var procuratory = auctions.Where(a => a.id == auctionId).Select(a => a.procuratories.FirstOrDefault(p => p.id == id));
            return Mapper.Map<Procuratory>(procuratory);
        }

        public bool AddProcuratory(int auctionId, Procuratory procuratory)
        {
            var procuratoryEf = Mapper.Map<ProcuratoryEF>(procuratory);
            var auction = auctions.Find(auctionId);
            if (auction != null) {
                auction.procuratories.Add(procuratoryEf);
                SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddProcuratories(int auctionId, List<Procuratory> procuratories)
        {
            var procuratoriesEf = Mapper.Map<List<ProcuratoryEF>>(procuratories);
            var auction = auctions.Find(auctionId);
            if (auction != null) {
                try {
                    foreach (var procuratoryEf in procuratoriesEf) {
                        auction.procuratories.Add(procuratoryEf);
                    }
                    SaveChanges();
                } catch (Exception) {
                    this.Entry(auctions).Reload();
                    return false;
                }
                return true;
            }
            return false;
        }


        public List<Procuratory> ReadProcuratories(int auctionId, int supplierId)
        {
            return Database.SqlQuery<Procuratory>("select * from procuratoriesListView where auctionid=@auctionId and supplierid=@supplierId",
                new MySqlParameter("auctionId", auctionId),
                new MySqlParameter("supplierId", supplierId)).ToList();
        }


        public Procuratory ReadProcuratory(int procuratoryId)
        {
            return Database.SqlQuery<Procuratory>("select * from procuratoriesListView where id=@procuratoryId",
                new MySqlParameter("procuratoryId", procuratoryId)).FirstOrDefault();
        }
        #endregion

        #region MailDeliveryExceptions
        public int AddMailDeliveryException(string lotName, DateTime date, string customer)
        {
            MailDeliveryException mde = new MailDeliveryException() {
                lot_Name = lotName,
                date = date,
                customer = customer
            };

            maildeliveryexceptions.Add(mde);
            SaveChanges();

            return mde.id;
        }

        public MailDeliveryException GetMailDeliveryException(string lotName, DateTime date, string customer)
        {
            return maildeliveryexceptions.FirstOrDefault(m => m.lot_Name.ToLower() == lotName.ToLower() && m.date == date && m.customer == customer);
        }
        #endregion

        #region Cases
        public int AddCase(int year, int brokerId, string caseName)
        {
            CaseEF caseInfo = new CaseEF() {
                Year = year,
                BrokerId = brokerId,
                Name = caseName
            };

            cases.Add(caseInfo);
            SaveChanges();

            return caseInfo.Id;
        }


        public List<CaseEF> GetCases(int year, int brokerId)
        {
            return cases.Where(c => c.Year == year && c.BrokerId == brokerId).ToList();
        }


        public bool UpdateCase(int id, string caseName)
        {
            var caseItem = cases.FirstOrDefault(c => c.Id == id);

            if (caseItem != null) {
                caseItem.Name = caseName;

                SaveChanges();
                return true;
            }

            return false;
        }
        #endregion

        #region ArchiveNumbers
        public int AddArchiveNumber(ArchiveNumberEF archiveNumberItem)
        {
            archivenumbers.Add(archiveNumberItem);
            SaveChanges();

            return archiveNumberItem.Id;
        }


        public List<DocumentEF> GetDocumentsWithArchiveNumbersInVolume(int volumeId)
        {
            return documents.Where(d => d.archiveNumber != null && d.archiveNumber.VolumeId == volumeId).ToList();
        }


        public bool UpdateArchiveNumber(ArchiveNumberEF archiveNumberInfo)
        {
            var archiveNumberItem = archivenumbers.FirstOrDefault(a => a.Id == archiveNumberInfo.Id);

            if (archiveNumberItem != null) {
                archiveNumberItem = archiveNumberInfo;
                SaveChanges();

                return true;
            }

            return false;
        }


        public int GetDocsCountInVolume(int volumeId)
        {
            return archivenumbers.Count(a => a.VolumeId == volumeId);
        }


        public int GetDocsCountInCase(int caseId)
        {
            return archivenumbers.Where(a => a.CaseId == caseId).GroupBy(a => a.DocumentNumber).Select(g => new { Cnt = g.Count() }).Count();
        }


        public int CheckDocInVolume(int volumeId, int archiveNumberId)
        {
            return archivenumbers.Count(a => a.Id == archiveNumberId && a.VolumeId == volumeId);
        }
        #endregion

        #region Documents
        public DocumentEF GetDocument(int documentId)
        {
            return documents.Find(documentId);
        }


        public bool UpdateDocumentWithArchiveNumber(int id, int archiveNumberId)
        {
            var documentItem = documents.FirstOrDefault(f => f.id == id);

            if (documentItem != null) {
                documentItem.archiveNumberId = archiveNumberId;
                SaveChanges();

                return true;
            }

            return false;
        }


        public List<DocumentView> ReadDocuments(int fileListId)
        {
            return Database.SqlQuery<DocumentView>("select * from documentsListView where fileslistid=@fileListId",
                new MySqlParameter("fileListId", fileListId)).ToList();
        }


        public List<DocumentType> ReadDocumentTypes()
        {
            return Database.SqlQuery<DocumentType>("select * from documentTypesListView", new MySqlParameter("", null)).ToList();
        }


        public void DeleteDocument(int documentId)
        {
            Database.SqlQuery<int>("delete from documents where id=@documentId;select last_insert_id()",
                new MySqlParameter("documentId", documentId)).FirstOrDefault();
        }


        public List<Document> SearchDocumentsInTS(string searchQuery)
        {
            return Database.SqlQuery<Document>("select * from documentsForArchiveView where number like @searchQuery or company like @searchQuery",
                new MySqlParameter("searchQuery", string.Format("%{0}%", searchQuery))).ToList();
        }
        #endregion

        #region Volumes
        public int AddVolume(string volumeName, int caseId)
        {
            var caseItem = cases.FirstOrDefault(c => c.Id == caseId);

            if (caseItem != null) {
                VolumeEF volumeItem = new VolumeEF();

                volumeItem.CaseId = caseId;
                volumeItem.Name = volumeName;
                volumeItem.StatusId = 12;

                volumes.Add(volumeItem);
                SaveChanges();

                return volumeItem.Id;
            }

            return 0;
        }

        public List<VolumeEF> GetVolumes(int caseId)
        {
            return volumes.Where(v => v.CaseId == caseId).ToList();
        }

        public bool UpdateVolume(int id, string volumeName)
        {
            var volumeItem = volumes.FirstOrDefault(v => v.Id == id);

            if (volumeItem != null) {
                volumeItem.Name = volumeName;

                SaveChanges();
                return true;
            }

            return false;
        }

        public bool SetVolumeStatus(int volumeId, int statusId)
        {
            var volumeItem = volumes.FirstOrDefault(v => v.Id == volumeId);

            if (volumeItem != null) {
                volumeItem.StatusId = statusId;

                SaveChanges();
                return true;
            }

            return false;
        }

        public bool SetVolumeName(int volumeId, string name)
        {
            var volumeItem = volumes.FirstOrDefault(v => v.Id == volumeId);

            if (volumeItem != null) {
                volumeItem.Name = name;

                SaveChanges();
                return true;
            }

            return false;
        }

        public int GetVolumeFirstDocNum(int volumeId)
        {
            var archiveNumbersItems = archivenumbers.Where(a => a.VolumeId == volumeId);

            if (archiveNumbersItems != null && archiveNumbersItems.Count() > 0) {
                return archiveNumbersItems.Min(a => a.DocumentNumber);
            }

            return 0;
        }

        public int GetVolumeLastDocNum(int volumeId)
        {
            var archiveNumbersItems = archivenumbers.Where(a => a.VolumeId == volumeId);

            if (archiveNumbersItems != null && archiveNumbersItems.Count() > 0) {
                return archiveNumbersItems.Max(a => a.DocumentNumber);
            }

            return 0;
        }

        #endregion

        #region Person
        public int CreatePerson(Person person)
        {
            try {
                var entity = Mapper.Map<PersonEF>(person);
                persons.Add(entity);
                SaveChanges();
                return entity.id;
            } catch (Exception) {
                return 0;
            }
        }
        #endregion

        #region Traders
        public List<Trader> ReadTraders()
        {
            return Database.SqlQuery<Trader>("select t.*, p.name from traders as t left join persons as p on p.id=t.personid order by p.name", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region Storage Procedure
        public List<AnaliticCountStatus> CustCount(int customerId, DateTime startDate, DateTime endDate)
        {
            return Mapper.Map<List<AnaliticCountStatus>>(this.CustCountProcedure(customerId, startDate, endDate));
        }

        public List<AnaliticCountStatus> SupCount(int supplierId, DateTime startDate, DateTime endDate)
        {
            return Mapper.Map<List<AnaliticCountStatus>>(this.SupCountProcedure(supplierId, startDate, endDate));
        }
        #endregion

        #region Online StockQuotes
        public void CreateStockQuotes(string lotCode, decimal priceOffer, string brokerCode, DateTime offerTime)
        {
            try {
                Database.SqlQuery<int>("insert into stockquotes(lotcode, priceoffer, brokercode, moment)values(@lotCode, @priceOffer, @brokerCode, @offerTime); select last_insert_id();",
                    new MySqlParameter("lotCode", lotCode),
                    new MySqlParameter("priceOffer", priceOffer),
                    new MySqlParameter("brokerCode", brokerCode),
                    new MySqlParameter("offerTime", offerTime)).First();
            } catch { }
        }
        #endregion

        #region Debtors
        public List<DebtorsList> ReadDebtorsList()
        {
            return Database.SqlQuery<DebtorsList>("select * from debtorsListView group by supplierId", new MySqlParameter("", null)).ToList();
        }


        public List<DebtorsList> ReadDebtorsList(string customerName, string supplierName)
        {
            return Database.SqlQuery<DebtorsList>("select * from debtorsListView where companyName like @supplierName and customerName like @customerName group by supplierId",
                new MySqlParameter("customerName", string.Format("%{0}%", customerName)),
                new MySqlParameter("supplierName", string.Format("%{0}%", supplierName))).ToList();
        }


        public List<DebtorDetails> ReadDebtorDetails(int supplierId, int statusId)
        {
            return Database.SqlQuery<DebtorDetails>("select * from debtorsDetailView where supplierId=@supplierId and statusId=@statusId",
                new MySqlParameter("supplierId", supplierId),
                new MySqlParameter("statusId", statusId)).ToList();
        }
        #endregion

        #region RatesList
        public RatesList ReadRatesList(int contractId, int siteId)
        {
            return Database.SqlQuery<RatesList>("select * from rateslist where contractid=@contractId and siteid=@siteId",
                new MySqlParameter("contractId", contractId),
                new MySqlParameter("siteId", siteId)).LastOrDefault();
        }


        public List<RatesList> ReadRatesLists(int contractId)
        {
            return Database.SqlQuery<RatesList>("select * from rateslist where contractid=@contractId",
                new MySqlParameter("contractId", contractId)).ToList();
        }
        #endregion

        #region Rates
        public List<Rate> ReadRates(int ratesListId)
        {
            return Database.SqlQuery<Rate>("select * from rates where rateslistid=@ratesListId", new MySqlParameter("ratesListId", ratesListId)).ToList();
        }
        #endregion

        #region FinalReport
        public void UpdateFinalReport(int finalReportId, int statusId)
        {
            Database.SqlQuery<int>("update finalreport set statusid=@statusId where id=@finalReportId;select last_insert_id();",
                new MySqlParameter("finalReportId", finalReportId),
                new MySqlParameter("statusId", statusId)).First();
        }
        #endregion

        #region Regulation
        public Regulation ReadRegulation(int regulationId)
        {
            return Database.SqlQuery<Regulation>("select * from regulations where id=@regulationId",
                new MySqlParameter("regulationId", regulationId)).First();
        }


        public int CreateRegulation(Regulation regulation)
        {
            return Database.SqlQuery<int>("insert into regulations (opendate, closedate, applydate, applydeadline, applicantsdeadline, provisiondeadline)" +
                "values(@openDate, @closeDate, @applyDate, @applyDeadLine, @applicantsDeadLine, @provisionDeadLine);select last_insert_id()",
                new MySqlParameter("openDate", regulation.openDate),
                new MySqlParameter("closeDate", regulation.closeDate),
                new MySqlParameter("applyDate", regulation.applyDate),
                new MySqlParameter("applyDeadLine", regulation.applyDeadLine),
                new MySqlParameter("applicantsDeadLine", regulation.applicantsDeadLine),
                new MySqlParameter("provisionDeadLine", regulation.provisionDeadLine)).First();
        }


        public void UpdateRegulation(Regulation regulation, int regulationId)
        {
            Database.SqlQuery<int>("update regulations set opendate=@openDate, closedate=@closeDate, applydate=@applyDate, applydeadline=@applyDeadLine" +
                ", applicantsdeadline=@applicantsDeadLine, provisiondeadline=@provisionDeadLine where id=@regulationId;select last_insert_id()",
                new MySqlParameter("openDate", regulation.openDate),
                new MySqlParameter("closeDate", regulation.closeDate),
                new MySqlParameter("applyDate", regulation.applyDate),
                new MySqlParameter("applyDeadLine", regulation.applyDeadLine),
                new MySqlParameter("applicantsDeadLine", regulation.applicantsDeadLine),
                new MySqlParameter("provisionDeadLine", regulation.provisionDeadLine),
                new MySqlParameter("regulationId", regulationId)).First();
        }
        #endregion

        #region Sections
        public List<Section> ReadSections()
        {
            return Database.SqlQuery<Section>("select * from sections", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region AuctionType
        public List<AuctionType> ReadTypes()
        {
            return Database.SqlQuery<AuctionType>("select * from types", new MySqlParameter("", null)).ToList();
        }
        #endregion

        #region Unit
        public List<Unit> ReadUnits()
        {
            return Database.SqlQuery<Unit>("select * from units", new MySqlParameter("", null)).ToList();
        }

        public int GetAuctionsCount(string numberOrProduct = null, int? customerId = default(int?), DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?), int? site = default(int?), int? supplierId = default(int?), int? statusId = default(int?), int winner = 0, int? brokerId = default(int?), int? traderId = default(int?))
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Tables
        public virtual DbSet<ProductCompanyEF> productsCompany { get; set; }
        public virtual DbSet<PersonEF> persons { get; set; }
        public virtual DbSet<UserEF> users { get; set; }
        public virtual DbSet<AuctionEF> auctions { get; set; }
        public virtual DbSet<EconomyViewEF> economy { get; set; }
        public virtual DbSet<CustomerEF> customers { get; set; }
        public virtual DbSet<CompanyEF> companies { get; set; }
        public virtual DbSet<SiteEF> sites { get; set; }
        public virtual DbSet<BrokerEF> brokers { get; set; }
        public virtual DbSet<AuctionServiceEF> auctionServices { get; set; }
        public virtual DbSet<RegulationEF> regulations { get; set; }
        public virtual DbSet<BrokersJournalEF> brokersjournal { get; set; }
        public virtual DbSet<SupplierEF> suppliers { get; set; }
        public virtual DbSet<SuppliersJournalEF> suppliersjournal { get; set; }
        public virtual DbSet<LotEF> lots { get; set; }
        public virtual DbSet<ContractEF> contracts { get; set; }
        public virtual DbSet<SupplierOrderEF> supplierorders { get; set; }
        public virtual DbSet<ApplicantEF> applicants { get; set; }
        public virtual DbSet<ProcuratoryEF> procuratories { get; set; }
        public virtual DbSet<UnitEF> units { get; set; }
        public virtual DbSet<TraderEF> traders { get; set; }
        public virtual DbSet<EmailSettingEF> emailsettings { get; set; }
        public virtual DbSet<EmployeeEF> employees { get; set; }
        public virtual DbSet<OrderEF> orders { get; set; }
        public virtual DbSet<SectionEF> sections { get; set; }
        public virtual DbSet<TypeEF> types { get; set; }
        public virtual DbSet<StatusEF> statuses { get; set; }
        public virtual DbSet<SerialNumberEF> serialnumbers { get; set; }
        public virtual DbSet<CountryEF> countries { get; set; }
        public virtual DbSet<BankEF> banks { get; set; }
        public virtual DbSet<ContractTypeEF> contracttypes { get; set; }
        public virtual DbSet<CurrencyEF> currencies { get; set; }
        public virtual DbSet<ProductEF> products { get; set; }
        public virtual DbSet<ScanEF> scans { get; set; }
        public virtual DbSet<DebtorEF> debtors { get; set; }
        public virtual DbSet<RequestedDocEF> requesteddocs { get; set; }
        public virtual DbSet<DocumentEF> documents { get; set; }
        public virtual DbSet<FilesListEF> fileslists { get; set; }
        public virtual DbSet<RatesListEF> rateslists { get; set; }
        public virtual DbSet<RateEF> rates { get; set; }
        public virtual DbSet<LotsExtendedEF> lotsextended { get; set; }
        public virtual DbSet<FileSectionEF> filesections { get; set; }
        public virtual DbSet<ClearingCountingEF> clearingcountings { get; set; }
        public virtual DbSet<OtherDocsEF> otherdocs { get; set; }
        public virtual DbSet<DocumentTypeEF> documenttypes { get; set; }
        public virtual DbSet<ListServEF> listservs { get; set; }
        public virtual DbSet<EnvelopEF> envelops { get; set; }
        public virtual DbSet<EnvelopContentEF> envelopcontents { get; set; }
        public virtual DbSet<FinalReportEF> finalreports { get; set; }
        public virtual DbSet<NotificationEF> notifications { get; set; }
        public virtual DbSet<QualificationEF> qualifications { get; set; }
        public virtual DbSet<QualificationDictionaryEF> qualificationdictionary { get; set; }
        public virtual DbSet<ArchiveNumberEF> archivenumbers { get; set; }
        public virtual DbSet<CaseEF> cases { get; set; }
        public virtual DbSet<VolumeEF> volumes { get; set; }
        #endregion

        #region Table (new method implement)
        public virtual DbSet<ConficurationsBroker> configurations { get; set; }
        public virtual DbSet<MailDeliveryException> maildeliveryexceptions { get; set; }
        #endregion

        #region Views
        public virtual DbSet<AuctionsView> auctionsview { get; set; }
        public virtual DbSet<AuctionsResultView> auctionsresultview { get; set; }
        public virtual DbSet<CompaniesWithContractView> companieswithcontractview { get; set; }
        public virtual DbSet<SuppliersWithContractsView> supplierswithcontractsview { get; set; }
        public virtual DbSet<ListServView> listservsview { get; set; }
        public virtual DbSet<EnvelopsView> envelopsview { get; set; }
        public virtual DbSet<TechSpecReportBO> techSpecReportsView { get; set; }
        public virtual DbSet<FinalReportPlmtl> finalReportPlmtlView { get; set; }
        #endregion
    }
}
