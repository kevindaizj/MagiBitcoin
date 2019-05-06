using NBitcoin;
using NBitcoin.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Operators;
using USDTWallet.Models.Models.Transactions;
using USDTWallet.Models.Models.Transfer;

namespace USDTWallet.Biz.Transactions
{
    public class BTCTransactionManager : BizBase
    {
        public async Task<UnsignTransactionResult> BuildUnsignedTransaction(BTCTransferVM transferInfo)
        {
            var unspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FromAddress);
            var spentCoins = BTCOperator.Instance.SelectCoinsToSpent(unspentCoins, transferInfo.Amount);
            var tx = BTCOperator.Instance.BuildUnsignedTx(transferInfo.FromAddress, transferInfo.ToAddress, transferInfo.ChangeAddress,
                                                 transferInfo.Amount, transferInfo.EstimateFeeRate, spentCoins);

            var result = new UnsignTransactionResult
            {
                Transaction = tx,
                ToSpentCoins = spentCoins
            };

            return result;
        }
    }
}
