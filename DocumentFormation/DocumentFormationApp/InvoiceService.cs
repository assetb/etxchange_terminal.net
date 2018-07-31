using System;
using AltaTransport;
using AltaTransport._1c.InvoicePaiment;
using AltaOffice;
using System.Diagnostics;
using System.Globalization;
using AltaBO;

namespace DocumentFormation
{
    public static class InvoiceService
    {
        private static string fileName;
        private static WordService word;
        private static Order order = new Order();


        public static string CreateInvoice(bool isWarranty, Order orderInfo, string lotNumber = "", bool onlyPlay = false)
        {
            order = orderInfo;

            int brokerType;

            if (orderInfo.Auction.SupplierOrders[0].BrokerName.ToUpper().Contains("АЛЬТА")) brokerType = 1;
            else if (orderInfo.Auction.SupplierOrders[0].BrokerName.ToUpper().Contains("КОРУНД")) brokerType = 2;
            else brokerType = 3;

            var requestMsg = _1CTransport.CreateInvoice(brokerType, isWarranty, order, onlyPlay);

            if (!requestMsg.Contains("Err")) {
                fileName = ArchiveTransport.PutUTBBill(brokerType, orderInfo, isWarranty, lotNumber);
                FillInvoice(isWarranty, _1CTransport.SearchInvoice(brokerType, requestMsg, isWarranty), requestMsg, order.Auction.SiteId != 4 ? null : _1CTransport.ReadInvoice(brokerType, requestMsg, isWarranty));

                return null;
            } else return requestMsg;
        }


        private static void FillInvoice(bool isWarranty, InvoicePrint invoicePrint, string invoiceNumber, InvoiceEx invoiceEx = null)
        {
            string[] currencyType = new string[2];

            word = new WordService(fileName, false);

            try {
                word.FindReplace("[brokerName]", order.Auction.SupplierOrders[0].BrokerName);
                word.FindReplace("[brokerKbe]", order.Auction.SupplierOrders[0].BrokerKbe);
                word.FindReplace("[brokerBIN]", order.Auction.SupplierOrders[0].BrokerBIN);
                word.FindReplace("[brokerBankName]", order.Auction.SupplierOrders[0].BrokerBankName);
                word.FindReplace("[brokerBIK]", order.Auction.SupplierOrders[0].BrokerBIK);
                word.FindReplace("[payCode]", isWarranty ? "171" : "859");
                word.FindReplace("[invoiceNumber]", invoiceNumber);
                word.FindReplace("[invoiceDate]", isWarranty ? DateTime.Now.ToShortDateString() : order.Auction.Date.ToShortDateString());
                word.FindReplace("[supplierName]", order.Auction.SupplierOrders[0].BrokerName);
                word.FindReplace("[supplierBIN]", order.Auction.SupplierOrders[0].BrokerBIN);
                word.FindReplace("[supplierAddress]", order.Auction.SupplierOrders[0].BrokerAddress);
                word.FindReplace("[buyerName]", order.Auction.SupplierOrders[0].Name);
                word.FindReplace("[buyerBIN]", order.Auction.SupplierOrders[0].BIN);
                word.FindReplace("[buyerAddress]", order.Auction.SupplierOrders[0].Address);
                word.FindReplace("[contractNum]", "№" + order.Auction.SupplierOrders[0].ContractNum);
                word.FindReplace("[contractDate]", order.Auction.SupplierOrders[0].ContractDate.ToShortDateString());
                word.SetCell(3, 2, 1, "1");

                if (isWarranty) word.SetCell(3, 2, 3, "Гарантийное обеспечение за участие в аукционе №" + order.Auction.Number + " от " + order.Auction.Date.ToShortDateString());
                else word.SetCell(3, 2, 3, "Брокерские услуги за участие в аукционе №" + order.Auction.Number + " от " + order.Auction.Date.ToShortDateString());

                if (order.Auction.SiteId == 4) {
                    word.AddTableRow(3);
                    word.SetCell(3, 3, 1, "2");
                    word.SetCell(3, 3, 3, "Открытие и ведение счета на товарной бирже");
                    word.SetCell(3, 3, 4, "1");
                    word.SetCell(3, 3, 5, "шт.");
                    word.SetCell(3, 3, 6, invoiceEx.Services[1].Price.ToString());
                    word.SetCell(3, 3, 7, invoiceEx.Services[1].Аmount.ToString());
                }

                word.SetCell(3, 2, 6, invoicePrint.Price);
                word.SetCell(3, 2, 7, invoicePrint.Аmount);
                word.FindReplace("[count]", order.Auction.SiteId == 4 ? "2" : "1");

                if (order.Auction.SiteId == 4) {
                    if (order.Auction.SupplierOrders[0].BIN == "141040012412") invoicePrint.Total = (invoiceEx.Services[0].Аmount + invoiceEx.Services[1].Аmount).ToString();
                    else invoicePrint.Total = (invoiceEx.Services[0].Аmount + invoiceEx.Services[1].Аmount).ToString() + ",00";

                    if (order.Auction.SupplierOrders[0].BrokerName.ToUpper().Contains("АЛТЫН")) invoicePrint.TotalTax = "";
                    else invoicePrint.TotalTax = ((invoiceEx.Services[0].Аmount + invoiceEx.Services[1].Аmount) * 12 / 112).ToString();
                }

                word.FindReplace("[invoiceSum]", invoicePrint.Total);
                word.FindReplace("[ndsSum]", invoicePrint.TotalTax);
                word.FindReplace("[invoiceSum]", invoicePrint.Total);

                if (invoicePrint.АmountInWords.Contains("тиын")) {
                    word.FindReplace("[currency]", "KZT");
                    currencyType[0] = " тенге";
                    currencyType[1] = " тиын";

                    if (order.Auction.SupplierOrders[0].BrokerName.ToUpper().Contains("КОРУНД")) word.FindReplace("[brokerIIK]", order.Auction.SupplierOrders[0].BrokerIIK);
                    else word.FindReplace("[brokerIIK]", order.Auction.SupplierOrders[0].BrokerIIK);
                } else if (invoicePrint.АmountInWords.Contains("копеек")) {
                    word.FindReplace("[currency]", "RUB");
                    currencyType[0] = " рублей";
                    currencyType[1] = " копеек";

                    if (order.Auction.SupplierOrders[0].BrokerName.ToUpper().Contains("КОРУНД")) word.FindReplace("[brokerIIK]", "KZ739261802171874002");
                    else word.FindReplace("[brokerIIK]", order.Auction.SupplierOrders[0].BrokerIIK);
                } else if (invoicePrint.АmountInWords.Contains("центов")) {
                    word.FindReplace("[currency]", "USD");
                    currencyType[0] = " долларов";
                    currencyType[1] = " центов";

                    if (order.Auction.SupplierOrders[0].BrokerName.ToUpper().Contains("КОРУНД")) word.FindReplace("[brokerIIK]", "KZ039261802171874001");
                    else word.FindReplace("[brokerIIK]", order.Auction.SupplierOrders[0].BrokerIIK);
                }

                if (order.Auction.SiteId == 4) invoicePrint.АmountInWords = "Всего к оплате: " + ConvertNumberToWord(Convert.ToInt32(invoiceEx.Services[0].Аmount + invoiceEx.Services[1].Аmount)) + currencyType[0] + ",00" + currencyType[1];

                word.FindReplace("[amountInWords]", invoicePrint.АmountInWords);
            } catch (Exception ex) { Debug.WriteLine("Err:" + ex); }

            word.SaveAsPDF(fileName);

            word.CloseDocument(true);
            word.CloseWord(true);
        }

        private static string ConvertNumberToWord(int sourceNum)
        {
            int number = int.Parse(sourceNum.ToString());
            int[] array_int = new int[4];
            string[,] array_string = new string[4, 3] {{" миллиард", " миллиарда", " миллиардов"},
                {" миллион", " миллиона", " миллионов"},
                {" тысяча", " тысячи", " тысяч"},
                {"", "", ""}};

            array_int[0] = (number - (number % 1000000000)) / 1000000000;
            array_int[1] = ((number % 1000000000) - (number % 1000000)) / 1000000;
            array_int[2] = ((number % 1000000) - (number % 1000)) / 1000;
            array_int[3] = number % 1000;

            string result = "";

            for (int i = 0; i < 4; i++) {
                if (array_int[i] != 0) {
                    if (((array_int[i] - (array_int[i] % 100)) / 100) != 0)
                        switch (((array_int[i] - (array_int[i] % 100)) / 100)) {
                            case 1: result += " сто"; break;
                            case 2: result += " двести"; break;
                            case 3: result += " триста"; break;
                            case 4: result += " четыреста"; break;
                            case 5: result += " пятьсот"; break;
                            case 6: result += " шестьсот"; break;
                            case 7: result += " семьсот"; break;
                            case 8: result += " восемьсот"; break;
                            case 9: result += " девятьсот"; break;
                        }

                    if (((array_int[i] % 100) - ((array_int[i] % 100) % 10)) / 10 != 1) {
                        switch (((array_int[i] % 100) - ((array_int[i] % 100) % 10)) / 10) {
                            case 1: result += " десять"; break;
                            case 2: result += " двадцать"; break;
                            case 3: result += " тридцать"; break;
                            case 4: result += " сорок"; break;
                            case 5: result += " пятьдесят"; break;
                            case 6: result += " шестьдесят"; break;
                            case 7: result += " семьдесят"; break;
                            case 8: result += " восемьдесят"; break;
                            case 9: result += " девяносто"; break;
                        }
                    }

                    switch (array_int[i] % 10) {
                        case 1: if (i == 2) result += " одна"; else result += " один"; break;
                        case 2: if (i == 2) result += " две"; else result += " два"; break;
                        case 3: result += " три"; break;
                        case 4: result += " четыре"; break;
                        case 5: result += " пять"; break;
                        case 6: result += " шесть"; break;
                        case 7: result += " семь"; break;
                        case 8: result += " восемь"; break;
                        case 9: result += " девять"; break;
                    }
                } else switch (array_int[i] % 100) {
                        case 10: result += " десять"; break;
                        case 11: result += " одиннадцать"; break;
                        case 12: result += " двенадцать"; break;
                        case 13: result += " тринадцать"; break;
                        case 14: result += " четырнадцать"; break;
                        case 15: result += " пятнадцать"; break;
                        case 16: result += " шестнадцать"; break;
                        case 17: result += " семнадцать"; break;
                        case 18: result += " восемннадцать"; break;
                        case 19: result += " девятнадцать"; break;
                    }

                if (array_int[i] % 100 >= 10 && array_int[i] % 100 <= 19) result += " " + array_string[i, 2] + " ";
                else switch (array_int[i] % 10) {
                        case 1: result += " " + array_string[i, 0] + " "; break;
                        case 2:
                        case 3:
                        case 4: result += " " + array_string[i, 1] + " "; break;
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9: result += " " + array_string[i, 2] + " "; break;
                    }
            }

            return result;
        }
    }
}
