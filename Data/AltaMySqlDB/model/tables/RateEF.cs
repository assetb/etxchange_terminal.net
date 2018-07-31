using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "rates")]
    public class RateEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public decimal transaction { get; set; }

        public decimal percent { get; set; }

        [MaxLength(60)]
        public string description { get; set; }

        [ForeignKey("ratesList")]
        public int rateslistid { get; set; }
        #endregion

        #region Foreign Objects
        public virtual RatesListEF ratesList { get; set; }
        #endregion
    }
}
