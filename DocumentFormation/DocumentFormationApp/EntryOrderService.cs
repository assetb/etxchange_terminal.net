using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AltaBO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class EntryOrderService
    {
        private readonly EntryOrder entryOrder;
        private readonly string wherePath;
        private const string FILE_NAME = "order_ets_entry.docx";
        private MSSQLTransport msSQLTransport;
        private DataSet dataSet;


        public EntryOrderService(EntryOrder entryOrder, string wherePath)
        {
            this.entryOrder = entryOrder;
            this.wherePath = wherePath;
        }


        public void CopyTemplateToEndPath()
        {
            System.IO.File.Copy(FileArchiveTransport.GetEntryOrderTemplateFileName(), wherePath + "\\" + FILE_NAME, true);
        }


        public void FillFileAndSave()
        {
            // TODO : Fill base
            //DataBaseTransport.AddSupplierOrder(entryOrder);

            GetInfoFromDB();

            var word = new WordService(wherePath + "\\" + FILE_NAME, false);

            word.FindReplace("[fromDate]", entryOrder.fromDate);
            //word.FindReplace("[brokerClient]", entryOrder.brokerClient);
            //word.FindReplace("[brokerClient]", entryOrder.brokerClient);
            word.FindReplace("[lotNumber]", entryOrder.lotNumber);
            word.FindReplace("[memberCode]", entryOrder.memberCode);
            word.FindReplace("[fullMemberName]", entryOrder.fullMemberName);
            word.FindReplace("[clientCode]", entryOrder.clientCode);
            word.FindReplace("[clientFullName]", entryOrder.clientFullName);
            word.FindReplace("[clientAddress]", entryOrder.clientAddress);
            word.FindReplace("[clientBIN]", entryOrder.clientBIN);
            word.FindReplace("[clientPhones]", entryOrder.clientPhones);
            word.FindReplace("[clientBankIIK]", entryOrder.clientBankIIK);
            word.FindReplace("[clientBankBIK]", entryOrder.clientBankBIK);
            word.FindReplace("[clientBankName]", entryOrder.clientBankName);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Список запрашиваемых документов").Append("\n");
            stringBuilder.Append("1. Заявка на участие").Append("\n");

            var iCount = 2;

            foreach (var rDoc in entryOrder.requestedDocs) {
                stringBuilder.Append(iCount+". "+ rDoc.name).Append("\n");
                iCount++;
            }

            word.SetCell(2, 3, 2, stringBuilder.ToString());

            word.CloseDocument(true);
            word.CloseWord(true);

            ChangeFileName();
        }


        private void GetInfoFromDB()
        {
            msSQLTransport = new MSSQLTransport();
            var regex = new Regex(@"[\d*]{1,}");

            var cBIN = entryOrder.clientBIN.Replace(" ", "");

            cBIN = regex.Match(cBIN).ToString();
            entryOrder.clientBIN = cBIN;

            dataSet = msSQLTransport.Execute("select a.* from altair.dbo.companiesView a where a.bin='" + entryOrder.clientBIN + "'");

            if (dataSet == null || dataSet.Tables.Count <= 0) return;

            entryOrder.clientFullName = dataSet.Tables[0].Rows[0].Field<string>("fullName");
            entryOrder.clientAddress = dataSet.Tables[0].Rows[0].Field<string>("addrJur");
            entryOrder.clientPhones = dataSet.Tables[0].Rows[0].Field<string>("phones");
            entryOrder.clientBankIIK = dataSet.Tables[0].Rows[0].Field<string>("iik");
            entryOrder.clientBankBIK = dataSet.Tables[0].Rows[0].Field<string>("bik");

            dataSet = msSQLTransport.Execute("select a.* from altair.dbo.banksView a where a.bik='" + entryOrder.clientBankBIK + "'");

            if (dataSet == null || dataSet.Tables.Count <= 0) return;

            entryOrder.clientBankName = dataSet.Tables[0].Rows[0].Field<string>("bankName");
        }


        private void ChangeFileName()
        {
            System.IO.File.Move(wherePath + "\\" + FILE_NAME, wherePath + "\\Заявка на участие по лоту " + entryOrder.lotNumber + " клиенту " + entryOrder.clientCode + ".docx");
        }
    }
}
