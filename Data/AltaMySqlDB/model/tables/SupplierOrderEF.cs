using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    public class SupplierOrderEF
    {
        #region Columns
        public int id { get; set; }
        public int? supplierid { get; set; }
        public int? auctionid { get; set; }
        public int? contractid { get; set; }
        public int? statusid { get; set; }
        public DateTime date { get; set; }
        public int? fileListId { get; set; }
        public string comments { get; set; }        
        #endregion

        #region Foreign Objects
        public virtual StatusEF status { get; set; }
        public virtual SupplierEF supplier { get; set; }
        public virtual AuctionEF auction { get; set; }
        public virtual ContractEF contract { get; set; }
        public virtual FilesListEF fileList { get; set; }
        public virtual List<LotEF> lots { get; set; }
        #endregion

        #region Foreign Collections
        public virtual ICollection<ApplicantEF> applicants { get; set; }
        #endregion
    }
}
