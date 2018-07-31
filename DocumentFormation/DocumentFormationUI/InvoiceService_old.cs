using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using AltaBO;
using AltaOffice;
using AltaTransport;
using Invoice = AltaBO.Invoice;

namespace DocumentFormation
{
    public class InvoiceService_old
    {
        private Invoice invoice;
        private MSSQLTransport msSQLTransport;
        private DataSet dataSet;
        private string fPath;
        private WordService word;
        private int type;


        public InvoiceService_old() { }


        // Get info about broker and buyer
        public void CreateInvoice(Invoice invoice, int type)
        {
            this.invoice = invoice;
            this.type = type; // 1-Before, 2-After bargain

            ConnectToDB();

            if (GetFromDB() != 1) return;

            SetDataFor1C();

            if (CopyTemplate() != 1) return;
            if (OpenTemplate() != 1) return;
            if (FillFileWithData() != 1) return;

            SaveAndClose();

            Process.Start("explorer", fPath);
        }


        // Connect to db Ais
        private void ConnectToDB()
        {
            msSQLTransport = new MSSQLTransport();
        }


        // Get from db lefted pieces of info
        private int GetFromDB()
        {
            var regex = new Regex(@"[\d*]{1,}");

            var cBIN = invoice.buyerBIN;
            cBIN = Regex.Replace(cBIN, @"[\s]", " ");

            var itmp = 0;

            while(cBIN.IndexOf(" ") != -1) {
                itmp = cBIN.IndexOf(" ");
                cBIN = cBIN.Remove(cBIN.IndexOf(" "), 1);
            }

            cBIN = regex.Match(cBIN).ToString();
            invoice.buyerBIN = cBIN;

            // Get info about buyer from company
            dataSet = msSQLTransport.Execute("select a.* from altair.dbo.companiesView a where a.bin like'%" + invoice.buyerBIN + "%'");

            if (dataSet == null || dataSet.Tables.Count <= 0 || string.IsNullOrEmpty(invoice.buyerBIN)) return 0;

            invoice.buyerName = dataSet.Tables[0].Rows[0].Field<string>("fullName");
            invoice.buyerIIK = dataSet.Tables[0].Rows[0].Field<string>("iik");
            invoice.buyerAddress = dataSet.Tables[0].Rows[0].Field<string>("addrJur");

            var id = dataSet.Tables[0].Rows[0].Field<long>("id").ToString();

            // Get info about broker and supplier from company
            dataSet = msSQLTransport.Execute("select a.* from altair.dbo.companiesView a where a.bin='" + invoice.brokerBIN + "'");

            if (dataSet == null || dataSet.Tables.Count <= 0) return 0;

            invoice.brokerName = dataSet.Tables[0].Rows[0].Field<string>("fullName");
            invoice.supplierName = invoice.brokerName;
            invoice.supplierAddress = dataSet.Tables[0].Rows[0].Field<string>("addrJur");
            invoice.supplierBIN = invoice.brokerBIN;
            invoice.brokerIIK = dataSet.Tables[0].Rows[0].Field<string>("iik");
            invoice.brokerKbe = dataSet.Tables[0].Rows[0].Field<string>("kbe");
            invoice.brokerBIK = dataSet.Tables[0].Rows[0].Field<string>("bik");

            // Broker bank name from banks
            dataSet = msSQLTransport.Execute("select a.* from altair.dbo.banksView a where a.bik='" + invoice.brokerBIK + "'");

            if (dataSet == null || dataSet.Tables.Count <= 0) return 0;

            invoice.brokerBankName = dataSet.Tables[0].Rows[0].Field<string>("bankName");

            // Contract data from contracts
            dataSet = msSQLTransport.Execute("select a.* from altair.dbo.contractsView a where a.clientId='" + id + "' and a.brokerid='" + invoice.brokerId + "'");

            if (dataSet == null || dataSet.Tables.Count <= 0) return 0;

            var lRow = dataSet.Tables[0].Rows.Count - 1;

            invoice.contractNumber = dataSet.Tables[0].Rows[lRow].Field<string>("contractNum");
            invoice.contractId = dataSet.Tables[0].Rows[lRow].Field<string>("id1c");
            invoice.contractDate = dataSet.Tables[0].Rows[lRow].Field<DateTime>("contractDate").ToString("dd.MM.yyyy");

            id = dataSet.Tables[0].Rows[lRow].Field<long>("id").ToString();

            // Tarif data from contracts
            dataSet = msSQLTransport.Execute("select a.* from altair.dbo.tarifView a where a.contractId='" + id + "'");

            if (dataSet == null || dataSet.Tables.Count <= 0) return 0;

            var sum = Convert.ToDecimal(invoice.invoiceSum);
            decimal percent = 0, bord = 0;
            var exchangeName = "";

            foreach (DataRow item in dataSet.Tables[0].Rows) {
                if (sum > bord && sum < item.Field<decimal>("bord")) break;

                bord = item.Field<decimal>("bord");
                percent = item.Field<decimal>("percent1");
                exchangeName = item.Field<string>("exchangeName");

                var sPercent = "0";

                if (exchangeName.Equals("Астана")) {
                    if (InputBoxService.InputBox("Тарифная сетка", "Введите процент:", ref sPercent) == DialogResult.OK) {
                        percent = Convert.ToDecimal(sPercent);
                    }

                    break;
                }
            }

            sum = sum / 100 * percent;

            if (Convert.ToDecimal(invoice.invoiceSum) < 1000000) invoice.invoiceBrokerSum = "500";
            else invoice.invoiceBrokerSum = "5000";

            invoice.invoiceSum = sum.ToString();

            return 1;
        }


        private void SetDataFor1C()
        {
            var invoice1C = new Invoice1C();

            invoice1C.AMOUNT = Convert.ToDecimal(invoice.invoiceSum) + Convert.ToDecimal(invoice.invoiceBrokerSum);
            invoice1C.BILL_DATE = invoice.invoiceDate;
            invoice1C.BIN_CLIENT = invoice.buyerBIN;

            if (type != 1) 
                invoice1C.COMMENTS = "Брокерские услуги";
            else
                invoice1C.COMMENTS = "Гарантийное обеспечение за участие в аукционе";

            invoice1C.COMMENTS2 = "Пользователь АО ЕТС";
            invoice1C.CONTRACT = "Договор №" + invoice.contractNumber + " от " + invoice.contractDate + "г.";
            invoice1C.CONTRACT_ID = invoice.contractId;
            invoice1C.COUNT = 1;
            invoice1C.brokerId = Convert.ToInt64(invoice.brokerId);

            if (type != 2) {
                invoice1C.DOC_TYPE = "СчетГО";
                invoice1C.SERVICE_NAME = "Гарантийное обеспечение за участие в аукционе";
            } else {
                invoice1C.DOC_TYPE = "Счет";
                invoice1C.SERVICE_NAME = "Брокерские услуги";
            }

            invoice1C.IIK_CLIENT = invoice.buyerIIK;
            invoice1C.IIK_HOLDER = invoice.brokerIIK;
            invoice1C.KNP = invoice.payCode;
            invoice1C.PRICE = Convert.ToDecimal(invoice.invoiceSum) + Convert.ToDecimal(invoice.invoiceBrokerSum);
            invoice1C.UNIT = "шт.";

            var returnInfo = new ReturnInfo1C();

            var xmlSer = new XmlSerializer(typeof(Invoice1C));

            var stringWriter = new StringWriter();

            var xmlWriter = XmlWriter.Create(stringWriter);

            xmlSer.Serialize(xmlWriter, invoice1C);

            var strXml = stringWriter.ToString();
            var t = string.Empty;

            try {
                t = NewInvoice(strXml, Convert.ToInt32(invoice1C.brokerId));
            } catch (Exception e) {
                returnInfo.ERROR_CODE = -1;
                returnInfo.MESSAGE = e.Message;
            }

            t = t.Replace("> <", "><");

            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(t);

            var subReturnInfo = new ReturnInfo1C();

            foreach (XmlElement el in xmlDoc) {
                foreach (XmlElement subEl in el) {
                    if (subEl.Name == "ERROR_CODE") subReturnInfo.ERROR_CODE = Convert.ToInt32(subEl.InnerText);
                    else if (subEl.Name == "MESSAGE") subReturnInfo.MESSAGE = subEl.InnerText;
                    else if (subEl.Name == "BILL_NUM") subReturnInfo.BILL_NUM = subEl.InnerText;
                    else if (subEl.Name == "DOC_ID") subReturnInfo.DOC_ID = subEl.InnerText;
                }
            }

            invoice.invoiceNumber = subReturnInfo.BILL_NUM;
        }


        private string NewInvoice(string data, int broker)
        {
            var oneCTransport = new OneCTransport();

            return null;//oneCTransport.NewInvoice(data, broker);
        }


        // Copy needed word template of invoice
        private int CopyTemplate()
        {
            var fName = "";

            switch (invoice.brokerCode.ToLower()) {
                case "kord":
                    fName = "Korund";
                    break;
                case "altk":
                    fName = "Altaik";
                    break;
                case "akal":
                    fName = "AkAltyn";
                    break;
            }

            var wherePath = Service.GetDirectory();
            if (wherePath != null) {

                fPath = wherePath.FullName;

                System.IO.File.Copy(FileArchiveTransport.GetTemplatesPath() + "\\" + "bill" + fName + ".docx", fPath + "\\Счет (" + invoice.invoiceNumber + ") от " + invoice.brokerCode + ".docx");
            } else return 0;

            return 1;
        }


        // Open template
        private int OpenTemplate()
        {
            try {
                word = new WordService(fPath + "\\Счет (" + invoice.invoiceNumber + ") от " + invoice.brokerCode + ".docx", false);
            } catch (Exception ex) { Debug.WriteLine(ex.Message); return 0; }

            return 1;
        }


        // Paste info to file
        private int FillFileWithData()
        {
            try {
                word.FindReplace("[brokerName]", invoice.brokerName);
                word.FindReplace("[brokerIIK]", invoice.brokerIIK);
                word.FindReplace("[brokerKbe]", invoice.brokerKbe);
                word.FindReplace("[brokerBIN]", invoice.brokerBIN);
                word.FindReplace("[brokerBankName]", invoice.brokerBankName);
                word.FindReplace("[brokerBIK]", invoice.brokerBIK);
                word.FindReplace("[payCode]", invoice.payCode);
                word.FindReplace("[invoiceNumber]", invoice.invoiceNumber);
                word.FindReplace("[invoiceDate]", invoice.invoiceDate);
                word.FindReplace("[supplierName]", invoice.supplierName);
                word.FindReplace("[supplierBIN]", invoice.supplierBIN);
                word.FindReplace("[supplierAddress]", invoice.supplierAddress);
                word.FindReplace("[buyerName]", invoice.buyerName);
                word.FindReplace("[buyerBIN]", invoice.buyerBIN);
                word.FindReplace("[buyerAddress]", invoice.buyerAddress);
                word.FindReplace("[contractNum]", invoice.contractNumber);
                word.FindReplace("[contractDate]", invoice.contractDate);

                var tSum = Convert.ToDecimal(invoice.invoiceSum);
                var tBSum = Convert.ToDecimal(invoice.invoiceBrokerSum);
                var aSum = Convert.ToDecimal(invoice.invoiceSum) + Convert.ToDecimal(invoice.invoiceBrokerSum);

                if (type != 1) {
                    word.SetCell(3, 2, 1, "1");
                    word.SetCell(3, 2, 3, "Брокерские услуги");
                    word.SetCell(3, 2, 5, "услуга");
                    word.SetCell(3, 2, 6, Math.Round(tSum, 0) + ",00");
                    word.SetCell(3, 2, 7, Math.Round(tSum, 0) + ",00");
                    word.FindReplace("[count]", "1");
                    aSum = tSum;
                } else {
                    word.AddTableRow(3);
                    word.SetCell(3, 2, 1, "1");
                    word.SetCell(3, 2, 3, "Гарантийное обеспечение за участие в аукционе №" + invoice.auctionNumber);
                    word.SetCell(3, 2, 6, Math.Round(tSum, 0) + ",00");
                    word.SetCell(3, 2, 7, Math.Round(tSum, 0) + ",00");
                    word.SetCell(3, 3, 1, "2");
                    word.SetCell(3, 3, 3, "Открытие и ведение счета на товарной бирже");
                    word.SetCell(3, 3, 4, "1");
                    word.SetCell(3, 3, 5, "тг.");
                    word.SetCell(3, 3, 6, Math.Round(tBSum, 0) + ",00");
                    word.SetCell(3, 3, 7, Math.Round(tBSum, 0) + ",00");
                    word.FindReplace("[count]", "2");
                }

                word.FindReplace("[invoiceSum]", Math.Round(aSum, 0) + ",00");
                word.FindReplace("[invoiceSum]", Math.Round(aSum, 0) + ",00");
                word.FindReplace("[ndsSum]", Math.Round((aSum * 12 / 112), 2).ToString());
            } catch (Exception ex) { Debug.WriteLine(ex.Message); return 0; }

            return 1;
        }


        // Save and close word
        private void SaveAndClose()
        {
            word.CloseDocument(true);
            word.CloseWord(true);
        }
    }
}
