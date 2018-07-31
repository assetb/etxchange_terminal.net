using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "otherdocs")]
    public class OtherDocsEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("company")]
        public int companyid { get; set; }

        [ForeignKey("broker")]
        public int brokerid { get; set; }

        public DateTime createdate { get; set; }

        [ForeignKey("documenttype")]
        public int documenttypeid { get; set; }

        public string number { get; set; }
        public bool inpost { get; set; }
        public int listservnumber { get; set; }
        public int quantity { get; set; }
        #endregion

        #region Foreign objects
        public virtual CompanyEF company { get; set; }
        public virtual BrokerEF broker { get; set; }
        public virtual DocumentTypeEF documenttype { get; set; }
        #endregion
    }
}
