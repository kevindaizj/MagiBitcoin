using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Operators;
using USDTWallet.Models.Models.Transfer;

namespace USDTWallet.Biz.Transactions
{
    public class BTCTransactionManager : BizBase
    {
        public async Task<Transaction> BuildUnsignedTransaction(BTCTransferVM transferInfo)
        {
            var unspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FromAddress);
            var spentCoins = BTCOperator.Instance.SelectCoinsToSpent(unspentCoins, transferInfo.Amount);
            return BTCOperator.Instance.BuildUnsignedTx(transferInfo.FromAddress, transferInfo.ToAddress, transferInfo.ChangeAddress,
                                                 transferInfo.Amount, transferInfo.EstimateFeeRate, spentCoins);
        }
    }
}
