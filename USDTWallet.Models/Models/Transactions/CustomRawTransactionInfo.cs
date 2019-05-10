using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Transactions
{
    public class CustomRawTransactionInfo
    {
        public Transaction Transaction { get; set; }
        public uint256 TransactionId { get; set; }
        public uint256 Hash { get; set; }
        public uint Size { get; set; }
        public uint VirtualSize { get; set; }
        public uint Version { get; set; }
        public LockTime LockTime { get; set; }
        public uint256 BlockHash { get; set; }
        public uint Confirmations { get; set; }
        public DateTimeOffset? TransactionTime { get; set; }
        public DateTimeOffset? BlockTime { get; set; }
    }
}
