using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaOffice;
using AltaBO;

namespace DocumentFormation
{
    public static class ContractService
    {
        public static bool FillKaspiContract(string templateFileName, Contract contract, Company company, Company brokerCompany)
        {
            if (string.IsNullOrEmpty(templateFileName) || contract == null || company == null || brokerCompany == null) return false;

            var word = new WordService(templateFileName, false);

            if (word == null) return false;

            try {            
                for (var i = 0; i < 2; i++) word.FindReplace("[contractNumber]", contract.number != null ? contract.number : "");
                for (var i = 0; i < 2; i++) word.FindReplace("[agreementDate]", contract.agreementdate != null ? contract.agreementdate.Value.ToShortDateString() : "");
                for (var i = 0; i < 3; i++) word.FindReplace("[companyName]", company.name != null ? company.name : "");
                for (var i = 0; i < 3; i++) word.FindReplace("[companyDirector]", company.director != null ? company.director : "");
                for (var i = 0; i < 3; i++) word.FindReplace("[brokerName]", brokerCompany.name != null ? brokerCompany.name : "");
                for (var i = 0; i < 5; i++) word.FindReplace("[brokerDirector]", brokerCompany.director != null ? brokerCompany.director : "");
                for (var i = 0; i < 2; i++) word.FindReplace("[expirationDate]", contract.terminationdate != null ? contract.terminationdate.Value.ToShortDateString() : "");
                for (var i = 0; i < 2; i++) word.FindReplace("[companyLegalAddress]", company.addressLegal != null ? company.addressLegal : "");

                word.FindReplace("[companyBin]", company.bin != null ? company.bin : "");
                word.FindReplace("[companyIik]", company.iik != null ? company.iik : "");
                word.FindReplace("[companyBank]", company.bank != null ? company.bank : "");
                word.FindReplace("[companyBankBik]", company.bik != null ? company.bik : "");
                word.FindReplace("[companyKbe]", company.kbe != null ? company.kbe.ToString() : "");
                word.FindReplace("[brokerLegalAddress]", brokerCompany.addressLegal != null ? brokerCompany.addressLegal : "");
                word.FindReplace("[brokerBin]", brokerCompany.bin != null ? brokerCompany.bin : "");
                word.FindReplace("[brokerIik]", brokerCompany.iik != null ? brokerCompany.iik : "");
                word.FindReplace("[brokerBank]", brokerCompany.bank != null ? brokerCompany.bank : "");
                word.FindReplace("[brokerBankBik]", brokerCompany.bik != null ? brokerCompany.bik : "");
                word.FindReplace("[brokerKbe]", brokerCompany.kbe != null ? brokerCompany.kbe.ToString() : "");
                word.FindReplace("[brokerCurrencyInfo]", contract.currencyid != null ? (contract.currencyid == 1 ? "" : brokerCompany.comments != null ? brokerCompany.comments : "") : "");
                
                word.CloseDocument(true);
                word.CloseWord(true);
            } catch {
                if (word.IsOpenDocument()) word.CloseDocument(true);
                if (word.IsOpenWord()) word.CloseWord(true);

                return false;
            }

            return true;
        }                        
    }
}
