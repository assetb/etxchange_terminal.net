using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaOffice;
using AltaBO;
using AltaTransport;

namespace DocumentFormation {
    public class JournalC01Service {
        private static FormC01 formc01;
        private static string fileName;


        public static FormC01 FillC01(string brokerBin, string code, string companyBin, string companyName, string serial) {
            FormC01 formc01 = new FormC01();
            OurCompany ourCompany = new OurCompany();
            OurCompaniesConsts ourCompanies = new OurCompaniesConsts();

            ourCompanies.GetListOurCompanies();
            ourCompany = ourCompanies.GetListOurCompanies().Where(x => x.bin == brokerBin).First();

            formc01.code = code;
            formc01.bin = companyBin;
            formc01.date = DateTime.Now;
            formc01.name = companyName;
            formc01.broker = ourCompany;
            formc01.number = serial.Length == 3 ? serial : "0" + serial;
            formc01.codeG = formc01.broker.code.ToLower() + "g" + formc01.number;
            formc01.codeP = formc01.broker.code.ToLower() + "p" + formc01.number;
            formc01.codeS = formc01.broker.code.ToLower() + "s" + formc01.number;

            return formc01;
        }


        public static void CreateRecordInJournal(FormC01 formc01Item) {
            formc01 = formc01Item;

            fileName = ArchiveTransport.PutETSJournalC01(formc01Item.bin, formc01Item.code);

            FillDocument();
        }


        private static void FillDocument() {
            var word = new WordService(fileName, false);

            word.SetCell(1, 1, 2, formc01.broker.name);
            word.SetCell(1, 2, 2, formc01.broker.code);
            word.SetCell(2, 2, 1, formc01.code);
            word.SetCell(2, 2, 2, formc01.bin);
            word.SetCell(2, 2, 3, formc01.name);
            word.SetCell(2, 2, 5, formc01.codeG + " " + formc01.codeS);
            word.SetCell(2, 2, 6, formc01.codeP);

            word.CloseDocument(true);
            word.CloseWord(true);
        }
    }
}
