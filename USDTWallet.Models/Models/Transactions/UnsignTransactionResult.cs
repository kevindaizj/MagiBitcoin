using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Transactions
{
    public class UnsignTransactionResult
    {
        public Transaction Transaction { get; set; }
        public List<Coin> ToSpentCoins { get; set; }
    }
}
