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
    public class USDTTransactionManager : BizBase
    {
        public async Task<UnsignTransactionResult> BuildUnsignedTransaction(USDTTransferVM transferInfo)
        {
            var unspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FromAddress);
            // 暂时不处理手续费
            var spentCoins = unspentCoins.Take(1).Select(o => o.AsCoin()).ToList();
            var tx = USDTOperator.Instance.BuildUnsignedTx(transferInfo.FromAddress, transferInfo.ToAddress, transferInfo.ChangeAddress,
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
