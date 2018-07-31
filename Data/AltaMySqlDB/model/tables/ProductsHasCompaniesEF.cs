using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.tables
{
    [Table("goodscompanies")]
    public class ProductsHasCompaniesEF
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("good")]
        public int? goodid { get; set; }
        [ForeignKey("company")]
        public int? companyid { get; set; }

        public virtual ProductEF good { get; set; }
        public virtual CompanyEF company { get; set; }
    }
}
