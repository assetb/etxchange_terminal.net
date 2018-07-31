using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "emailsettings")]
    public class EmailSettingEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public string email { get; set; }
        public string pass { get; set; }

        [ForeignKey("persons")]
        public int? personid { get; set; }
        #endregion


        #region Foreign
        public virtual PersonEF persons { get; set; }
        #endregion
    }
}
