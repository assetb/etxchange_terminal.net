using System;
using AltaTransport._1c.InvoicePaiment;
using AltaBO;
using System.Collections.Generic;
using AltaLog;

namespace AltaTransport
{
    public class _1CTransport
    {
        private static InvoicePaymentConnectionProtocol altaConn = new InvoicePaymentConnectionProtocol(1);
        private static InvoicePaymentConnectionProtocol kordConn = new InvoicePaymentConnectionProtocol(2);
        private static InvoicePaymentConnectionProtocol akalConn = new InvoicePaymentConnectionProtocol(3);
        private static InvoicePaymentConnectionProtocol baseConn;
        private static ResponseData responseData, rd3;


        private static void ChooseBaseType(int baseType)
        {
            switch (baseType)
            {
                case 1:
                    baseConn = altaConn;
                    break;
                case 2:
                    baseConn = kordConn;
                    break;
                case 3:
                    baseConn = akalConn;
                    break;
            }
        }


        public static string CreateCompany(int baseType, Clients clients)
        {
            AppJournal.Write("1C Transport", "Create company", true);

            ChooseBaseType(baseType);

            try
            {
                responseData = baseConn.writeClient(clients, true);
            }
            catch (Exception) { }

            if (responseData.RequestSuccess) return "ok";
            else
            {
                AppJournal.Write("1C Transport", "Company creation error :" + responseData.ErrorMsg, true);
                return responseData.ErrorMsg;
            }
        }


        public static Clients SearchCompany(String bin, int baseType)
        {
            AppJournal.Write("1C Transport", "Search company", true);

            ChooseBaseType(baseType);

            var clientSearch = new ClientSearch();
            clientSearch.IIN = bin;

            try
            {
                responseData = baseConn.readClient(clientSearch);
            }
            catch (Exception) { }

            if (responseData.RequestSuccess)
                return (responseData.ResponseObject as Clients);
            else
            {
                AppJournal.Write("1C Transport", "Search company error :" + responseData.ErrorMsg, true);
                return null;
            }
        }


        public static string CreateContract(int baseType, Contracts contract)
        {
            AppJournal.Write("1C Transport", "Create contract", true);

            ChooseBaseType(baseType);

            try
            {
                responseData = baseConn.writeContract(contract, false);
            }
            catch (Exception) { }

            if (responseData.RequestSuccess) return "ok";
            else
            {
                AppJournal.Write("1C Transport", "Create contract error :" + responseData.ErrorMsg, true);
                return responseData.ErrorMsg;
            }
        }


        public static Contracts SearchContract(String clientIIN, String contractNumber, int baseType)
        {
            AppJournal.Write("1C Transport", "Search contract", true);

            ChooseBaseType(baseType);

            var contractSearch = new ContractSearch();
            contractSearch.ClientIIN = clientIIN;
            contractSearch.ContractNumber = contractNumber;

            try
            {
                responseData = baseConn.readContract(contractSearch);
            }
            catch (Exception) { }

            if (responseData.RequestSuccess)
                return (responseData.ResponseObject as Contracts);
            else
            {
                AppJournal.Write("1C Transport", "Search contract error :" + responseData.ErrorMsg, true);
                return null;
            }
        }


        public static string CreateBankAccount(string accountNumber, string clientIIN, string currencyCode, int baseType, string bankBik)
        {
            AppJournal.Write("1C Transport", "Create bank account", true);

            ChooseBaseType(baseType);

            BankAccounts bankAccount = new BankAccounts();

            bankAccount.ClientIIN = clientIIN;//.PadLeft(12, '0');
            bankAccount.CtlgName = accountNumber;
            bankAccount.CtlgCode = accountNumber;
            bankAccount.BankBIК = bankBik;
            bankAccount.BankCode = bankBik;
            bankAccount.AccountNumber = accountNumber;
            bankAccount.AccountType = "Расчетный";
            bankAccount.Сurrency = currencyCode;

            try
            {
                rd3 = baseConn.writeBankAccount(bankAccount, false);
            }
            catch (Exception ex) { AppJournal.Write("1C Transport", "Write bank account error exception:" + ex.ToString(), true); }

            if (rd3 != null && rd3.RequestSuccess) return "success";
            else
            {
                AppJournal.Write("1C Transport", "Create bank account error :" + (rd3 != null ? rd3.ErrorMsg : "error"), true);
                return null;
            }
        }


        public static BankAccounts SearchBankAccount(String accountNumber, String organizationIIN, int baseType)
        {
            AppJournal.Write("1C Transport", "Search bank account", true);

            ChooseBaseType(baseType);

            var bankAccountSearch = new BankAccountSearch();
            bankAccountSearch.AccountNumber = accountNumber;
            bankAccountSearch.ClientIIN = organizationIIN;

            try
            {
                responseData = baseConn.readBankAccount(bankAccountSearch);
            }
            catch (Exception) { }

            if (responseData.RequestSuccess)
                return (responseData.ResponseObject as BankAccounts);
            else
            {
                AppJournal.Write("1C Transport", "Search bank account error :" + responseData.ErrorMsg, true);
                return null;
            }
        }


        public static String CreateInvoice(int brokerType, bool isWarranty, Order order, bool onlyPlay=false)
        {
            AppJournal.Write("1C Transport", "Create invoice", true);

            ChooseBaseType(brokerType);

            string companyBin = order.Auction.SupplierOrders[0].BIN;
            string contractNumber = order.Auction.SupplierOrders[0].ContractNum;

            if (contractNumber.ToLower().Contains("с") || contractNumber.ToLower().Contains("c"))
            {
                // Check for contract exist (russian & english letter conflict)
                contractNumber = contractNumber.Replace("с", "c"); // Eng->rus

                if (SearchContract(companyBin, contractNumber, brokerType) == null)
                {
                    contractNumber = contractNumber.Replace("c", "с"); // Rus->eng

                    if (SearchContract(companyBin, contractNumber, brokerType) == null)
                    {
                        companyBin = companyBin.PadLeft(12, '0');
                        contractNumber = contractNumber.Replace("с", "c"); // Eng->rus

                        if (SearchContract(companyBin, contractNumber, brokerType) == null)
                        {
                            contractNumber = contractNumber.Replace("c", "с"); // Rus->eng

                            if (SearchContract(companyBin, contractNumber, brokerType) == null) return "Err: Договора не существует";
                        }
                    }
                }
            }
            else
            {
                if (SearchContract(companyBin, contractNumber, brokerType) == null)
                {
                    companyBin = companyBin.PadLeft(12, '0');

                    if (SearchContract(companyBin, contractNumber, brokerType) == null) return "Err: Договора не существует";
                }
            }

            var invoiceEx = new InvoiceEx();

            invoiceEx.IncludeVAT = true;

            if (brokerType == 3) invoiceEx.IncludeVAT = false;

            invoiceEx.DocNum = "";
            invoiceEx.DocDate = isWarranty ? DateTime.Now.Date : order.Auction.Date.Date;
            
            invoiceEx.Comments = ".NET Автоматизация (" + order.Auction.Trader + ")";

            invoiceEx.Client = new Clients();
            invoiceEx.Client.IIN = companyBin;

            invoiceEx.Contract = new Contracts();
            invoiceEx.Contract.ContractNumber = order.Auction.SupplierOrders[0].ContractNum;
            invoiceEx.Contract.ClientIIN = companyBin;
            invoiceEx.Contract.CtlgCode = "";

            invoiceEx.OrganizationBankAccount = new BankAccounts();
            invoiceEx.OrganizationBankAccount.CtlgCode = "";
            invoiceEx.OrganizationBankAccount.BankCode = "";
            invoiceEx.OrganizationBankAccount.OrganizationtIIN = order.Auction.SupplierOrders[0].BrokerBIN;

            switch (order.Auction.SupplierOrders[0].CurrencyCode)
            {
                case "KZT":
                    invoiceEx.OrganizationBankAccount.AccountNumber = order.Auction.SupplierOrders[0].BrokerIIK;
                    break;
                case "RUB":
                    invoiceEx.OrganizationBankAccount.AccountNumber = brokerType == 1 ? order.Auction.SupplierOrders[0].BrokerIIK : brokerType == 2 ? "KZ739261802171874002" : order.Auction.SupplierOrders[0].BrokerIIK;
                    break;
                case "USD":
                    invoiceEx.OrganizationBankAccount.AccountNumber = brokerType == 1 ? order.Auction.SupplierOrders[0].BrokerIIK : brokerType == 2 ? "KZ039261802171874001" : "KZ889470840991144845";
                    break;
                case "EUR":
                    invoiceEx.OrganizationBankAccount.AccountNumber = brokerType == 1 ? order.Auction.SupplierOrders[0].BrokerIIK : brokerType == 2 ? order.Auction.SupplierOrders[0].BrokerIIK : "KZ309470978907126878";
                    break;
            }

            var percent = order.Auction.InvoicePercent;

            if (isWarranty)
            {
                invoiceEx.PaymentCode = "171";
                invoiceEx.Services = new ServicesRow[order.Auction.SiteId == 4 ? 2 : 1];

                invoiceEx.Services[0] = new ServicesRow();
                invoiceEx.Services[0].Service = "Гарантийное обеспечение";
                invoiceEx.Services[0].ServiceContent = "Гарантийное обеспечение по аукциону №" + order.Auction.Number;
                invoiceEx.Services[0].Quantity = 1;

                if (order.Auction.Lots[0].Name.ToUpper().Contains("БЕНЗИН") || order.Auction.Lots[0].Name.ToUpper().Contains("ДИЗЕЛЬ"))
                    invoiceEx.Services[0].Price = (float)Math.Round(order.Auction.SupplierOrders[0].MinimalPrice / 100 * Convert.ToDecimal(0.4), 0);
                else
                {
                    if (order.Auction.SupplierOrders[0].BIN == "141040012412") invoiceEx.Services[0].Price = (float)Math.Round(order.Auction.SupplierOrders[0].MinimalPrice / 100 * Convert.ToDecimal(percent), 2);
                    else invoiceEx.Services[0].Price = (float)Math.Round(order.Auction.SupplierOrders[0].MinimalPrice / 100 * Convert.ToDecimal(percent), 0);
                }

                if (order.Auction.SupplierOrders[0].BIN == "141040012412") invoiceEx.Services[0].Аmount = (float)Math.Round((decimal)(invoiceEx.Services[0].Price * invoiceEx.Services[0].Quantity), 2);
                else invoiceEx.Services[0].Аmount = (float)Math.Round((decimal)(invoiceEx.Services[0].Price * invoiceEx.Services[0].Quantity), 0);

                if (order.Auction.SiteId == 4)
                {
                    invoiceEx.Services[1] = new ServicesRow();                    
                    invoiceEx.Services[1].Service = "Открытие и введение счета на товарной бирже";
                    invoiceEx.Services[1].ServiceContent = "Открытие и ведение счета на товарной бирже";
                    invoiceEx.Services[1].Quantity = 1;
                    invoiceEx.Services[1].Price = order.Auction.SupplierOrders[0].MinimalPrice < 1000000 ? 500 : 5000;
                    invoiceEx.Services[1].Аmount = order.Auction.SupplierOrders[0].MinimalPrice < 1000000 ? 500 : 5000;
                }
            }
            else
            {
                invoiceEx.PaymentCode = "859";
                invoiceEx.Services = new ServicesRow[order.Auction.SiteId == 4 ? 2 : 1];

                invoiceEx.Services[0] = new ServicesRow();
                invoiceEx.Services[0].Service = "Брокерские услуги";
                invoiceEx.Services[0].ServiceContent = "Брокерские услуги по аукциону №" + order.Auction.Number;
                invoiceEx.Services[0].Quantity = 1;

                if (order.Auction.Lots[0].Name.ToUpper().Contains("БЕНЗИН") || order.Auction.Lots[0].Name.ToUpper().Contains("ДИЗЕЛЬ"))
                    invoiceEx.Services[0].Price =onlyPlay?0: (float)Math.Round(order.Auction.SupplierOrders[0].MinimalPrice / 100 * Convert.ToDecimal(0.4), 0);
                else
                {
                    if (order.Auction.SupplierOrders[0].BIN == "141040012412") invoiceEx.Services[0].Price = onlyPlay ? 0 : (float)Math.Round(order.Auction.SupplierOrders[0].MinimalPrice / 100 * Convert.ToDecimal(percent), 2);
                    else invoiceEx.Services[0].Price = onlyPlay ? 0 : (float)Math.Round(order.Auction.SupplierOrders[0].MinimalPrice / 100 * Convert.ToDecimal(percent), 0);
                }

                if (order.Auction.SupplierOrders[0].BIN == "141040012412") invoiceEx.Services[0].Аmount = onlyPlay ? 0 : (float)Math.Round((decimal)(invoiceEx.Services[0].Price * invoiceEx.Services[0].Quantity), 2);
                else invoiceEx.Services[0].Аmount = onlyPlay ? 0 : (float)Math.Round((decimal)(invoiceEx.Services[0].Price * invoiceEx.Services[0].Quantity), 0);

                if (order.Auction.SiteId == 4)
                {
                    invoiceEx.Services[1] = new ServicesRow();
                    invoiceEx.Services[1].Service = "Открытие и введение счета на товарной бирже";
                    invoiceEx.Services[1].ServiceContent = "Открытие и ведение счета на товарной бирже";
                    invoiceEx.Services[1].Quantity = 1;
                    invoiceEx.Services[1].Price = order.Auction.SupplierOrders[0].MinimalPrice < 1000000 ? 500 : 5000;
                    invoiceEx.Services[1].Аmount = order.Auction.SupplierOrders[0].MinimalPrice < 1000000 ? 500 : 5000;
                }
            }

            responseData = baseConn.writeInvoiceEx(invoiceEx, false, isWarranty);

            if (responseData.RequestSuccess) return (responseData.ResponseObject as InvoiceSearch).DocNum;
            else
            {
                AppJournal.Write("1C Transport", "Create invoice error :" + responseData.ErrorMsg, true);
                return "Err: " + responseData.ErrorMsg;
            }
        }


        public static InvoicePrint SearchInvoice(int brokerType, string docNum, bool isWarranty)
        {
            AppJournal.Write("1C Transport", "Search invoice", true);

            ChooseBaseType(brokerType);

            InvoiceSearch invoiceSearch = new InvoiceSearch();
            invoiceSearch.DocDate = new DateTime(DateTime.Now.Year, 01, 01);

            ResponseData responseData = new ResponseData();

            invoiceSearch.DocNum = docNum.PadLeft(11, '0');
            invoiceSearch.GO = isWarranty;

            try
            {
                responseData = baseConn.printInvoice(invoiceSearch);
            }
            catch (Exception ex) { AppJournal.Write("1C Transport", "Search invoice error :" + ex.ToString(), true); }

            if (responseData.RequestSuccess) return (responseData.ResponseObject as InvoicePrint);
            else
            {
                AppJournal.Write("1C Transport", "Search invoice error :" + responseData.ErrorMsg, true);
                return null;
            }
        }

        public static InvoiceEx ReadInvoice(int brokerType, string docNum, bool isWarranty)
        {
            AppJournal.Write("1C Transport", "Read invoice", true);

            ChooseBaseType(brokerType);

            InvoiceSearch invoiceSearch = new InvoiceSearch();
            invoiceSearch.DocDate = new DateTime(DateTime.Now.Year, 01, 01);

            ResponseData responseData = new ResponseData();

            invoiceSearch.DocNum = docNum.PadLeft(11, '0');
            invoiceSearch.GO = isWarranty;

            try
            {
                responseData = baseConn.readInvoiceEx(invoiceSearch);
            }
            catch (Exception) { }

            if (responseData.RequestSuccess) return (responseData.ResponseObject as InvoiceEx);
            else
            {
                AppJournal.Write("1C Transport", "Read invoice error :" + responseData.ErrorMsg, true);
                return null;
            }
        }

        public static List<DebtorReport> GetDebtors(int baseType, DateTime startDate, DateTime endDate, string clientBin)
        {
            string[] cBin = new string[] { clientBin };

            return GetDebtors(baseType, startDate, endDate, cBin);
        }

        public static List<DebtorReport> GetDebtors(int baseType, DateTime startDate, DateTime endDate, string[] clientBins)
        {
            AppJournal.Write("1C Transport", "Get debtors", true);

            ChooseBaseType(baseType);

            RRSelection rRSelection = new RRSelection();
            List<DebtorReport> debtorReport = new List<DebtorReport>();

            rRSelection.StartDate = startDate;
            rRSelection.EndDate = endDate;

            foreach (var item in clientBins)
            {
                rRSelection.ClientIIN = item;

                var reconcilationReportRows = baseConn.getDebitorsReport(rRSelection);

                if (reconcilationReportRows != null)
                {
                    foreach (var subItem in reconcilationReportRows)
                    {
                        if (subItem.Credit != subItem.Debit)
                        {
                            debtorReport.Add(new DebtorReport()
                            {
                                clientBIN = subItem.Client,
                                credit = Convert.ToDecimal(subItem.Credit) > Convert.ToDecimal(subItem.Debit) ? Convert.ToDecimal(subItem.Credit) - Convert.ToDecimal(subItem.Debit) : 0,
                                debit = Convert.ToDecimal(subItem.Debit) > Convert.ToDecimal(subItem.Credit) ? Convert.ToDecimal(subItem.Debit) - Convert.ToDecimal(subItem.Credit) : 0,
                                result = Convert.ToDecimal(subItem.Debit) - Convert.ToDecimal(subItem.Credit),
                                brokerName = baseType == 1 ? "ТОО Альта и К" : baseType == 2 ? "Корунд-777" : "Ак Алтын Ко",
                                balance = (Convert.ToDecimal(subItem.Debit) - Convert.ToDecimal(subItem.Credit)) > 0 ? true : false
                            });
                        }
                    }
                }
            }

            return debtorReport;
        }

        public static List<ReconcilationReport> GetReconcilation(string Url, string Login, string Pass, string clientBin, string contractNumber, DateTime startDate, DateTime endDate)
        {
            baseConn = new InvoicePaymentConnectionProtocol(Url, Login, Pass);
      

            return GetReconcilation(clientBin, contractNumber, startDate, endDate);
        }

        public static List<ReconcilationReport> GetReconcilation(int baseType, string clientBin, string contractNumber, DateTime startDate, DateTime endDate)
        {
            ChooseBaseType(baseType);

            return GetReconcilation(clientBin, contractNumber, startDate, endDate);
        }

        public static List<ReconcilationReport> GetReconcilation(string clientBin, string contractNumber, DateTime startDate, DateTime endDate)
        {
            List<ReconcilationReport> reconcilationReport = new List<ReconcilationReport>();

            RRSelection rRSelection = new RRSelection();
            rRSelection.ClientIIN = clientBin;
            //rRSelection.ContractName = contractNumber;
            //rRSelection.EndDate = endDate;
            //rRSelection.StartDate = startDate;

            var reconcilationReportRows = baseConn.getReconciliationReport(rRSelection);

            if (reconcilationReportRows != null)
            {
                foreach (var item in reconcilationReportRows)
                {
                    reconcilationReport.Add(new ReconcilationReport()
                    {
                        clientName = item.Client,
                        contractNum = item.Contract,
                        credit = (decimal)item.Credit,
                        debit = (decimal)item.Debit,
                        docDate = item.DocDate,
                        docName = item.DocName,
                        currency = item.Сurrency,
                    });
                }
                return reconcilationReport;
            }
            else return null;
        }
    }
}

