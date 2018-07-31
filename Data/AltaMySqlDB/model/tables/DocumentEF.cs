using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{

    [Table(name: "documents")]
    public class DocumentEF {
        #region Columns
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public string number { get; set; }
        public string extension { get; set; }
        public DateTime date { get; set; }
        public string description { get; set; }

        [ForeignKey("documentType")]
        public int documenttypeid { get; set; }

        [ForeignKey("filesList")]
        public int? fileslistid { get; set; }

        [ForeignKey("site")]
        public int? siteid { get; set; }

        [ForeignKey("fileSection")]
        public int? filesectionid { get; set; }

        [ForeignKey("archiveNumber")]
        public int? archiveNumberId { get; set; }
        #endregion

        #region Foreign Keys
        public virtual DocumentTypeEF documentType { get; set; }
        public virtual FilesListEF filesList { get; set; }
        public virtual SiteEF site { get; set; }
        public virtual FileSectionEF fileSection { get; set; }
        public virtual ArchiveNumberEF archiveNumber { get; set; }
        #endregion
    }
}
