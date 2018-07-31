using AltaBO.specifics;
using System;

namespace AltaBO
{
    public class DocumentRequisite
    {
        public int id { get; set; }
        public String number { get; set; }
        public DateTime date { get; set; }
        public string fileName { get; set; }
        public string description { get; set; }
        public string extension { get; set; }
        public int? filesListId { get; set; }
        public DocumentTypeEnum type { get; set; }
        public MarketPlaceEnum market { get; set; }
        public DocumentSectionEnum section { get; set; }
    }
}