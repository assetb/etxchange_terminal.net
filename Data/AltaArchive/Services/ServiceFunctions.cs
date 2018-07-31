using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchive.Services {
    public static class ServiceFunctions {
        public static string CompanyRenamer(string companyName) {
            // Make name more short
            companyName = companyName.Replace("Товарищество с ограниченной ответственностью", "ТОО");
            companyName = companyName.Replace("Общество с ограниченной ответственностью", "ООО");
            companyName = companyName.Replace("Акционерное общество", "АО");
            companyName = companyName.Replace("Индивидуальный предприниматель", "ИП");

            // Replace system symbols
            companyName = companyName.Replace("\\", "_");
            companyName = companyName.Replace("\"", "'");
            companyName = companyName.Replace("/", "_");

            return companyName;
        }
    }
}
