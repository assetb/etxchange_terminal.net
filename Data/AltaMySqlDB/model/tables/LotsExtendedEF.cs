using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "lotsextended")]
    public class LotsExtendedEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("lot")]
        public int? lotid { get; set; }

        public int serialnumber { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public decimal sum { get; set; }
        public string country { get; set; }
        public string techspec { get; set; }
        public string terms { get; set; }
        public string paymentterm { get; set; }
        public int? dks { get; set; }
        public string contractnumber { get; set; }
        public decimal? endprice { get; set; }
        public decimal? endsum { get; set; }
        public string marka { get; set; }
        public string gost { get; set; }
        public string codeTNVD { get; set; }
        public string factory { get; set; }
        #endregion

        #region Foreigen Objects
        public virtual LotEF lot { get; set; }
        #endregion
    }
}
