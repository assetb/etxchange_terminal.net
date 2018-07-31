using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.tables {
    [Table(name:"documenttypes")]
    public class DocumentTypeEF {
        #region Columns
        [Key]
        public int id { get; set; }
        public string description_ru { get; set; }
        public string description_en { get; set; }
        #endregion
    }
}
