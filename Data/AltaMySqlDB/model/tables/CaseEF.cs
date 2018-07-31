using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.tables {
    [Table(name: "cases")]
    public class CaseEF {
        #region Columns
        [Key]
        public int Id { get; set; }
        public int Year { get; set; }

        [ForeignKey("Broker")]
        public int BrokerId { get; set; }
        public string Name { get; set; }
        #endregion

        #region Foreign
        public virtual BrokerEF Broker { get; set; }
        #endregion
    }
}
