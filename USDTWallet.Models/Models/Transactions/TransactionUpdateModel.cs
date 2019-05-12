using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Transactions
{
    public class TransactionUpdateModel
    {
        public string TransactionId { get; set; }
        public Nullable<long> Confirmations { get; set; }
        public string BlockHash { get; set; }
        public Nullable<System.DateTime> BlockTime { get; set; }
    }
}
