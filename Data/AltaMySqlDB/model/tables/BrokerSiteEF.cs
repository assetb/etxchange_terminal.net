using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "brokersites")]
    public class BrokerSiteEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("broker")]
        public int brokerid { get; set; }

        [ForeignKey("site")]
        public int siteid { get; set; }

        [MaxLength(45)]
        public string code { get; set; }
        #endregion

        #region Foreign Objects
        public virtual BrokerEF broker { get; set; }
        public virtual SiteEF site { get; set; }
        #endregion
    }
}
