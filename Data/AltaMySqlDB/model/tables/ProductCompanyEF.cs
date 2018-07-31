using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table("products_company_view")]
    public class ProductCompanyEF
    {
        [Key]
        public int goods_company_id { get; set; }

        public int id { get; set; }
        [ForeignKey("company")]
        public int companyId { get; set; }
        [Column("goodid")]
        public int productId { get; set; }

        public string description { get; set; }
        [Column("document_id"), ForeignKey("document")]
        public int? documentId { get; set; }
        public string name { get; set; }
        [ForeignKey("unit")]
        public int? unitId { get; set; }      
        
        public virtual CompanyEF company { get; set; }
        public virtual UnitEF unit { get; set; }
        public virtual DocumentEF document { get; set; }
    }
}
