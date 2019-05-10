using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Addresses
{
    public class AddressBalancePair
    {
        public BitcoinAddress Address { get; set; }
        public Money Balance { get; set; }
    }
}
