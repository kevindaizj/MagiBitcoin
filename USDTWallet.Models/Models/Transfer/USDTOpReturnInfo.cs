using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Transfer
{
    public class USDTOpReturnInfo
    {
        public uint TransactionVersion { get; set; }
        public uint TransactionType { get; set; }
        public uint CurrencyIdentifier { get; set; }
        public Money Amount { get; set; }
    }
}
