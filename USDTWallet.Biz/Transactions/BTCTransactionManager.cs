using NBitcoin;
using NBitcoin.JsonConverters;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Operators;
using USDTWallet.Dao.Transaction;
using USDTWallet.Models.Enums.Transaction;
using USDTWallet.Models.Models.Transactions;
using USDTWallet.Models.Models.Transfer;

namespace USDTWallet.Biz.Transactions
{
    public class BTCTransactionManager : BizBase
    {
        private TransactionDao TransactionDao { get; set; }
        public BTCTransactionManager(TransactionDao txDao)
        {
            this.TransactionDao = txDao;
        }

        public async Task<UnsignTransactionResult> CreateUnsignedTransaction(BTCTransferVM transferInfo)
        {
            var allUnspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FromAddress);

            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(transferInfo.ToAddress, network);
            var change = BitcoinAddress.Create(transferInfo.ChangeAddress, network);

            var buildInfo = this.Build(to, change, allUnspentCoins, transferInfo.Amount, transferInfo.EstimateFeeRate);
            
            var txInfo = new BaseTransactionInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                TransactionId = buildInfo.Transaction.GetHash().ToString(),
                TransactionType = (short)TransactionType.BTC,
                FromAddress = transferInfo.FromAddress,
                ToAddress = transferInfo.ToAddress,
                ChangeAddress = transferInfo.ChangeAddress,
                FeeAddress = transferInfo.FromAddress,
                FeeRate = transferInfo.EstimateFeeRate.SatoshiPerByte,
                EstimateSize = buildInfo.TransactionSize,
                Amount = transferInfo.Amount.ToDecimal(MoneyUnit.BTC),
                IsSigned = false,
                CreateDate = DateTime.Now
            };
            
            TransactionDao.Create(txInfo);

            var result = new UnsignTransactionResult
            {
                OperationId = txInfo.Id,
                Transaction = buildInfo.Transaction,
                ToSpentCoins = buildInfo.InputCoins
            };

            return result;
        }

        public BTCTransactionBuildResult Build(BitcoinAddress toAddress, 
                                               BitcoinAddress changeAddress,
                                               List<UnspentCoin> unspentCoins,
                                               Money amount, 
                                               FeeRate estimateFeeRate)
        {
            var unspentTotalAmount = unspentCoins.Select(o => o.AsCoin()).Select(o => o.Amount).Sum();
            if (unspentTotalAmount < amount)
                throw new WTException(ExceptionCode.InsufficientBTC, "发送地址没有足够的BTC支付");

            var total = amount;
            var coinTotalAmount = Money.Zero;

            TransactionBuilder builder = null;
            Transaction tx = null;
            List<Coin> coins = null;
            int size = 0;
            Money fee = null;

            do
            {
                coins = BTCOperator.Instance.SelectCoinsToSpent(unspentCoins, total);
                coinTotalAmount = coins.Select(o => o.Amount).Sum();

                builder = NetworkOperator.Instance.Network.CreateTransactionBuilder();
                tx = builder.AddCoins(coins)
                            .Send(toAddress, amount)
                            .SetChange(changeAddress)
                            .BuildTransaction(false);

                size = builder.EstimateSize(tx);
                var feeAmount = size * estimateFeeRate.SatoshiPerByte;
                fee = new Money(feeAmount, MoneyUnit.Satoshi);

                total = amount + fee;

                if (total > unspentTotalAmount)
                    throw new WTException(ExceptionCode.InsufficientBTC, "没有足够的费用创建交易");
            }
            while (total > coinTotalAmount);

            builder.SendFees(fee);
            tx = builder.BuildTransaction(false);

            return new BTCTransactionBuildResult
            {
                Transaction = tx,
                TransactionSize = size,
                Fee = fee,
                InputCoins = coins
            };
        }
    }
}
