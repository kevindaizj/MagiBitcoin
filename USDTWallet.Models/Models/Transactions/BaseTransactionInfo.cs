using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Transactions
{
    public class BaseTransactionInfo
    {
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public int TransactionType { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string ChangeAddress { get; set; }
        public string FeeAddress { get; set; }
        public decimal FeeRate { get; set; }
        public int EstimateSize { get; set; }
        public decimal Amount { get; set; }
        public bool IsSigned { get; set; }
        public string BlockHash { get; set; }
        public Nullable<long> Confirmations { get; set; }
        public Nullable<System.DateTime> BlockTime { get; set; }
        public System.DateTime CreateDate { get; set; }

    }
}
