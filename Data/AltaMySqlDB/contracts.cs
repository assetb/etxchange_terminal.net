//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AltaMySqlDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class contracts
    {
        public int id { get; set; }
        public string number { get; set; }
        public Nullable<System.DateTime> agreementdate { get; set; }
        public Nullable<System.DateTime> terminationdate { get; set; }
        public Nullable<int> bankid { get; set; }
        public Nullable<int> scanid { get; set; }
        public Nullable<System.DateTime> updatedate { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<int> typesid { get; set; }
        public int currencyid { get; set; }
    
        public virtual brokers brokers { get; set; }
        public virtual companies companies { get; set; }
        public virtual sites sites { get; set; }
    }
}
