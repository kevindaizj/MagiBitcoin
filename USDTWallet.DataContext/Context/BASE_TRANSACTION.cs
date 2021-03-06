//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class BASE_TRANSACTION
    {
        public string ID { get; set; }
        public string TRANSACTION_ID { get; set; }
        public int TRANSACTION_TYPE { get; set; }
        public string FROM_ADDRESS { get; set; }
        public string TO_ADDRESS { get; set; }
        public string CHANGE_ADDRESS { get; set; }
        public string FEE_ADDRESS { get; set; }
        public decimal FEE_RATE { get; set; }
        public int ESTIMATE_SIZE { get; set; }
        public decimal AMOUNT { get; set; }
        public bool IS_SIGNED { get; set; }
        public string BLOCK_HASH { get; set; }
        public Nullable<long> CONFIRMATIONS { get; set; }
        public Nullable<System.DateTime> BLOCK_TIME { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
    }
}
