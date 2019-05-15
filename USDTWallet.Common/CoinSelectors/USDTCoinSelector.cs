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

        public USDTCoinSelector(OutPoint fromOutpoint)
        {
            this.FromOutpoint = fromOutpoint;
        }

        public IEnumerable<ICoin> Select(IEnumerable<ICoin> coins, IMoney target)
        {
            var selector = new DefaultCoinSelector();
            var selectedCoins = selector.Select(coins, target).ToList();

            if (!selectedCoins.Any(o => o.Outpoint == FromOutpoint))
            {
                var fromCoin = coins.Single(o => o.Outpoint == this.FromOutpoint);
                var fromCoinAmount = fromCoin.Amount as Money;
                var selectedAmount = selectedCoins.Select(o => o.Amount).Cast<Money>().Sum();

                if (fromCoinAmount >= selectedAmount)
                    selectedCoins = new List<ICoin> { fromCoin };
                else
                    selectedCoins.Insert(0, fromCoin);
            }

            return selectedCoins;
        }
    }
}
