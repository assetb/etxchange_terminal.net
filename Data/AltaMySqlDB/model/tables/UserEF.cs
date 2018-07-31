using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "users")]
    public class UserEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("role")]
        public int roleid { get; set; }
        
        [MaxLength(45)]
        public string login { get; set; }

        [MaxLength(45)]
        public string pass { get; set; }

        [ForeignKey("person")]
        public int? personid { get; set; }

        public bool isactive { get; set; }
        #endregion

        #region Foreign Objects
        public virtual RoleEF role { get; set; }

        public virtual PersonEF person { get; set; }
        #endregion
    }
}
