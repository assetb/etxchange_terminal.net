using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "companies")]
    public class CompanyEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [MaxLength(254)]
        public string name { get; set; }

        [MaxLength(20)]
        public string bin { get; set; }

        public int? kbe { get; set; }

        [ForeignKey("countries")]
        public int? countryid { get; set; }

        [MaxLength(254)]
        public string email { get; set; }

        public string telephone { get; set; }
        public string fax { get; set; }
        public string addresslegal { get; set; }
        public string addressactual { get; set; }

        [MaxLength(15)]
        public string postcode { get; set; }

        [MaxLength(50)]
        public string director { get; set; }

        [MaxLength(50)]
        public string directorpowers { get; set; }

        public string comments { get; set; }
        public DateTime updatedate { get; set; }
        public DateTime createdate { get; set; }

        [MaxLength(50)]
        public string iik { get; set; }

        [MaxLength(20)]
        public string bik { get; set; }

        public string govregnumber { get; set; }
        public DateTime? govregdate { get; set; }

        [ForeignKey("fileList")]
        public int filesListId { get; set; }
        #endregion

        #region Foreign Objects
        public virtual CountryEF countries { get; set; }
        public virtual FilesListEF fileList { get; set; }

        public virtual ICollection<BankEF> banks { get; set; }
        public virtual ICollection<BrokerEF> brokers { get; set; }
        public virtual ICollection<SiteEF> sites { get; set; }
        public virtual ICollection<ContractEF> contracts { get; set; }
        #endregion

        public virtual ICollection<ProductEF> products { get; set; }
    }
}
