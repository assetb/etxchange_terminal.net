using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "brokersjournal")]
    public class BrokersJournalEF
    {
        #region Column
        
        public int id { get; set; }

        [Key]
        //[ForeignKey("broker")]
        public int brokerid { get; set; }

        public string code { get; set; }
        #endregion

        #region Foreign keys
        //public virtual BrokerEF broker { get; set; }
        #endregion
    }
}
