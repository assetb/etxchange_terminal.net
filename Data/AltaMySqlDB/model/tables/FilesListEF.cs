using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name:"fileslist")]
    public class FilesListEF {
        #region Columns
        [Key]
        public int id { get; set; }
        public string description { get; set; }
        #endregion

        public virtual ICollection<DocumentEF> documents { get; set; }
    }
}
