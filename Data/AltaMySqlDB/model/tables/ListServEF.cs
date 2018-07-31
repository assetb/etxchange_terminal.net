using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "listserv")]
    public class ListServEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("broker")]
        public int brokerid { get; set; }

        public DateTime createdate { get; set; }
        public int number { get; set; }

        [ForeignKey("status")]
        public int statusid { get; set; }

        public DateTime? departuredate { get; set; }
        #endregion

        #region Foreign objects
        public virtual BrokerEF broker { get; set; }
        public virtual StatusEF status { get; set; }
        #endregion
    }
}
