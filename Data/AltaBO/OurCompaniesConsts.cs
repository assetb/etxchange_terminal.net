using System.Collections.Generic;

namespace AltaBO
{
    public class OurCompaniesConsts
    {
        public List<OurCompany> OurCompanies = new List<OurCompany>() { new OurCompany() {
            code = "KORD",
            name = "ТОО \"Корунд-777\"",
            bin = "061140015550",
            email = "info@korund-777.kz",
            pass = "zyjgtwUr59yQjmPX",
            c01 = "korund"
        },
        new OurCompany() {
            code = "ALTA",
            name = "ТОО \"Альтаир-Нур\"",
            bin = "040240007942",
            email = "info@altairnur.kz",
            pass = "Z6ZhJ8FL3VfduCfL",
            c01 = "alta008"
        },
        new OurCompany() {
            code = "AKAL",
            name = "ТОО \"Ак Алтын Ко\"",
            bin = "151140023897",
            email = "",
            pass = "",
            c01 = "akaltko"
        },
            new OurCompany() {
            code = "ALTK",
            name = "ТОО \"Альта и К\"",
            bin = "140540002130",
            email = "info@altaik.kz",
            pass = "79v9HVGfVTnkhvvC",
            c01 = "altaik"
        }, };

        public List<OurCompany> GetListOurCompanies() {
            return new List<OurCompany>(OurCompanies);
        }

        public OurCompany GetOurCompany(string code){
            foreach(var com in OurCompanies)
            {
                if (com.code.ToLower() == code.ToLower())
                    return com;
            }
            return null;
        }
    }
}
