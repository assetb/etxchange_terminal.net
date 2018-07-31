using System;

namespace AltaBO
{
    public class Company
    {
        public int id { get; set; }
        public string bin { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string fax { get; set; }
        public int? countryId { get; set; }
        public string country { get; set; }
        public string addressLegal { get; set; }
        public string addressActual { get; set; }
        public string postCode { get; set; }
        public string directorPowers { get; set; }
        public string director { get; set; }
        public string comments { get; set; }
        public int? kbe { get; set; }
        public string iik { get; set; }
        public string bik { get; set; }
        public DateTime updateDate { get; set; }
        public DateTime createDate { get; set; }
        public string govregnumber { get; set; }
        public DateTime? govregdate { get; set; }
        public string bank { get; set; }
        public int filesListId { get; set; }
    }
}
