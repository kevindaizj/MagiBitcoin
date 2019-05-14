using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Transactions
{
    public class CalculatedOmniFeeInfo
    {
        public Money Fee { get; set; }
        public List<Coin> InputCoins { get; set; }
        public int TransactionSize { get; set; }
    }
}
