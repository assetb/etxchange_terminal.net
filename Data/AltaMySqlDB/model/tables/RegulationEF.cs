using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "regulations")]
    public class RegulationEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public DateTime opendate { get; set; }

        public DateTime closedate { get; set; }

        public DateTime applydate { get; set; }

        public DateTime applydeadline { get; set; }

        public DateTime applicantsdeadline { get; set; }

        public DateTime provisiondeadline { get; set; }
        #endregion
    }
}
