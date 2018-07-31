using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables {
    [Table(name: "volumes")]
    public class VolumeEF {
        #region Columns
        [Key]
        public int Id { get; set; }

        [ForeignKey("Case")]
        public int CaseId { get; set; }
        public string Name { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }
        #endregion

        #region Foreign
        public virtual CaseEF Case { get; set; }
        public virtual StatusEF Status { get; set; }
        #endregion
    }
}