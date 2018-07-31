using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "rateslist")]
    public class RatesListEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        [ForeignKey("contract")]
        public int contractid { get; set; }

        [ForeignKey("site")]
        public int siteid { get; set; }
        #endregion

        #region Foreign Objects
        public virtual ContractEF contract { get; set; }
        public virtual SiteEF site { get; set; }
        #endregion

        #region Foreign Collections
        public virtual ICollection<RateEF> rates { get; set; }
        #endregion
    }
}
