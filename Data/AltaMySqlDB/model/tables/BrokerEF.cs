using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "brokers")]
    public class BrokerEF
    {
        #region Columns
        [Key, ForeignKey("brokersJournal")]
        public int id { get; set; }

        public string name { get; set; }
        public string shortname { get; set; }

        public string requisites { get; set; }

        [ForeignKey("company")]
        public int companyId { get; set; }
        #endregion

        #region Foreign Objects
        public virtual CompanyEF company { get; set; }
        #endregion

        #region Freign Collections
        //public virtual ICollection<SiteEF> sites { get; set; }
        public virtual BrokersJournalEF brokersJournal { get; set; }
        #endregion
    }
}
