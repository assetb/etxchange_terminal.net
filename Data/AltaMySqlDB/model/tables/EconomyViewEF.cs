using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
  [Table(name: "economyGraph")]
  public class EconomyViewEF
  {
    #region Columns
    
    [Column("aucID")]
    public int auctionId { get; set; }
    
    [Key]
    public int lotId { get; set; }
    
    
    public string aucNumber { get; set; }
    
    
    public int brokerid { get; set; }
    
    
    public int customerid { get; set; }
    
    
    public int typeid { get; set; }

    public DateTime date { get; set; }

    public int siteid { get; set; }
    
    public int statusid { get; set; }

    [ForeignKey("supplier")]
    public int supplierid { get; set; }
    
    public string lotNumber { get; set; }

    public string description { get; set; }
    
    public decimal amount { get; set; }

    public string unit { get; set; }   

    public decimal startprice { get; set; }

    public decimal finalprice { get; set; }

    public decimal unitprice { get; set; }


        #endregion

        public virtual SupplierEF supplier { get; set; }
    }
}
