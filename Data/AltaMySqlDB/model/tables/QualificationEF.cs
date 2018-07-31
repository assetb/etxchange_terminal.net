using AltaMySqlDB.model.catalogs;

namespace AltaMySqlDB.model.tables
{
    public class QualificationEF {        
        public int id { get; set; }
        public int auctionId { get; set; }
        public int qualification_dictionary_id { get; set; }
        public string note { get; set; }
        public bool file { get; set; }

        public virtual AuctionEF auction { get; set; }
        public virtual QualificationDictionaryEF qualificationDictionary { get; set; }
    }
}
