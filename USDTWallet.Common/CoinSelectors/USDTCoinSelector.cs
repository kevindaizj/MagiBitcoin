using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common.Operators;

namespace USDTWallet.Common.CoinSelectors
{
    public class USDTCoinSelector : ICoinSelector
    {
        private OutPoint FromOutpoint { get; set; }

        private Money DustAmount { get; set; }

        public USDTCoinSelector(OutPoint fromOutpoint, Money dustAmount)
        {
            this.FromOutpoint = fromOutpoint;
            this.DustAmount = dustAmount;
        }

        public IEnumerable<ICoin> Select(IEnumerable<ICoin> coins, IMoney target)
        {
            var defaultSelector = new DefaultCoinSelector();

            var fromCoin = coins.FirstOrDefault(o => o.Outpoint == this.FromOutpoint);

            if (null != fromCoin &&
                fromCoin.Amount.CompareTo(target) >= 0 &&
                DustAmount.CompareTo(target) == 0)
            {
                return coins;
            }
            
            return new DefaultCoinSelector().Select(coins, target);
        }
    }
}
