using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables {
    [Table(name: "requesteddocs")]
    public class RequestedDocEF {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("supplierorder")]
        public int supplierorderid { get; set; }

        public string name { get; set; }
        #endregion

        #region Foreign
        public virtual SupplierOrderEF supplierorder { get; set; }
        #endregion
    }
}
