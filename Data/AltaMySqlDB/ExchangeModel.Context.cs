﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ExchangeEntities : DbContext
    {
        public ExchangeEntities()
            : base("name=ExchangeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<applicants> applicants { get; set; }
        public virtual DbSet<auctions> auctions { get; set; }
        public virtual DbSet<brokers> brokers { get; set; }
        public virtual DbSet<companies> companies { get; set; }
        public virtual DbSet<contracts> contracts { get; set; }
        public virtual DbSet<customers> customers { get; set; }
        public virtual DbSet<lots> lots { get; set; }
        public virtual DbSet<operauction> operauction { get; set; }
        public virtual DbSet<order> order { get; set; }
        public virtual DbSet<person> person { get; set; }
        public virtual DbSet<sites> sites { get; set; }
        public virtual DbSet<users> users { get; set; }
    }
}
