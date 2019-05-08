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

            var feeUnspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FeeAddress);


            var totalBTC = Money.Parse("0");
            var coinAmount = Money.Parse("0");

            TransactionBuilder builder = null;
            Transaction tx = null;
            List<Coin> coins = null;
            Money fee = null;

            do
            {
                builder = network.CreateTransactionBuilder();

                coins = BTCOperator.Instance.SelectCoinsToSpent(feeUnspentCoins, totalBTC);
                coins.Add(fromSpentCoin);

                tx = await this.BuildUnsignedTx(builder, transferInfo.FromAddress, transferInfo.ToAddress, transferInfo.FeeAddress,
                                                minFee, transferInfo.Amount, coins);

                var size = builder.EstimateSize(tx);
                var feeAmount = size * transferInfo.EstimateFeeRate.SatoshiPerByte;
                fee = new Money(feeAmount, MoneyUnit.Satoshi);

                totalBTC = minFee + fee;
                coinAmount = coins.Select(o => o.Amount).Sum();

            }
            while (totalBTC > coinAmount);
            
            builder.SendFees(fee);
            tx = builder.BuildTransaction(false);

            var result = new UnsignTransactionResult
            {
                Transaction = tx,
                ToSpentCoins = coins
            };

            return result;

        }


        public async Task<Transaction> BuildUnsignedTx(TransactionBuilder builder, string fromAddress, string toAddress, string changeAddress, 
                                                       Money btcAmount, Money usdtAmount, List<Coin> spentCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(toAddress, network);
            var change = BitcoinAddress.Create(changeAddress, network);
            
            var tx = builder.AddCoins(spentCoins)
                            .Send(to, btcAmount)
                            .SetChange(change)
                            .BuildTransaction(false);

            var detail = tx.ToString();

            var amountPayload = await USDTOperator.Instance.CreatePayloadSimpleSend(usdtAmount);
            var opreturn = await USDTOperator.Instance.GenerateOpRetrun(tx.ToHex(), amountPayload);
            var receiveRef = await USDTOperator.Instance.GenerateReference(opreturn, toAddress);

            var finalTx = Transaction.Parse(receiveRef, network);

            return finalTx;
        }



    }
}
