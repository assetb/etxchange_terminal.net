using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "scans")]
    public class ScanEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public string filename { get; set; }

        public byte[] scanfile { get; set; }

        public DateTime date { get; set; }

        [MaxLength(20)]
        public string archnumber { get; set; }
        #endregion
    }
}
