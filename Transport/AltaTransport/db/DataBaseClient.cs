using AltaBO;
using AltaBO.specifics;
using AltaMySqlDB.model;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.model.views;
using AltaMySqlDB.service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AltaTransport
{
    public class DataBaseClient : EntityContext, IDataManager
    {
        private static EntityContext context = new EntityContext();

        public static List<AuctionsView> ReadAuctionsView()
        {
            return context.auctionsview.ToList();
        }

        #region Auctions
        public static int CreateAuction(AuctionEF auctionInfo)
        {
            context.regulations.Add(auctionInfo.regulation);

            var filesList = new FilesListEF() { description = "Файлы аукциона №" + auctionInfo.number };

            context.fileslists.Add(filesList);
            context.SaveChanges();

            auctionInfo.fileslistid = filesList.id;

            context.auctions.Add(auctionInfo);
            context.SaveChanges();

            return auctionInfo.id;
        }


        public static AuctionEF ReadAuction(int id)
        {
            return context.auctions.FirstOrDefault(a => a.id == id);
        }


        public static List<AuctionEF> ReadAuctions()
        {
            return context.auctions.ToList();
        }


        public static List<AuctionEF> ReadAuctions(DateTime fromDate, DateTime toDate, int site, int statusId, int traderId)
        {
            if (statusId == 0) {
                if (traderId != 1) return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site && a.traderid == traderId).OrderBy(a => a.date).ToList();
                else return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site).OrderBy(a => a.date).ToList();
            } else {
                if (traderId != 1) return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site && a.statusid == statusId && a.traderid == traderId).OrderBy(a => a.date).ToList();
                else return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site && a.statusid == statusId).OrderBy(a => a.date).ToList();
            }
        }


        public static List<AuctionEF> ReadAuctions(DateTime fromDate, DateTime toDate, int site, int statusId, int traderId, string searchQuery)
        {
            if (statusId == 0) {
                if (traderId != 1) return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site && a.traderid == traderId &&
                   (a.number.ToLower().Contains(searchQuery.ToLower()) || context.supplierorders.Any(s => s.auctionid == a.id && s.supplier.company.name.ToLower().Contains(searchQuery.ToLower())))).OrderBy(a => a.date).ToList();
                else return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site &&
                (a.number.ToLower().Contains(searchQuery.ToLower()) || context.supplierorders.Any(s => s.auctionid == a.id && s.supplier.company.name.ToLower().Contains(searchQuery.ToLower())))).OrderBy(a => a.date).ToList();
            } else {
                if (traderId != 1) return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site && a.statusid == statusId && a.traderid == traderId &&
                   (a.number.ToLower().Contains(searchQuery.ToLower()) || context.supplierorders.Any(s => s.auctionid == a.id && s.supplier.company.name.ToLower().Contains(searchQuery.ToLower())))).OrderBy(a => a.date).ToList();
                else return context.auctions.Where(a => a.date >= fromDate && a.date <= toDate && a.siteid == site && a.statusid == statusId &&
                (a.number.ToLower().Contains(searchQuery.ToLower()) || context.supplierorders.Any(s => s.auctionid == a.id && s.supplier.company.name.ToLower().Contains(searchQuery.ToLower())))).OrderBy(a => a.date).ToList();
            }
        }


        public static void UpdateAuction(Order orderInfo)
        {
            var regulationItem = context.regulations.Where(r => r.id == orderInfo.Auction.RegulationId).FirstOrDefault();

            regulationItem.closedate = orderInfo.Auction.Date;
            regulationItem.applydeadline = orderInfo.Deadline;
            regulationItem.applicantsdeadline = orderInfo.Auction.ApplicantsDeadline;
            regulationItem.provisiondeadline = orderInfo.Auction.ExchangeProvisionDeadline;

            var auctionItem = ReadAuction(orderInfo.Auction.Id);

            auctionItem.date = orderInfo.Auction.Date;
            auctionItem.statusid = 4;
            auctionItem.number = orderInfo.Auction.Number;

            context.SaveChanges();
        }


        public static void UpdateAuction(AuctionEF auctionInfo)
        {
            var regulationItem = ReadRegulation(auctionInfo.regulationid);

            regulationItem = auctionInfo.regulation;

            var auctionItem = ReadAuction(auctionInfo.id);

            auctionItem = auctionInfo;

            context.SaveChanges();
        }


        public static void DeleteAuction(int id)
        {
            var auctionItem = ReadAuction(id);

            var regulationItem = ReadRegulation(auctionItem.regulationid);

            context.auctions.Remove(auctionItem);
            context.regulations.Remove(regulationItem);
            context.SaveChanges();
        }


        public static AuctionEF GetFileListAuctionByLot(string lotCode)
        {
            var lot = context.lots.FirstOrDefault(l => l.number.ToLower().Contains(lotCode.ToLower()));

            if (lot == null) return null;

            return context.auctions.FirstOrDefault(a => a.id == lot.auctionid);
        }


        public static void SetAuctionStatus(int auctionId, int status)
        {
            var auction = context.auctions.FirstOrDefault(a => a.id == auctionId);

            if (auction != null) {
                auction.statusid = status;
                context.SaveChanges();
            }
        }
        #endregion

        #region Regulations
        public static RegulationEF ReadRegulation(int? id)
        {
            return context.regulations.Where(r => r.id == id).FirstOrDefault();
        }
        #endregion

        #region Units
        public static List<UnitEF> ReadUnits()
        {
            return context.units.ToList();
        }
        #endregion

        #region Lots
        public static int CreateLot(LotEF lotInfo)
        {
            context.lots.Add(lotInfo);

            context.SaveChanges();

            return lotInfo.id;
        }


        public static LotEF ReadLot(int id)
        {
            return context.lots.Where(l => l.id == id).FirstOrDefault();
        }


        public static List<LotEF> ReadLots(int? auctionId)
        {
            return context.lots.Where(l => l.auctionid == auctionId).ToList();
        }


        public static List<LotEF> ReadLots(int customerId, int sourceId, int status)
        {
            var lotList = from lot in context.lots
                          join auction in context.auctions on lot.auctionid equals auction.id
                          where auction.customerid == customerId && auction.siteid == sourceId && auction.statusid == status
                          select lot;

            return lotList.ToList();
        }


        public static void UpdateLot(LotEF lotInfo)
        {
            var lotItem = ReadLot(lotInfo.id);

            lotItem = lotInfo;

            /*lotItem.price = lotInfo.price;
            lotItem.sum = lotInfo.sum;
            lotItem.description = lotInfo.description;*/

            context.SaveChanges();
        }


        public static void DeleteLot(int id)
        {
            var lotItem = ReadLot(id);

            context.lots.Remove(lotItem);
            context.SaveChanges();
        }


        public static decimal GetPriceFromLot(string lotCode)
        {
            var lot = context.lots.FirstOrDefault(l => l.number == lotCode);

            return lot != null ? lot.sum : 0;
        }


        public static LotEF GetLotByCode(string lotCode)
        {
            var lot = context.lots.FirstOrDefault(l => l.number == lotCode);

            return lot != null ? lot : null;
        }
        #endregion

        #region LotsExtended
        public static void CreateLotsExtended(Order order, int lotId, string lotNumber = null, string lotDescription = null)
        {
            var lot = order.Auction.Lots.FirstOrDefault(l => l.Id == lotId);

            if (lot != null) {
                foreach (var item in lot.LotsExtended) {
                    context.lotsextended.Add(new LotsExtendedEF() {
                        serialnumber = item.serialnumber,
                        lotid = lotId,
                        name = item.name,
                        unit = item.unit,
                        quantity = item.quantity,
                        price = item.price,
                        sum = item.sum,
                        country = item.country,
                        techspec = item.techspec,
                        terms = item.terms,
                        paymentterm = item.paymentterm,
                        dks = item.dks,
                        contractnumber = item.contractnumber
                    });
                }

                context.SaveChanges();
            }
        }

        public static List<LotsExtendedEF> ReadLotsExtended(int lotId)
        {
            return context.lotsextended.Where(l => l.lotid == lotId).ToList();
        }

        public static List<LotsExtendedEF> ReadLotsExtended(DateTime startDate, DateTime endDate)
        {
            return context.lotsextended.Where(l => l.lot.auction.date >= startDate && l.lot.auction.date <= endDate).ToList();
        }

        public static void UpdateLotsExtended(Order order, int lotId, string lotNumber = null, string lotDescription = null)
        {
            var lotsExtended = context.lotsextended.Where(l => l.lotid == lotId);

            if (lotsExtended != null && lotsExtended.Count() > 0) {
                context.lotsextended.RemoveRange(lotsExtended);
                context.SaveChanges();
            }

            CreateLotsExtended(order, lotId, lotNumber, lotDescription);
        }

        public static void UpdateLotsExtended(List<LotsExtendedEF> lotExInfo)
        {
            foreach (var item in lotExInfo) {
                var lotEx = context.lotsextended.FirstOrDefault(l => l.id == item.id);

                if (lotEx != null) {
                    lotEx = item;
                    context.SaveChanges();
                }
            }
        }

        public static void DeleteLotsExtended(int lotId)
        {
            var lotsEx = context.lotsextended.Where(le => le.id == lotId);

            if (lotsEx != null && lotsEx.Count() > 0) {
                context.lotsextended.RemoveRange(lotsEx);
                context.SaveChanges();
            }
        }
        #endregion

        #region SupplierOrders
        public static int CreateSupplierOrder(SupplierOrderEF supplierOrderInfo)
        {
            context.supplierorders.Add(supplierOrderInfo);

            context.SaveChanges();

            return supplierOrderInfo.id;
        }


        public static void CreateSupplierOrder(Order order)
        {
            if (context.supplierorders.Where(s => s.supplierid == order.Auction.SupplierOrders[0].Id && s.auctionid == order.Auction.Id).Count() == 0) {
                ContractEF contractId = context.contracts.Where(c => c.number == order.Auction.SupplierOrders[0].ContractNum).FirstOrDefault();

                context.supplierorders.Add(new SupplierOrderEF {
                    supplierid = order.Auction.SupplierOrders[0].Id,
                    auctionid = order.Auction.Id,
                    contractid = contractId.id,
                    date = DateTime.Now
                });

                context.SaveChanges();
            }
        }

        public static List<SupplierOrderEF> ReadSupplierOrders()
        {
            return context.supplierorders.ToList();
        }

        public static SupplierOrderEF ReadSupplierOrder(int id)
        {
            return context.supplierorders.Where(s => s.id == id).FirstOrDefault();
        }


        public static List<SupplierOrderEF> ReadSupplierOrders(int auctionId)
        {
            return context.supplierorders.Where(s => s.auctionid == auctionId).ToList();
        }


        public static List<SupplierOrderEF> ReadSupplierOrders(int auctionId, int exceptStatusId)
        {
            return context.supplierorders.Where(s => s.auctionid == auctionId && (s.statusid != exceptStatusId || s.statusid == null)).ToList();
        }


        public static void UpdateSupplierOrder(SupplierOrderEF supplierOrderInfo)
        {
            var supplierOrderItem = ReadSupplierOrder(supplierOrderInfo.id);

            supplierOrderItem = supplierOrderInfo;
            context.SaveChanges();
        }


        public static void DeleteSupplierOrder(int id)
        {
            context.applicants.RemoveRange(context.applicants.Where(a => a.supplierorderid == id));
            context.SaveChanges();

            context.supplierorders.Remove(context.supplierorders.Where(s => s.id == id).FirstOrDefault());
            context.SaveChanges();
        }
        #endregion

        #region Applicants
        public static void CreateApplicants(Order order, int lotId)
        {
            if (context.applicants.Where(a => a.auctionid == order.Auction.Id && a.lotid == lotId).Count() > 0) {
                context.applicants.RemoveRange(context.applicants.Where(a => a.auctionid == order.Auction.Id));
                context.SaveChanges();
            }

            foreach (var item in order.Auction.SupplierOrders) {
                int supplierOrderId = item.Id;

                context.applicants.Add(new ApplicantEF() {
                    auctionid = order.Auction.Id,
                    supplierorderid = supplierOrderId,
                    lotid = lotId
                });

                var supplierOrder = ReadSupplierOrder(item.Id);

                if (supplierOrder != null) supplierOrder.statusid = 15;
            }

            context.SaveChanges();
        }


        public static List<ApplicantEF> ReadApplicants(int auctionId)
        {
            return context.applicants.Where(a => a.auctionid == auctionId).ToList();
        }
        #endregion

        #region Contracts
        public static int CreateContract(ContractEF contractInfo)
        {
            context.contracts.Add(contractInfo);

            context.SaveChanges();

            return context.contracts.OrderByDescending(i => i.id).FirstOrDefault().id;
        }


        public static ContractEF ReadContract(int contractId)
        {
            return context.contracts.Where(c => c.id == contractId).FirstOrDefault();
        }


        public static ContractEF GetContractByCompany(int companyId, int brokerId)
        {
            return context.contracts.Where(c => c.companyid == companyId && c.brokerid == brokerId).FirstOrDefault();
        }


        public static List<ContractEF> ReadContracts(int companyId)
        {
            return context.contracts.AsNoTracking().Where(c => c.companyid == companyId).ToList();
        }

        public static List<ContractEF> ReadContracts(int companyid, int brokerid)
        {
            return context.contracts.AsNoTracking().Where(c => c.companyid == companyid && c.brokerid == brokerid).ToList();
        }

        public static void UpdateContract(ContractEF contractInfo)
        {
            ContractEF contractItem = ReadContract(contractInfo.id);

            contractItem = contractInfo;

            context.SaveChanges();
        }

        public static bool UpdateContractFile(int contractId, int fileId)
        {
            var contract = context.contracts.FirstOrDefault(c => c.id == contractId);

            if (contract == null) return false;

            contract.documentId = fileId;
            context.SaveChanges();

            return true;
        }

        public static void DeleteContract(int id)
        {
            context.contracts.Remove(context.contracts.Where(c => c.companyid == id).FirstOrDefault());
        }

        public static bool CheckDuplicateContract(ContractEF contractItem)
        {
            if (context.contracts.Where(c => c.number == contractItem.number && c.contracttypeid == contractItem.contracttypeid && c.brokerid == contractItem.brokerid && c.companyid == contractItem.companyid).Count() > 0) return true;

            return false;
        }

        public static string[] GetSuppliersBins(int brokerId)
        {
            var clientsBins = context.contracts.Where(c => c.brokerid == brokerId && c.companyid != null && c.company.bin != null).Select(c => c.company.bin).Distinct().ToArray();

            if (clientsBins == null) return null;

            string[] cBins = new string[clientsBins.Count()];

            cBins = clientsBins;

            return cBins;
        }
        #endregion

        #region Countries
        public static List<CountryEF> ReadCountries()
        {
            return context.countries.ToList();
        }
        #endregion

        #region Companies
        public static int CreateCompany(CompanyEF companyInfo)
        {
            context.companies.Add(companyInfo);

            context.SaveChanges();

            return companyInfo.id;
        }


        public static CompanyEF ReadCompany(int id)
        {
            return context.companies.Where(c => c.id == id).FirstOrDefault();
        }


        public static CompanyEF ReadCompany(string bin)
        {
            return context.companies.FirstOrDefault(c => c.bin == bin);
        }


        public static List<CompanyEF> ReadCompanies()
        {
            return context.companies.ToList();
        }


        public static List<CompanyEF> ReadCompanies(string companyName = null, string companyBin = null)
        {
            return context.companies.Where(c => (companyName != null ? (c.name.ToLower().Contains(companyName.ToLower())) : true) && (companyBin != null ? (c.bin.Contains(companyBin)) : true)).ToList();
        }


        public static void UpdateCompany(CompanyEF companyInfo)
        {
            CompanyEF companyItem = ReadCompany(companyInfo.id);

            companyItem = companyInfo;

            context.SaveChanges();
        }


        public static void DeleteCompany(int companyId)
        {
            context.companies.Remove(context.companies.Where(c => c.id == companyId).FirstOrDefault());
            context.SaveChanges();
        }


        public static bool CheckDuplicateCompany(CompanyEF companyItem)
        {
            if (context.companies.Where(c => c.bin == companyItem.bin).Count() > 0) return true;

            return false;
        }

        public static string GetCompanyName(string BIN)
        {
            return context.companies.FirstOrDefault(c => c.bin == BIN).name;
        }
        #endregion

        #region Banks
        public static List<BankEF> ReadBanks()
        {
            return context.banks.ToList();
        }
        #endregion

        #region ContractTypes
        public static List<ContractTypeEF> ReadContractTypes()
        {
            return context.contracttypes.ToList();
        }
        #endregion

        #region Currencies
        public static List<CurrencyEF> ReadCurrencies()
        {
            return context.currencies.ToList();
        }
        #endregion

        #region Suppliers
        public static void CreateSupplier(int companyId)
        {
            context.suppliers.Add(new SupplierEF { companyid = companyId });
            context.SaveChanges();
        }

        public static SupplierEF ReadSupplier(int companyId)
        {
            return context.suppliers.Where(s => s.companyid == companyId).FirstOrDefault(); ;
        }

        public static void DeleteSupplier(int companyId)
        {
            context.suppliers.Remove(context.suppliers.Where(s => s.companyid == companyId).FirstOrDefault());
            context.SaveChanges();
        }

        public static new int GetSupplierId(int companyId)
        {
            var supplier = context.suppliers.FirstOrDefault(s => s.companyid == companyId);

            return supplier != null ? supplier.id : 0;
        }

        public static string GetSupplierNameByCode(string code)
        {
            var supplier = context.suppliersjournal.FirstOrDefault(s => s.code.ToLower() == code.ToLower());

            if (supplier == null) return null;
            else return supplier.supplier.company.name;
        }

        public static string[] GetSupplierByBroker_Auction(string brokerCode, int auctionId, int lotId)
        {
            var brokerId = context.brokersjournal.FirstOrDefault(b => b.code.ToUpper() == brokerCode.ToUpper());

            if (brokerId != null) {
                var supplierOrders = context.supplierorders.Where(s => s.auctionid == auctionId && s.contract.brokerid == brokerId.brokerid && s.statusid != 16);

                if (supplierOrders != null) {
                    //var procuratorys = context.procuratories.Where(p => p.auctionid == auctionId);
                    List<SupplierOrderEF> supplierOrdersInfo = new List<SupplierOrderEF>(supplierOrders);

                    foreach (var item in supplierOrdersInfo) {
                        var pCount = context.procuratories.Count(p => p.auctionid == auctionId && p.lotid == lotId && p.supplierid == item.supplierid);

                        if (pCount > 0) {
                            var procuratory = context.procuratories.FirstOrDefault(p => p.auctionid == auctionId && p.lotid == lotId && p.supplierid == item.supplierid);

                            if (procuratory != null) {
                                var supplierJournal = context.suppliersjournal.FirstOrDefault(s => s.supplierid == procuratory.supplierid && s.brokerid == brokerId.brokerid);

                                if (supplierJournal != null) {
                                    return new string[] { supplierJournal.code, procuratory.supplier.company.name, procuratory.supplierid.ToString(), brokerId.brokerid.ToString(), item.id.ToString() };
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static List<CompaniesWithContractView> GetSuppliersWithContract()
        {
            //return context.Database.SqlQuery<CompaniesWithContractView>("", new MySqlParameter("",null)).ToList();
            return context.companieswithcontractview.ToList();
        }

        public static List<CompaniesWithContractView> GetSuppliersWithContract(int brokerId)
        {
            return context.companieswithcontractview.Where(c => c.brokerId == brokerId).ToList();
        }

        public static List<SuppliersWithContractsView> GetSuppliersWithContracts()
        {
            return context.supplierswithcontractsview.ToList();
        }
        #endregion

        #region Customer
        public static void CreateCustomer(int companyId)
        {
            context.customers.Add(new CustomerEF { companiesid = companyId });
            context.SaveChanges();
        }


        public static CustomerEF ReadCustomer(int companyId)
        {
            if (context.customers.Where(c => c.companiesid == companyId).Count() > 0) return context.customers.Where(c => c.companiesid == companyId).FirstOrDefault();
            else return null;
        }


        public static void DeleteCustomer(int companyId)
        {
            context.customers.Remove(context.customers.FirstOrDefault(c => c.companiesid == companyId));
            context.SaveChanges();
        }
        #endregion

        #region Procuratories
        public static int CreateProcuratory(ProcuratoryEF procuratoryItem)
        {
            context.procuratories.Add(procuratoryItem);
            context.SaveChanges();

            return procuratoryItem.id;
        }

        public static void CreateProcuratory(Order order)
        {
            int supplierId = order.Auction.SupplierOrders[0].SupplierId;
            int auctionId = order.Auction.Id;

            foreach (var item in order.Auction.Lots) {
                int lotId = item.Id;

                var procuratory = context.procuratories.FirstOrDefault(p => p.supplierid == supplierId && p.auctionid == auctionId && p.lotid == lotId);

                if (procuratory != null) { // Update
                    procuratory.minimalprice = item.Sum;
                } else { // Create
                    var procuratoryItem = new ProcuratoryEF() {
                        supplierid = supplierId,
                        auctionid = auctionId,
                        lotid = lotId,
                        minimalprice = item.Sum
                    };

                    context.procuratories.Add(procuratoryItem);
                }
            }

            context.SaveChanges();
        }

        public static void CreateProcuratory(Order order, decimal minimalPrice, bool single = false)
        {
            int supplierId = order.Auction.SupplierOrders[0].SupplierId;
            int auctionId = order.Auction.Id;
            int lotId = order.Auction.Lots[0].Id;

            var procuratory = context.procuratories.FirstOrDefault(p => p.supplierid == supplierId && p.auctionid == auctionId && p.lotid == lotId);

            if (procuratory != null) { // Update
                procuratory.minimalprice = minimalPrice;
            } else { // Create
                var procuratoryItem = new ProcuratoryEF() {
                    supplierid = supplierId,
                    auctionid = auctionId,
                    lotid = lotId,
                    minimalprice = minimalPrice
                };

                context.procuratories.Add(procuratoryItem);
            }

            context.SaveChanges();
        }

        public static void CreateProcuratory(Order order, decimal minimalPrice)
        {
            int supplierId = order.Auction.SupplierOrders[0].SupplierId;
            int auctionId = order.Auction.Id;
            int lotId = order.Auction.Lots[0].Id;
            int supplierOrderId = order.Auction.SupplierOrders[0].Id;

            var suppliersCount = context.procuratories.Count(p => p.supplierid == supplierId && p.auctionid == auctionId && p.lotid == lotId);

            if (suppliersCount != 0) {
                var procuratoryItem = (from procuratory in context.procuratories
                                       where procuratory.supplierid == supplierId && procuratory.auctionid == auctionId && procuratory.lotid == lotId
                                       select procuratory).FirstOrDefault();

                procuratoryItem.minimalprice = minimalPrice;
            } else {
                var supplierOrderItem = (from supplierOrder in context.supplierorders
                                         where supplierOrder.id == supplierOrderId
                                         select supplierOrder).FirstOrDefault();

                context.procuratories.Add(new ProcuratoryEF {
                    supplierid = supplierOrderItem.supplierid,
                    auctionid = order.Auction.Id,
                    lotid = lotId,
                    minimalprice = minimalPrice
                });
            }
            context.SaveChanges();
        }

        public static ProcuratoryEF ReadProcuratory(int procuratoryId)
        {
            var procuratoryItem = (from procuratory in context.procuratories
                                   where procuratory.id == procuratoryId
                                   select procuratory).FirstOrDefault();

            return procuratoryItem;
        }

        public static ProcuratoryEF ReadProcuratory(int supplierId, int lotId)
        {
            var procuratoryItem = (from procuratory in context.procuratories
                                   where procuratory.supplierid == supplierId && procuratory.lotid == lotId
                                   select procuratory).FirstOrDefault();

            return procuratoryItem;
        }

        public static void UpdateProcuratory(ProcuratoryEF procuratoryItem, decimal newPrice)
        {
            var procuratory = ReadProcuratory((int)procuratoryItem.supplierid, (int)procuratoryItem.lotid);

            procuratory = procuratoryItem;
            procuratory.minimalprice = newPrice;

            context.SaveChanges();
        }


        public static void UpdateProcuratory(int supplierId, int lotId, decimal lastPrice)
        {
            var lot = context.lots.FirstOrDefault(l => l.id == lotId);
            var procuratory = context.procuratories.FirstOrDefault(p => p.auctionid == lot.auctionid && p.lotid == lot.id && p.supplierid == supplierId);

            if (procuratory != null) {
                procuratory.minimalprice = lastPrice;
                context.SaveChanges();
            }
        }


        public static void UpdateProcuratory(string supplierName, string lotCode, string lastPrice)
        {
            var supplier = context.suppliers.FirstOrDefault(s => s.companyid == (context.companies.FirstOrDefault(c => c.name.ToLower().Contains(supplierName.ToLower()))).id);
            var lot = context.lots.FirstOrDefault(l => l.number.ToLower().Contains(lotCode.ToLower()));
            var procuratory = context.procuratories.FirstOrDefault(p => p.auctionid == lot.auctionid && p.lotid == lot.id && p.supplierid == supplier.id);

            if (procuratory != null) {
                procuratory.minimalprice = Convert.ToDecimal(lastPrice.Replace(" ", ""));
                context.SaveChanges();
            }
        }


        public static void UpdateProcuratoriesWithFile(int supplierOrderId, int fileId)
        {
            var supplierOrder = context.supplierorders.FirstOrDefault(so => so.id == supplierOrderId);

            if (supplierOrder != null) {
                var procuratories = context.procuratories.Where(p => p.auctionid == supplierOrder.auctionid && p.supplierid == supplierOrder.supplierid);

                if (procuratories != null && procuratories.Count() > 0) {
                    foreach (var item in procuratories) {
                        item.fileId = fileId;
                    }

                    context.SaveChanges();
                }
            }
        }

        public static void DeleteProcuratory(int procuratoryId)
        {
            var procuratory = ReadProcuratory(procuratoryId);

            context.procuratories.Remove(procuratory);
            context.SaveChanges();
        }
        #endregion

        #region SuppliersJournal
        public static void CreateSuppliersJournal(SuppliersJournalEF suppliersJournalItem)
        {
            context.suppliersjournal.Add(suppliersJournalItem);
            context.SaveChanges();
        }

        public static SuppliersJournalEF ReadSuppliersJournal(int suppliersJournalId)
        {
            var suppliersJournalItem = (from suppliersjournal in context.suppliersjournal
                                        where suppliersjournal.id == suppliersJournalId
                                        select suppliersjournal).FirstOrDefault();

            return suppliersJournalItem;
        }

        public static List<SuppliersJournalEF> ReadSuppliersJournals()
        {
            return context.suppliersjournal.ToList();
        }

        public static List<SuppliersJournalEF> ReadSuppliersJournals(int supplierId)
        {
            return context.suppliersjournal.Where(s => s.supplierid == supplierId).ToList();
        }

        public static List<SuppliersJournalEF> ReadSuppliersJournals(int supplierId, int brokerId)
        {
            var suppliersJournalList = context.suppliersjournal.Where(s => s.supplierid == supplierId && s.brokerid == brokerId);

            if (suppliersJournalList != null) return suppliersJournalList.ToList();
            return null;
        }

        public static List<SuppliersJournalEF> ReadSuppliersJournals(int brokerId, bool single = false)
        {
            var suppliersJournalList = context.suppliersjournal.Where(s => s.brokerid == brokerId);

            if (suppliersJournalList != null) return suppliersJournalList.ToList();
            return null;
        }

        public static void UpdateSuppliersJournal(SuppliersJournalEF suppliersJournalItem)
        {
            var suppliersJournal = ReadSuppliersJournal(suppliersJournalItem.id);

            suppliersJournal = suppliersJournalItem;
            context.SaveChanges();
        }

        public static void DeleteSuppliersJournal(int suppliersJounralId)
        {
            var suppliersJournalItem = ReadSuppliersJournal(suppliersJounralId);

            context.suppliersjournal.Remove(suppliersJournalItem);
            context.SaveChanges();
        }

        public static string GetSupplierCodeByCompany(int companyId)
        {
            var supplierItem = context.suppliers.FirstOrDefault(s => s.companyid == companyId);

            if (supplierItem != null) {
                var customerCode = context.suppliersjournal.FirstOrDefault(s => s.supplierid == supplierItem.id);

                if (customerCode != null) return customerCode.code;
            }

            return null;
        }
        #endregion

        #region RequestedDoc
        public static void CreateRequestedDoc(RequestedDocEF requestedDocItem)
        {
            context.requesteddocs.Add(requestedDocItem);
            context.SaveChanges();
        }

        public static RequestedDocEF ReadRequestedDoc(int requestedDocId)
        {
            var requestedDoc = (from requestedDocs in context.requesteddocs
                                where requestedDocs.id == requestedDocId
                                select requestedDocs).FirstOrDefault();

            return requestedDoc;
        }

        public static List<RequestedDocEF> ReadRequestedDocs(int supplierOrderId)
        {
            var requestedDoc = from requestedDocs in context.requesteddocs
                               where requestedDocs.supplierorderid == supplierOrderId
                               select requestedDocs;

            return requestedDoc.ToList();
        }

        public static void UpdateRequestedDoc(RequestedDocEF requestedDocItem)
        {
            var requestedDoc = ReadRequestedDoc(requestedDocItem.id);

            requestedDoc = requestedDocItem;
            context.SaveChanges();
        }

        public static void DeleteRequestedDoc(int requestedDocId)
        {
            var requestedDoc = ReadRequestedDoc(requestedDocId);

            context.requesteddocs.Remove(requestedDoc);
            context.SaveChanges();
        }
        #endregion

        #region BrokersJournal
        public static BrokersJournalEF GetBrokerCodeC01(int brokerId)
        {
            return context.brokersjournal.FirstOrDefault(b => b.brokerid == brokerId);
        }
        #endregion

        #region RatesList
        public static int CreateRatesList(RatesListEF ratesList)
        {
            context.rateslists.Add(ratesList);
            context.SaveChanges();

            return ratesList.id;
        }

        public static List<RatesListEF> ReadRatesList(int contractId)
        {
            return context.rateslists.Where(r => r.contractid == contractId).ToList();
        }

        public static bool UpdateRatesList(int ratesListId, string name, int siteId)
        {
            var ratesList = context.rateslists.Find(ratesListId);

            if (ratesList == null) return false;

            ratesList.name = name;
            ratesList.siteid = siteId;

            context.SaveChanges();

            return true;
        }

        public static bool DeleteRatesList(int ratesListId)
        {
            var ratesList = context.rateslists.Find(ratesListId);

            if (ratesList == null) return false;

            var rates = context.rates.Where(r => r.rateslistid == ratesListId);

            if (rates != null) {
                context.rates.RemoveRange(rates);
                context.SaveChanges();
            }

            context.rateslists.Remove(ratesList);
            context.SaveChanges();

            return true;
        }
        #endregion

        #region Rates
        public static int CreateRate(RateEF rate)
        {
            context.rates.Add(rate);
            context.SaveChanges();

            return rate.id;
        }

        public static List<RateEF> ReadRates(int ratesListId)
        {
            return context.rates.Where(r => r.rateslistid == ratesListId).ToList();
        }

        public static bool UpdateRate(int rateId, decimal transaction, decimal procent)
        {
            var rate = context.rates.Find(rateId);

            if (rate == null) return false;

            rate.transaction = transaction;
            rate.percent = procent;
            context.SaveChanges();

            return true;
        }

        public static bool DeleteRate(int rateId)
        {
            var rate = context.rates.Find(rateId);

            if (rate == null) return false;

            context.rates.Remove(rate);
            context.SaveChanges();

            return true;
        }
        #endregion

        #region Orders
        public static int CreateOrder(OrderEF orderItem)
        {
            context.orders.Add(orderItem);
            context.SaveChanges();

            return orderItem.id;
        }


        public static OrderEF ReadOrder(int orderId)
        {
            return context.orders.FirstOrDefault(o => o.id == orderId);
        }


        public static void UpdateOrderStatus(OrderEF orderItem, int statusId)
        {
            var orderInfo = context.orders.Find(orderItem.id);
            orderInfo.statusid = statusId;

            context.SaveChanges();
        }


        public static void UpdateOrderStatus(OrderEF orderItem, int statusId, int auctionId)
        {
            var orderInfo = context.orders.Find(orderItem.id);
            orderInfo.auctionid = auctionId;
            orderInfo.statusid = statusId;

            context.SaveChanges();
        }


        public static List<OrderEF> GetOrders(int siteId, int statusId)
        {
            return context.orders.Where(o => o.siteid == siteId && o.statusid == statusId).ToList();
        }
        #endregion

        #region Documents
        public static int CreateDocument(DocumentRequisite documentRequisite, int filesListId)
        {
            var document = new DocumentEF() {
                name = documentRequisite.fileName.Substring(0, documentRequisite.fileName.Length - 5),
                siteid = documentRequisite.market == MarketPlaceEnum.ETS ? 4 : 1,
                extension = documentRequisite.fileName.Substring(documentRequisite.fileName.LastIndexOf(".") + 1),
                documenttypeid = (int)documentRequisite.type,
                fileslistid = filesListId,
                number = documentRequisite.number,
                date = documentRequisite.date,
                filesectionid = (int)documentRequisite.section
            };

            context.documents.Add(document);
            context.SaveChanges();

            return document.id;
        }


        public static int CreateDocument(DocumentEF document)
        {
            context.documents.Add(document);
            context.SaveChanges();

            return document.id;
        }


        public static void CreateDocuments(DocumentEF documentInfo)
        {
            context.documents.Add(documentInfo);
            context.SaveChanges();
        }

        public static DocumentEF ReadDocument(int filesListId, int documentTypeId)
        {
            return context.documents.FirstOrDefault(f => f.fileslistid == filesListId && f.documenttypeid == documentTypeId);
        }

        public static DocumentEF ReadDocument(int filesListId, int documentTypeId, string fileName)
        {
            return context.documents.FirstOrDefault(f => f.fileslistid == filesListId && f.documenttypeid == documentTypeId && f.name == fileName);
        }

        public static List<DocumentEF> ReadDocuments()
        {
            return context.documents.ToList();
        }

        public static List<DocumentEF> ReadDocuments(int fileListId, int documentTypeId)
        {
            return context.documents.Where(f => f.fileslistid == fileListId && f.documenttypeid == documentTypeId).ToList();
        }

        public static void UpdateFileSection(int filesListId, DocumentTypeEnum documentType, DocumentSectionEnum documentSection, string auctionNumber, DateTime auctionDate)
        {
            var file = context.documents.FirstOrDefault(f => f.fileslistid == filesListId && f.documenttypeid == (int)documentType);

            if (file != null) {
                file.number = auctionNumber;
                file.filesectionid = (int)documentSection;
                file.date = auctionDate;
                context.SaveChanges();
            }
        }

        public static void UpdateFileSection(int fileId, DocumentSectionEnum documentSection, string auctionNumber, DateTime auctionDate, DocumentTypeEnum docType = DocumentTypeEnum.Scheme)
        {
            var file = context.documents.FirstOrDefault(f => f.id == fileId && f.documenttypeid == (int)docType);

            if (file != null) {
                file.number = auctionNumber;
                file.filesectionid = (int)documentSection;
                file.date = auctionDate;
                context.SaveChanges();
            }
        }

        public static void DeleteDocument(int id)
        {
            var file = context.documents.FirstOrDefault(f => f.id == id);

            if (file != null) {
                context.documents.Remove(file);
                context.SaveChanges();
            }
        }
        #endregion

        #region FileList
        public static int CreateFileList(FilesListEF fileListInfo)
        {
            context.fileslists.Add(fileListInfo);

            context.SaveChanges();

            return fileListInfo.id;
        }
        #endregion

        #region ClearingCountings
        public static int CreateClearingCounting(ClearingCountingEF clearingCounting)
        {
            // Check for exist
            var clearingcount = context.clearingcountings.FirstOrDefault(c => c.lotid == clearingCounting.lotid && c.brokerid == clearingCounting.brokerid &&
            c.fromsupplierid == clearingCounting.fromsupplierid && c.tosupplierid == clearingCounting.tosupplierid);

            // Update old or create new
            if (clearingcount == null) context.clearingcountings.Add(clearingCounting);
            else {
                clearingcount.transaction = clearingCounting.transaction;
                clearingCounting = clearingcount;
            }

            context.SaveChanges();

            return clearingCounting.id;
        }

        public static List<ClearingCountingEF> ReadClearingCountings()
        {
            return context.clearingcountings.ToList();
        }

        public static ClearingCountingEF ReadClearingCounting(int brokerId, int fromSupplierId, int toSupplierId, int lotId)
        {
            return context.clearingcountings.FirstOrDefault(c => c.brokerid == brokerId && c.fromsupplierid == fromSupplierId && c.tosupplierid == toSupplierId && c.lotid == lotId);
        }

        public static List<ClearingCountingEF> ReadClearingCountings(DateTime fromDate, DateTime toDate, int statusId, int brokerId)
        {
            return context.clearingcountings.Where(c => c.createdate >= fromDate && c.createdate <= toDate && c.statusid == statusId && c.brokerid == brokerId).ToList();
        }

        public static List<ClearingCountingEF> ReadClearingCountings(DateTime fromDate, DateTime toDate, int statusId, int brokerId, int companyId)
        {
            var supplier = context.suppliers.FirstOrDefault(s => s.companyid == companyId);

            if (supplier != null) {
                if (statusId == 9) return context.clearingcountings.Where(c => c.createdate >= fromDate && c.createdate <= toDate && c.statusid == statusId && c.brokerid == brokerId && c.tosupplierid == supplier.id).ToList();
                else return context.clearingcountings.Where(c => c.createdate >= fromDate && c.createdate <= toDate && c.statusid == statusId && c.brokerid == brokerId && c.fromsupplierid == supplier.id).ToList();
            }

            return new List<ClearingCountingEF>();
        }

        public static bool UpdateClearingCounting(int id, decimal transaction)
        {
            var clearingCounting = context.clearingcountings.FirstOrDefault(c => c.id == id);

            if (clearingCounting != null) {
                clearingCounting.transaction = transaction;

                context.SaveChanges();
                return true;
            }

            return false;
        }

        public static void UpdateClearingCountStatus(int fromSup, int toSup, int lotId, int statusId = 10)
        {
            var count = context.clearingcountings.FirstOrDefault(c => c.fromsupplierid == fromSup && c.tosupplierid == toSup && c.lotid == lotId);

            if (count != null) count.statusid = statusId;

            context.SaveChanges();
        }
        #endregion

        #region OtherDocs
        public static int CreateOtherDoc(OtherDocsEF otherDoc)
        {
            context.otherdocs.Add(otherDoc);
            context.SaveChanges();

            return otherDoc.id;
        }

        public static void UpdateOtherDoc(OtherDocsEF otherDoc)
        {
            var otherDocItem = context.otherdocs.FirstOrDefault(o => o.id == otherDoc.id);

            otherDocItem = otherDoc;

            context.SaveChanges();
        }

        public static OtherDocsEF ReadOtherDoc(int id)
        {
            return context.otherdocs.FirstOrDefault(o => o.id == id);
        }

        public static List<OtherDocsEF> ReadOtherDocs(int companyId, DateTime dateStart, DateTime dateEnd)
        {
            return context.otherdocs.Where(o => o.companyid == companyId && o.createdate >= dateStart && o.createdate <= dateEnd).ToList();
        }

        public static void DeleteOtherDoc(int otherdocId)
        {
            var otherDoc = context.otherdocs.FirstOrDefault(o => o.id == otherdocId);

            if (otherDoc != null) {
                context.otherdocs.Remove(otherDoc);
                context.SaveChanges();
            }
        }
        #endregion

        #region DocumentTypes
        public static List<DocumentTypeEF> ReadDocumentTypes()
        {
            return context.documenttypes.ToList();
        }

        public static List<DocumentTypeEF> ReadDocumentTypes(int from, int to, int special)
        {
            return context.documenttypes.Where(d => d.id >= from && d.id <= to || d.id == special).ToList();
        }
        #endregion

        #region ListServ
        public static int CreateListServ(ListServEF listServ)
        {
            context.listservs.Add(listServ);
            context.SaveChanges();

            return listServ.id;
        }

        public static ListServEF ReadListServ(int id)
        {
            return context.listservs.FirstOrDefault(l => l.id == id);
        }

        public static List<ListServEF> ReadListServ()
        {
            return context.listservs.ToList();
        }

        public static List<ListServView> ReadListServ(DateTime startDate, DateTime endDate)
        {
            return context.listservsview.AsNoTracking().Where(l => l.createdate >= startDate && l.createdate <= endDate).ToList();
        }

        public static List<ListServView> ReadListServ(DateTime startDate, DateTime endDate, int listServNumber)
        {

            return context.listservsview.AsNoTracking().Where(l => l.createdate >= startDate && l.createdate <= endDate && l.number == listServNumber).ToList();
        }

        public static ListServEF ReadListServ(int brokerId, int statusId)
        {

            return context.listservs.FirstOrDefault(l => l.brokerid == brokerId && l.statusid == statusId);
        }

        public static void UpdateListServ(int listServId, int statusId, DateTime? departureDate = null)
        {
            var listServ = context.listservs.FirstOrDefault(l => l.id == listServId);

            if (listServ != null) {
                listServ.statusid = statusId;

                if (departureDate != null) listServ.departuredate = departureDate;

                context.SaveChanges();
            }
        }
        #endregion

        #region Envelop
        public static int CreateEnvelop(EnvelopEF envelop)
        {
            context.envelops.Add(envelop);
            context.SaveChanges();

            return envelop.id;
        }

        public static List<EnvelopsView> ReadEnvelopsList(int listServId)
        {
            var envelopView = context.envelopsview.AsNoTracking().Where(e => e.listservid == listServId).OrderBy(e => e.id).ToList();

            if (envelopView != null && envelopView.Count > 0) {
                int iCount = 1;
                foreach (var item in envelopView) {
                    item.serialnumber = iCount;
                    iCount++;
                }
            }

            return envelopView;
        }

        public static EnvelopEF ReadEnvelop(int listServId, int companyId)
        {
            return context.envelops.FirstOrDefault(e => e.listservid == listServId && e.companyid == companyId);
        }

        public static bool UpdateEnvelop(int id, string code)
        {
            var envelop = context.envelops.FirstOrDefault(e => e.id == id);

            if (envelop != null) {
                envelop.code = code;
                context.SaveChanges();

                return true;
            }

            return false;
        }

        public static bool DeleteEnvelop(int id)
        {
            var envelop = context.envelops.FirstOrDefault(e => e.id == id);

            if (envelop != null) {
                context.envelops.Remove(envelop);
                context.SaveChanges();

                return true;
            } else return false;
        }
        #endregion

        #region EnvelopContent
        public static int CreateEnvelopContent(EnvelopContentEF envelopContent)
        {
            context.envelopcontents.Add(envelopContent);
            context.SaveChanges();

            return envelopContent.id;
        }

        public static List<EnvelopContentEF> ReadEnvelopContentList()
        {
            return context.envelopcontents.ToList();
        }

        public static List<EnvelopContentEF> ReadEnvelopContentList(int envelopId)
        {
            return context.envelopcontents.Where(e => e.envelopid == envelopId).OrderBy(e => e.id).ToList();
        }

        public static bool DeleteEnvelopContent(int otherDocId)
        {
            var envelopContent = context.envelopcontents.FirstOrDefault(e => e.otherdocid == otherDocId);

            if (envelopContent != null) {
                context.envelopcontents.Remove(envelopContent);

                var otherDoc = context.otherdocs.FirstOrDefault(o => o.id == otherDocId);

                otherDoc.inpost = false;
                otherDoc.listservnumber = 0;

                context.SaveChanges();

                return true;
            }

            return false;
        }
        #endregion

        #region FinalReport
        public static int CreateFinalReport(FinalReportEF finalReportInfo)
        {
            context.finalreports.Add(finalReportInfo);
            context.SaveChanges();

            return finalReportInfo.id;
        }

        public static List<FinalReportEF> ReadFinalReports()
        {
            return context.finalreports.ToList();
        }

        public static FinalReportEF ReadFinalReport(int auctionId, int lotId)
        {
            return context.finalreports.FirstOrDefault(f => f.auctionId == auctionId && f.lotId == lotId);
        }

        public static void DeleteFinalReport(FinalReportEF finalReportInfo)
        {
            context.finalreports.Remove(finalReportInfo);
            context.SaveChanges();
        }
        #endregion

        #region TechSpecReports
        public static List<TechSpecReportBO> ReadTechSpecReport(int customerId, DateTime startDate, DateTime endDate, List<int> siteId, List<int> statusId)
        {
            return context.GetTechSpecReport(customerId, startDate, endDate, siteId, statusId);
        }
        #endregion

        #region Qualifications
        public static int CreateQualification(QualificationEF qualificationInfo)
        {
            context.qualifications.Add(qualificationInfo);
            context.SaveChanges();

            return qualificationInfo.id;
        }

        public static bool UpdateQualification(QualificationEF qualificationInfo)
        {
            var qualification = context.qualifications.FirstOrDefault(q => q.id == qualificationInfo.id);

            if (qualification != null) {
                qualification = qualificationInfo;

                context.SaveChanges();

                return true;
            }

            return false;
        }

        public static List<QualificationEF> ReadQualifications(int auctionId)
        {
            return context.qualifications.Where(q => q.auctionId == auctionId).ToList();
        }

        public static bool DeleteQualification(int id)
        {
            var qualification = context.qualifications.FirstOrDefault(q => q.id == id);

            if (qualification != null) {
                context.qualifications.Remove(qualification);
                context.SaveChanges();

                return true;
            }

            return false;
        }
        #endregion

        #region KarazhiraCustomer
        public static void SetNewKarazhiraOrder(Order order)
        {
            // Default parametrs for new auction
            var auctionInfo = new AuctionEF();
            auctionInfo.date = DateTime.Now;
            auctionInfo.sectionid = 1;
            auctionInfo.typeid = 1;
            auctionInfo.number = "New";
            auctionInfo.statusid = 1;
            auctionInfo.comments = order.Auction.Comments;
            auctionInfo.ndsincluded = true;
            auctionInfo.signstatusid = false;
            auctionInfo.published = false;
            auctionInfo.ownerid = order.Auction.OwnerId > 0 ? order.Auction.OwnerId : 1;
            auctionInfo.siteid = 5;
            auctionInfo.traderid = 3;
            auctionInfo.customerid = 2;
            auctionInfo.brokerid = 1;


            var lotInfo = new LotEF();
            lotInfo.number = "";
            lotInfo.description = order.Auction.Lots[0].Name;
            lotInfo.unitid = order.Auction.Lots[0].UnitId;
            lotInfo.amount = order.Auction.Lots[0].Quantity;
            lotInfo.price = order.Auction.Lots[0].Price;
            lotInfo.sum = order.Auction.Lots[0].Sum;
            lotInfo.paymentterm = order.Auction.Lots[0].PaymentTerm;
            lotInfo.deliverytime = order.Auction.Lots[0].DeliveryTime;
            lotInfo.deliveryplace = order.Auction.Lots[0].DeliveryPlace;
            lotInfo.dks = 0;
            lotInfo.contractnumber = "";
            lotInfo.step = (double)order.Auction.Lots[0].Step;
            lotInfo.warranty = (double)order.Auction.Lots[0].Warranty;
            lotInfo.localcontent = (int)order.Auction.Lots[0].LocalContent;

            CreateNewUTBOrder(auctionInfo, lotInfo);
        }


        public static void CreateNewUTBOrder(AuctionEF auctionInfo, LotEF lotInfo)
        {
            context.regulations.Add(new RegulationEF {
                opendate = DateTime.Now,
                closedate = DateTime.Now,
                applydate = DateTime.Now,
                applydeadline = DateTime.Now,
                applicantsdeadline = DateTime.Now,
                provisiondeadline = DateTime.Now
            });
            context.SaveChanges();

            var regulationId = (from regulation in context.regulations
                                select regulation).OrderByDescending(i => i.id).FirstOrDefault();

            auctionInfo.regulationid = regulationId.id;
            context.auctions.Add(auctionInfo);
            context.SaveChanges();

            var auctionId = (from auction in context.auctions
                             select auction).OrderByDescending(i => i.id).FirstOrDefault();

            lotInfo.auctionid = auctionId.id;
            context.lots.Add(lotInfo);
            context.SaveChanges();
        }


        public static Order GetKarazhiraAuction(int auctionId)
        {
            Order order = new Order();
            order.Auction = new Auction();
            order.Auction.Lots = new System.Collections.ObjectModel.ObservableCollection<Lot>();
            order.Auction.SupplierOrders = new System.Collections.ObjectModel.ObservableCollection<SupplierOrder>();

            var auction = context.auctions.FirstOrDefault(a => a.id == auctionId);

            if (auction != null) {
                // General
                order.Date = auction.regulation.opendate;
                order.Auction.Date = auction.date;
                order.Auction.Number = auction.number;
                order.Auction.StatusId = auction.statusid;

                var lot = context.lots.FirstOrDefault(l => l.auctionid == auctionId);

                if (lot != null) {
                    // Lot
                    order.Auction.Lots.Add(new Lot() {
                        Name = lot.description,
                        Unit = lot.unit.name,
                        Quantity = lot.amount,
                        Price = lot.price,
                        Sum = lot.sum,
                        PaymentTerm = lot.paymentterm,
                        DeliveryTime = lot.deliverytime,
                        DeliveryPlace = lot.deliveryplace
                    });

                    var supplierOrder = ReadSupplierOrders(auctionId);//context.supplierorders.Where(s => s.auctionid == auctionId);

                    if (supplierOrder != null && supplierOrder.Count() > 0) {
                        // Suppliers
                        foreach (var item in supplierOrder) {
                            var minPrice = context.procuratories.FirstOrDefault(p => p.auctionid == auctionId && p.supplierid == item.supplierid);

                            order.Auction.SupplierOrders.Add(new SupplierOrder() {
                                Name = item.supplier.company.name,
                                BrokerName = item.contract != null ? item.contract.broker.name : "",
                                MinimalPrice = minPrice != null ? minPrice.minimalprice : 0
                            });
                        }

                        // Winner
                        // return order;
                    }
                }
            }

            return order;
        }


        public static List<Order> GetKarazhiraAuctions(int status)
        {
            List<Order> orders = new List<Order>();

            if (status == 4 || status == 1 || status == 3) { // New & waiting
                var lotsList = ReadLots(2, 1, status);

                foreach (var item in lotsList) {
                    orders.Add(new Order {
                        Date = item.auction.regulation.opendate,
                        Auction = new Auction() {
                            Id = item.auctionid,
                            Date = item.auction.date,
                            ApplicantsDeadline = item.auction.regulation.applicantsdeadline,
                            Number = item.auction.number,
                            Status = item.auction.status.name,
                            StatusId = item.auction.statusid,
                            Lots = new System.Collections.ObjectModel.ObservableCollection<Lot>() { new Lot {
                            Name = item.description,
                            Sum = item.amount * item.price
                        } }
                        }
                    });
                }
            } else if (status == 2) {
                var items = (from lot in context.lots
                             where lot.auction.statusid == 2 && lot.auction.siteid == 1 && lot.auction.customerid == 2
                             join procuratory in context.procuratories on lot.id equals procuratory.lotid
                             select new {
                                 AuctionId = lot.auctionid,
                                 AuctionDate = lot.auction.date,
                                 OrderDate = lot.auction.regulation.opendate,
                                 AuctionNumber = lot.auction.number,
                                 AuctionStatus = lot.auction.status.name,
                                 AuctionStatusId = lot.auction.status.id,
                                 LotName = lot.description,
                                 StartPrice = lot.sum,
                                 WinningPrice = procuratory.minimalprice,
                                 Winner = procuratory.supplier.company.name
                             });

                foreach (var item in items.ToList().OrderBy(x => x.AuctionId)) {
                    if (!item.Winner.ToUpper().Contains("АЛТЫН")) {
                        orders.Add(new Order {
                            Date = item.OrderDate,
                            Auction = new Auction() {
                                Id = item.AuctionId,
                                Date = item.AuctionDate,
                                Number = item.AuctionNumber,
                                Status = item.AuctionStatus,
                                StatusId = item.AuctionStatusId,
                                Lots = new System.Collections.ObjectModel.ObservableCollection<Lot>() { new Lot {
                                Name = item.LotName,
                                Sum = item.StartPrice
                            } },
                                Procuratories = new System.Collections.ObjectModel.ObservableCollection<Procuratory>() { new Procuratory {
                                MinimalPrice=item.WinningPrice,
                                SupplierName=item.Winner
                            } }
                            }
                        });
                    }
                }
            }

            return orders;
        }


        public static List<Supplier> GetKarazhiraSuppliers()
        {
            var suppliersList = (from supplierOrder in context.supplierorders
                                 join auction in context.auctions on supplierOrder.auctionid equals auction.id
                                 where auction.customerid == 2
                                 select new {
                                     Name = supplierOrder.supplier.company.name,
                                     Bin = supplierOrder.supplier.company.bin,
                                     Address = supplierOrder.supplier.company.addresslegal,
                                     Telephone = supplierOrder.supplier.company.telephone,
                                     Email = supplierOrder.supplier.company.email,
                                     Country = supplierOrder.supplier.company.countries.name
                                 }).Distinct().ToList();

            List<Supplier> suppliers = new List<Supplier>();

            foreach (var item in suppliersList) {
                suppliers.Add(new Supplier {
                    Name = item.Name,
                    BIN = item.Bin,
                    Address = item.Address,
                    Contacts = item.Telephone + ", " + item.Email,
                    Country = item.Country
                });
            }

            return suppliers;
        }


        public static int[] GetKarazhiraWaitingEndedStatistic()
        {
            int[] array = new int[3];

            array[0] = context.auctions.Where(a => a.customerid == 2 && a.siteid == 1 && a.statusid == 4).Count();
            array[1] = context.auctions.Where(a => a.customerid == 2 && a.siteid == 1 && a.statusid == 2).Count();
            array[2] = context.auctions.Where(a => a.customerid == 2 && a.siteid == 1 && a.statusid == 3).Count();

            return array;
        }


        public static List<Unit> GetUnits()
        {
            List<Unit> units = new List<Unit>();

            foreach (var item in context.units.ToList()) {
                units.Add(new Unit {
                    Id = item.id,
                    Name = item.name,
                    Description = item.description
                });
            }

            return units;
        }
        #endregion

        #region ETS
        public static void CreateNewAuction(Order order)
        {
            context.regulations.Add(new RegulationEF {
                opendate = order.Date,
                closedate = order.Auction.Date,
                applydeadline = order.Deadline,
                applicantsdeadline = order.Auction.ApplicantsDeadline,
                provisiondeadline = order.Auction.ExchangeProvisionDeadline
            });
            context.SaveChanges();

            var regulationId = (from regulation in context.regulations
                                select regulation).OrderByDescending(i => i.id).FirstOrDefault();

            var auctionNumber = order.Title;

            if (auctionNumber.Contains("от")) auctionNumber = auctionNumber.Substring(0, auctionNumber.IndexOf("от") - 1);

            context.auctions.Add(new AuctionEF {
                date = order.Auction.Date,
                sectionid = 1, // Test
                typeid = 1, // Test                
                number = auctionNumber,
                statusid = 4, // Waiting
                              // TODO : comments
                              // TODO : ndsincluded
                              // TODO : signstatusid
                published = false,
                ownerid = 1, //Test                
                siteid = 4, // ETS
                regulationid = regulationId.id,
                traderid = 1, // Test
                customerid = 1, // Vostok
                brokerid = 3 // Altair Nur
            });
            context.SaveChanges();
        }


        public static void CreateSupplierCode(FormC01 formC01)
        {
            var supplierId = (from supplier in context.suppliers
                              join company in context.companies on supplier.companyid equals company.id
                              where company.bin == formC01.bin
                              select supplier).FirstOrDefault();

            var brokerId = (from brokersjournal in context.brokersjournal
                            where brokersjournal.code == formC01.broker.code
                            select brokersjournal).FirstOrDefault();

            context.suppliersjournal.Add(new SuppliersJournalEF {
                supplierid = supplierId.id,
                brokerid = brokerId.brokerid,
                code = formC01.code,
                regdate = DateTime.Now
            });
            context.SaveChanges();
        }


        //public static void CreateSupplierOrder(EntryOrder entryOrder) {
        //    var supplierId = (from supplier in context.suppliers
        //                      join company in context.companies on supplier.companyid equals company.id
        //                      where company.bin == entryOrder.clientBIN
        //                      select supplier).FirstOrDefault();

        //    var auctionId = (from lot in context.lots
        //                     where lot.number == entryOrder.lotNumber
        //                     select lot).FirstOrDefault();

        //    var contractId = (from contract in context.contracts
        //                      join brokersjournal in context.brokersjournal on contract.brokerid equals brokersjournal.brokerid
        //                      where contract.companyid == supplierId.companyid
        //                      select contract).Last();

        //    context.supplierorders.Add(new SupplierOrderEF {
        //        supplierid = supplierId.id,
        //        auctionid = auctionId.auctionid,
        //        contractid = contractId.id,
        //        date = DateTime.Now
        //    });
        //    context.SaveChanges();
        //}


        //public static void CreateApplicants(WaitingList waitingList) {
        //    var auctionId = context.lots.Where(l=>l.number==waitingList.lotCode).FirstOrDefault();

        //    foreach(var item in waitingList.waitingListTable) {
        //        var supplierCode = item.company.Split(' ');

        //        var supplierOrderId = (from supplierorder in context.supplierorders
        //                               join suppliersjournal in context.suppliersjournal on supplierorder.supplierid equals suppliersjournal.supplierid
        //                               where supplierorder.auctionid == auctionId.auctionid && suppliersjournal.code == supplierCode[2]
        //                               select supplierorder).FirstOrDefault();

        //        context.applicants.Add(new ApplicantEF {
        //            auctionid = auctionId.auctionid,
        //            supplierorderid = supplierOrderId.id
        //        });
        //        context.SaveChanges();
        //    }
        //}


        public static void AddProcuratory(StockDealInfo stockDealInfo)
        {
            var ids = context.lots.Where(l => l.number == stockDealInfo.LotCode).FirstOrDefault();

            context.procuratories.Add(new ProcuratoryEF {
                supplierid = context.suppliersjournal.Where(s => s.code == stockDealInfo.SupplierCode).FirstOrDefault().id,
                auctionid = ids.auctionid,
                lotid = ids.id
            });
            context.SaveChanges();
        }
        #endregion

        #region Karazhira
        public static int ReadNewKarazhiraOrders()
        {
            return context.auctions.Where(a => a.statusid == 1 & a.siteid == 1 & a.customerid == 2).Count();
        }


        public static LotEF UpdateUTBNewOrder(Order order, int auctionId, int traderId)
        {
            var newAuction = context.auctions.Where(a => a.id == auctionId).FirstOrDefault();

            var auctionNumber = order.Title;

            if (auctionNumber.Contains("от")) auctionNumber = auctionNumber.Substring(0, auctionNumber.IndexOf("от") - 1);

            newAuction.date = order.Auction.Date;
            newAuction.number = auctionNumber;
            newAuction.statusid = 4;
            newAuction.traderid = traderId;

            var regulationRecord = context.regulations.Where(r => r.id == newAuction.regulationid).FirstOrDefault();

            regulationRecord.closedate = order.Auction.Date;
            regulationRecord.applydeadline = order.Deadline;
            regulationRecord.applicantsdeadline = order.Auction.ApplicantsDeadline;
            regulationRecord.provisiondeadline = order.Auction.ExchangeProvisionDeadline;

            context.SaveChanges();

            return context.lots.Where(l => l.auctionid == newAuction.id).FirstOrDefault();
        }


        public static List<AuctionEF> ReadAuctionsForUtb(int status)
        {
            return context.auctions.Where(a => a.statusid == status && a.siteid == 1).ToList();
        }
        #endregion

        #region Intermediary
        public static UnitEF ReadUnitInfo(int id)
        {
            return context.units.Where(u => u.id == id).FirstOrDefault();
        }


        public static List<BrokerEF> ReadBrokers()
        {
            return context.brokers.ToList();
        }


        public static List<SupplierEF> ReadSuppliers(bool withContract = false)
        {
            if (withContract) return context.suppliers.Where(s => context.contracts.Any(c => c.companyid == s.companyid)).ToList();
            else return context.suppliers.ToList();
        }


        public static List<SupplierEF> ReadSuppliers(int brokerId)
        {
            return context.suppliers.Where(s => context.contracts.Any(c => c.brokerid == brokerId && c.companyid == s.companyid)).ToList();
        }


        public static List<TraderEF> ReadTraders()
        {
            return context.traders.ToList();
        }


        public static List<LotEF> ReadLotsInNewAuction()
        {
            var lotLst = from lot in context.lots
                         join auction in context.auctions on lot.auctionid equals auction.id
                         where auction.statusid == 1
                         select lot;

            return lotLst.ToList();
        }

        public static List<ProcuratoryEF> ReadProcuratories()
        {
            return context.procuratories.ToList();
        }

        public static List<ProcuratoryEF> ReadProcuratories(int lotId, bool forSO = true)
        {
            return context.procuratories.Where(p => p.lotid == lotId).ToList();
        }

        public static List<ProcuratoryEF> ReadProcuratories(DateTime formateDate)
        {
            return context.procuratories.Where(p => p.auction.siteid == 4 && p.auction.date == formateDate && p.supplierid != 3 && p.supplierid != 27 && p.supplierid != 354 && p.supplierid != 384 && p.supplierid != 385).ToList();
        }

        public static List<ProcuratoryEF> ReadProcuratories(int auctionId)
        {
            return context.procuratories.Where(p => p.auctionid == auctionId).ToList();
        }


        public static List<ProcuratoryEF> ReadProcuratories(int supplierId, int auctionId)
        {
            return context.procuratories.Where(p => p.auctionid == auctionId && p.supplierid == supplierId).ToList();
        }


        public static int GetProcuratoriesCount(int supplierId, int lotId)
        {
            return context.procuratories.Where(p => p.lotid == lotId && p.supplierid == supplierId).Count();
        }


        public static ContractEF ReadContract(SupplierEF supplier, BrokerEF broker)
        {
            return context.contracts.Where(c => c.companyid == supplier.companyid && c.brokerid == broker.id).LastOrDefault();
        }


        public static List<SectionEF> ReadSections()
        {
            return context.sections.ToList();
        }


        public static List<TypeEF> ReadTypes()
        {
            return context.types.ToList();
        }


        public static List<StatusEF> ReadStatuses()
        {
            return context.statuses.ToList();
        }


        public static List<SiteEF> ReadSites()
        {
            return context.sites.ToList();
        }


        public static List<CustomerEF> ReadCustomers()
        {
            return context.customers.ToList();
        }


        public static int CreateNextSerialNumber(int id)
        {
            var serialNumber = context.serialnumbers.Where(s => s.id == id).FirstOrDefault();

            serialNumber.number += 1;
            context.SaveChanges();

            return serialNumber.number;
        }


        public static int ReadSerialNumber(int id)
        {
            return context.serialnumbers.Where(s => s.id == id).FirstOrDefault().number;
        }
        #endregion

        #region User
        public static User GetUserByLogin(string login)
        {
            return context.GetUser(login);
        }
        #endregion
    }
}