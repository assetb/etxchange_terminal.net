using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "envelopcontent")]
    public class EnvelopContentEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("otherdoc")]
        public int otherdocid { get; set; }

        [ForeignKey("listserv")]
        public int listservid { get; set; }

        [ForeignKey("envelop")]
        public int envelopid { get; set; }
        #endregion

        #region Foreign objects
        public virtual OtherDocsEF otherdoc { get; set; }
        public virtual ListServEF listserv { get; set; }
        public virtual EnvelopEF envelop { get; set; }
        #endregion
    }
}
