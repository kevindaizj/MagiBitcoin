using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Exceptions;
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
            var minFee = new Money(USDTOperator.MinTxFee, MoneyUnit.BTC);
            var fromSpentCoin = unspentCoins.Where(o => o.Amount >= minFee).OrderByDescending(o => o.Amount)
                                             .Select(o => o.AsCoin()).FirstOrDefault();
            if (null == fromSpentCoin)
                throw new WTException(ExceptionCode.insufficientBTC, "发送地址没有足够的BTC：至少需要：" + USDTOperator.MinTxFee);
            
            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(transferInfo.ToAddress, network);
            var change = BitcoinAddress.Create(transferInfo.FeeAddress, network);

            var allUnspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FeeAddress);


            var totalBTC = new Money(minFee.Satoshi);
            var coinAmount = Money.Parse("0");

            Transaction tx = null;
            List<Coin> coins = null;
            Money fee = null;

            while (totalBTC > coinAmount)
            {
                coins = BTCOperator.Instance.SelectCoinsToSpent(allUnspentCoins, totalBTC);
                coins.Add(fromSpentCoin);
                
                tx = await USDTOperator.Instance.BuildUnsignedTx(transferInfo.FromAddress, transferInfo.ToAddress, transferInfo.FeeAddress,
                                                        transferInfo.Amount, transferInfo.EstimateFeeRate, coins);

                var builder = network.CreateTransactionBuilder();
                var size = builder.EstimateSize(tx);
                var feeAmount = size * transferInfo.EstimateFeeRate.SatoshiPerByte;
                fee = new Money(feeAmount, MoneyUnit.Satoshi);

                totalBTC = fee;
                coinAmount = coins.Select(o => o.Amount).Sum();
            }



            var result = new UnsignTransactionResult
            {
                Transaction = tx,
                ToSpentCoins = coins
            };

            return result;

        }


        
    }
}
