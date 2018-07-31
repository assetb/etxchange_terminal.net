using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "envelops")]
    public class EnvelopEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("company")]
        public int companyid { get; set; }

        public string code { get; set; }

        [ForeignKey("listserv")]
        public int listservid { get; set; }
        #endregion

        #region Foreign objects
        public virtual CompanyEF company { get; set; }
        public virtual ListServEF listserv { get; set; }
        #endregion
    }
}
