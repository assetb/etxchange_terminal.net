using System;
using System.Collections.Generic;
using System.Xml.Linq;
using AltaBO;

namespace DocumentFormation
{
    public class ReportConverter
    {
        public static List<Report> GetReports(string fileFullName)
        {
            var xDoc = XDocument.Load(fileFullName);

            var reports = new List<Report>();

            var commons = new ReportCommonFields();
            // Корневые атрибуты Reciever
            if (xDoc.Root != null) {
                foreach (var fAtr in xDoc.Root.Attributes()) {
                    if (fAtr.Name == "Id") commons.id = fAtr.Value;
                    if (fAtr.Name == "Name") commons.name = fAtr.Value;
                    if (fAtr.Name == "DateTo") commons.dateTo = fAtr.Value;
                }

                foreach (var el in xDoc.Root.Elements()) {
                    if (el.Name == "Issue") {

                        var report = new Report();
                        report.Commons = commons;

                        // Пробежка по записям 
                        foreach (var atr in el.Attributes()) {
                            if (atr.Name == "Code") report.Code = atr.Value;
                            if (atr.Name == "Name") report.ProductName = atr.Value;
                        }

                        foreach (var subEl in el.Elements()) {
                            foreach (var subAtr in subEl.Attributes()) {
                                if (subAtr.Name == "Number") report.Number = subAtr.Value;
                                if (subAtr.Name == "Moment") report.Moment = subAtr.Value;
                                if (subAtr.Name == "Qty") report.Qty = subAtr.Value;

                                decimal tPrice = 0;

                                if (subAtr.Name == "Price") {
                                    tPrice = Convert.ToDecimal(subAtr.Value.Replace(".", ","));
                                    report.Price = string.Format("{0:C}", tPrice);
                                    report.Price = report.Price.Substring(0, report.Price.Length - 2) + " тенге, с учетом НДС";
                                }
                                if (subAtr.Name == "Amt") {
                                    tPrice = Convert.ToDecimal(subAtr.Value.Replace(".", ","));
                                    report.Amt = string.Format("{0:C}", tPrice);
                                    report.Amt = report.Amt.Substring(0, report.Amt.Length - 2) + " тенге, с учетом НДС";
                                }
                            }

                            foreach (var moreSubEl in subEl.Elements()) {
                                foreach (var moreSubAtr in moreSubEl.Attributes()) {
                                    if (moreSubAtr.Name == "ClientName") report.ClientName = moreSubAtr.Value;
                                    if (moreSubAtr.Name == "ContrCode") report.ContrCode = moreSubAtr.Value;
                                }
                            }
                        }

                        // Вставка данных в отчет-шаблон
                        switch (report.Id) {
                            case "ALTK":
                                report.Id = "Альта и К";
                                report.Director = "Андреев В.И.";
                                break;
                            case "KORD":
                                report.Id = "Корунд-777";
                                report.Director = "Мешков Е.В.";
                                break;
                            case "ALTA":
                                report.Id = "Альтаир-Нур";
                                report.Director = "Кулик В.К.";
                                break;
                            case "AKAL":
                                report.Id = "Ак Алтын Ко";
                                report.Director = "Милошенко И.А.";
                                break;
                        }
                        
                        reports.Add(report);
                    }
                }
            }
            
            return reports;
        }


    }
}
