﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace USDTWallet.DataContext.Context
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WalletContext : DbContext
    {
        public WalletContext()
            : base("name=WalletContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BASE_ADDRESS> BASE_ADDRESS { get; set; }
        public virtual DbSet<BASE_WALLET> BASE_WALLET { get; set; }
        public virtual DbSet<BASE_TRANSACTION> BASE_TRANSACTION { get; set; }
    }
}
