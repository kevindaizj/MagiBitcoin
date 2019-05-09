using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Common.CoinSelectors
{
    public class AllCoinSelector : ICoinSelector
    {
        public IEnumerable<ICoin> Select(IEnumerable<ICoin> coins, IMoney target)
        {
            return coins;
        }
    }
}
