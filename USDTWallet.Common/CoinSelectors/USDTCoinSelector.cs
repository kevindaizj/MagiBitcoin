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
        private OutPoint DustOut { get; set; }

        public USDTCoinSelector(OutPoint dustOut)
        {
            this.DustOut = dustOut;
        }

        public IEnumerable<ICoin> Select(IEnumerable<ICoin> coins, IMoney target)
        {
            var dustAmount = USDTOperator.SentBTCPerTx;
            var defaultSelector = new DefaultCoinSelector();

            var dust = coins.FirstOrDefault(o => o.Outpoint == this.DustOut);
            if (null == dust)
                return defaultSelector.Select(coins, target);

            if (dust.Amount.CompareTo(target) >= 0)
                return coins;
            
            return defaultSelector.Select(coins, target);
        }
    }
}
