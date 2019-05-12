using NBitcoin;
using NBitcoin.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
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

        public async Task<UnsignTransactionResult> BuildUnsignedTransaction(BTCTransferVM transferInfo)
        {
            var allUnspentCoins = await BTCOperator.Instance.ListUnspentAsync(transferInfo.FromAddress);

            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(transferInfo.ToAddress, network);
            var change = BitcoinAddress.Create(transferInfo.ChangeAddress, network);

            var total = transferInfo.Amount;
            var coinAmount = Money.Parse("0");

            TransactionBuilder builder = null;
            Transaction tx = null;
            List<Coin> coins = null;
            int size = 0;
            Money fee = null;

            do
            {
                coins = BTCOperator.Instance.SelectCoinsToSpent(allUnspentCoins, total);
                builder = network.CreateTransactionBuilder();
                tx = builder.AddCoins(coins)
                                .Send(to, transferInfo.Amount)
                                .SetChange(change)
                                .BuildTransaction(false);

                size = builder.EstimateSize(tx);
                var feeAmount = size * transferInfo.EstimateFeeRate.SatoshiPerByte;
                fee = new Money(feeAmount, MoneyUnit.Satoshi);
               
                total = transferInfo.Amount + fee;
                coinAmount = coins.Select(o => o.Amount).Sum();
            }
            while (total > coinAmount);

            builder.SendFees(fee);
            tx = builder.BuildTransaction(false);


            var txInfo = new BaseTransactionInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                TransactionId = tx.GetHash().ToString(),
                TransactionType = (short)TransactionType.BTC,
                FromAddress = transferInfo.FromAddress,
                ToAddress = transferInfo.ToAddress,
                ChangeAddress = transferInfo.ChangeAddress,
                FeeAddress = transferInfo.FromAddress,
                FeeRate = transferInfo.EstimateFeeRate.SatoshiPerByte,
                EstimateSize = size,
                Amount = transferInfo.Amount.ToDecimal(MoneyUnit.BTC),
                IsSigned = false,
                CreateDate = DateTime.Now
            };

            TransactionDao.Create(txInfo);

            var result = new UnsignTransactionResult
            {
                Transaction = tx,
                ToSpentCoins = coins
            };

            return result;
        }
    }
}
