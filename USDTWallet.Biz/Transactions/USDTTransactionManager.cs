using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.CoinSelectors;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Operators;
using USDTWallet.Dao.Transaction;
using USDTWallet.Models.Enums.Transaction;
using USDTWallet.Models.Models.Transactions;
using USDTWallet.Models.Models.Transfer;

namespace USDTWallet.Biz.Transactions
{
    public class USDTTransactionManager : BizBase
    {
        private TransactionDao TransactionDao { get; set; }
        public USDTTransactionManager(TransactionDao txDao)
        {
            this.TransactionDao = txDao;
        }

        public async Task<UnsignTransactionResult> BuildUnsignedTransaction(USDTTransferVM transferInfo)
        {
            var unspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FromAddress);
            var sentBTC = new Money(USDTOperator.SentBTCPerTx, MoneyUnit.BTC);
            var fromSpentCoin = unspentCoins.Where(o => o.Amount >= sentBTC).OrderByDescending(o => o.Amount)
                                             .Select(o => o.AsCoin()).FirstOrDefault();
            if (null == fromSpentCoin)
                throw new WTException(ExceptionCode.insufficientBTC, "发送地址没有足够的BTC：至少需要：" + USDTOperator.SentBTCPerTx);
            
            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(transferInfo.ToAddress, network);
            var change = BitcoinAddress.Create(transferInfo.FeeAddress, network);

            var feeUnspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FeeAddress);


            var totalBTC = Money.Parse("0");
            var coinAmount = Money.Parse("0");

            TransactionBuilder builder = null;
            Transaction tx = null;
            List<Coin> coins = null;
            int size = 0;
            Money fee = null;

            do
            {
                builder = network.CreateTransactionBuilder();

                var feeSpentCoins = BTCOperator.Instance.SelectCoinsToSpent(feeUnspentCoins, totalBTC);
                coins = feeSpentCoins.Concat(new List<Coin> { fromSpentCoin }).ToList();

                tx = await this.BuildUnsignedTx(builder, transferInfo.FromAddress, transferInfo.ToAddress, transferInfo.FeeAddress,
                                                sentBTC, transferInfo.Amount, transferInfo.EstimateFeeRate, fromSpentCoin, feeSpentCoins);

                size = builder.EstimateSize(tx);
                var feeAmount = size * transferInfo.EstimateFeeRate.SatoshiPerByte;
                fee = new Money(feeAmount, MoneyUnit.Satoshi);

                totalBTC = sentBTC + fee;
                coinAmount = coins.Select(o => o.Amount).Sum();

            }
            while (totalBTC > coinAmount);

            var opReturnOutput = tx.Outputs.Single(o => o.ScriptPubKey.ToString().StartsWith("OP_RETURN"));
            var originalOrderdOutput = tx.Clone().Outputs;
            
            builder.SetCoinSelector(new AllCoinSelector());
            builder.AddCoins(coins).SendFees(fee);
            tx = builder.BuildTransaction(false);

            tx.Outputs.Insert(1, opReturnOutput);
            var sssize = builder.EstimateSize(tx);

            var result = new UnsignTransactionResult
            {
                Transaction = tx,
                ToSpentCoins = coins
            };


            var txInfo = new BaseTransactionInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                TransactionId = tx.GetHash().ToString(),
                TransactionType = (short)TransactionType.USDT,
                FromAddress = transferInfo.FromAddress,
                ToAddress = transferInfo.ToAddress,
                ChangeAddress = transferInfo.FeeAddress,
                FeeAddress = transferInfo.FeeAddress,
                FeeRate = transferInfo.EstimateFeeRate.SatoshiPerByte,
                EstimateSize = size,
                Amount = transferInfo.Amount.ToDecimal(MoneyUnit.BTC),
                IsSigned = false,
                CreateDate = DateTime.Now
            };

            TransactionDao.Create(txInfo);

            return result;

        }


        private async Task<Transaction> BuildUnsignedTx(TransactionBuilder builder, string fromAddress, string toAddress, string feeAddress, 
                                                       Money btcAmount, Money usdtAmount, FeeRate feeRate, Coin fromCoin, List<Coin> feeCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(toAddress, network);
            var feeAddr = BitcoinAddress.Create(feeAddress, network);

            var tx = builder.AddCoins(fromCoin)
                            .AddCoins(feeCoins)
                            .Send(to, btcAmount)
                            .SetChange(feeAddr)
                            .SetCoinSelector(new USDTCoinSelector(fromCoin.Outpoint))
                            .BuildTransaction(false);

            var detail = tx.ToString();

            var amountPayload = await USDTOperator.Instance.CreatePayloadSimpleSend(usdtAmount);
            var opreturn = await USDTOperator.Instance.GenerateOpRetrun(tx.ToHex(), amountPayload);
            var receiveRef = await USDTOperator.Instance.GenerateReference(opreturn, toAddress);

            var finalTx = Transaction.Parse(receiveRef, network);
            this.KeepDustOutputUnique(finalTx);

            var finalDetail = finalTx.ToString();

            return finalTx;
        }


        private void KeepDustOutputUnique(Transaction tx)
        {
            var dust = new Money(USDTOperator.SentBTCPerTx, MoneyUnit.BTC);

            var dupIndexs = new List<uint>();
            for(uint i = 0; i < tx.Outputs.Count; i++)
            {
                if (tx.Outputs[i].Value == dust)
                    dupIndexs.Add(i);
            }

            if (dupIndexs.Count > 0)
                dupIndexs.RemoveAt(dupIndexs.Count - 1);

            var originalOutputs = tx.Clone().Outputs;
            tx.Outputs.Clear();

            for(uint i = 0; i < originalOutputs.Count; i++)
            {
                if (dupIndexs.Any(idx => idx == i))
                    continue;

                var output = originalOutputs[i];
                tx.Outputs.Add(output);
            }
        }


    }
}
