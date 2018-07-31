using System;
using System.Collections.Generic;
using System.Xml.Linq;
using AltaBO;

namespace DocumentFormation
{
    /// <summary>
    /// Converts reports as xml file to Report BO
    /// </summary>
    public class ETSReportConverter
    {
        /// <summary>
        /// Converts incoming xml file to Report BO and returns them.
        /// </summary>
        /// <param name="fileName"> Full file name of imcoming xml report file</param>
        /// <returns></returns>
        public static List<Report> GetReports(string fileName)
        {
            var xDoc = XDocument.Load(fileName);
            var reports = new List<Report>();
            var commons = new ReportCommonFields();

            if (xDoc.Root == null) return reports;
            foreach (var fAtr in xDoc.Root.Attributes()) {
                if (fAtr.Name == "Id") commons.id = fAtr.Value;
                if (fAtr.Name == "Name") commons.name = fAtr.Value;
                if (fAtr.Name == "DateTo") commons.dateTo = fAtr.Value;
            }

            foreach (var el in xDoc.Root.Elements()) {
                if (el.Name == "Issue") {

                    var report = new Report {Commons = commons};

                    foreach (var atr in el.Attributes()) {
                        if (atr.Name == "Code") report.Code = atr.Value;
                        if (atr.Name == "Name") report.ProductName = atr.Value;
                    }

                    foreach (var subEl in el.Elements()) {
                        foreach (var subAtr in subEl.Attributes()) {
                            if (subAtr.Name == "Number") report.Number = subAtr.Value;
                            if (subAtr.Name == "Moment") report.Moment = subAtr.Value;
                            if (subAtr.Name == "Qty") report.Qty = subAtr.Value;

                            decimal tPrice;

                            if (subAtr.Name == "Price") {
                                tPrice = Convert.ToDecimal(subAtr.Value.Replace(".", ","));
                                report.Price = $"{tPrice:C}";
                                report.Price = report.Price.Substring(0, report.Price.Length - 2) + " тенге, с учетом НДС";
                            }
                            if (subAtr.Name == "Amt") {
                                tPrice = Convert.ToDecimal(subAtr.Value.Replace(".", ","));
                                report.Amt = $"{tPrice:C}";
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

                    switch (report.Id)
                    {
                        case "ALTK":
                            report.Director = "Андреев В.И.";
                            break;
                        case "KORD":
                            report.Director = "Мешков Е.В.";
                            break;
                        case "ALTA":
                            report.Director = "Кулик В.К.";
                            break;
                        case "AKAL":
                            report.Director = "Милошенко И.А.";
                            break;
                    }

                    reports.Add(report);
                }
            }

            return reports;
        }
    }
}