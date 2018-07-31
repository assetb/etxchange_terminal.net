using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.tables {
    [Table(name: "archive_numbers")]
    public class ArchiveNumberEF {
        #region Columns
        [Key]
        public int Id { get; set; }
        public int Year { get; set; }

        [ForeignKey("Broker")]
        public int BrokerId { get; set; }

        [ForeignKey("Case")]
        public int CaseId { get; set; }

        [ForeignKey("Volume")]
        public int VolumeId { get; set; }
        public int DocumentNumber { get; set; }
        #endregion

        #region Foreign Objects
        public virtual BrokerEF Broker { get; set; }
        public virtual CaseEF Case { get; set; }
        public virtual VolumeEF Volume { get; set; }
        #endregion

        #region Foreign Collections
        public virtual ICollection<DocumentEF> documents { get; set; }
        #endregion
    }
}
