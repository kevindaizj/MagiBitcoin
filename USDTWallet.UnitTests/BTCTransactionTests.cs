using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using NBitcoin.RPC;
using USDTWallet.Biz.Transactions;
using USDTWallet.UnitTests.Tools;

namespace USDTWallet.UnitTests
{
    [TestClass]
    public class BTCTransactionTests
    {
        private BTCTransactionManager TxManager
        {
            get { return new BTCTransactionManager(new Dao.Transaction.TransactionDao()); }
        }

        private Key FromPrivKey
        {
            get { return Key.Parse("cT6LC1FGBu78cD47D9K6yT8yyrsEcQKAfeMqUuFcVub26568VMCM", Network.RegTest); }
        }

        private BitcoinAddress FromAddress
        {
            get { return FromPrivKey.GetBitcoinSecret(Network.RegTest).GetAddress(); }
        }
        private BitcoinAddress ToAddress
        {
            get { return BitcoinAddress.Create("n3uXG3c3ouZUtVZUbzCZCQomAguGxdwQcQ", Network.RegTest); }
        }

        private Key ChangePrivKey
        {
            get { return Key.Parse("cSijF7WaKNXbaTd3H2ydnHTHbB2aG4spPXH5xqwmUMCBFFjfxGnw", Network.RegTest); }
        }

        private BitcoinAddress ChangeAddress
        {
            get { return ChangePrivKey.GetBitcoinSecret(Network.RegTest).GetAddress(); }
        }

        private FeeRate EstimateFeeRate
        {
            get { return new FeeRate(Money.Parse("0.001")); }
        }


        [TestMethod]
        public void TestMethod1()
        {
            var tool = new Tool();

            var total = Money.Parse("10");
            var unspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress,  Money.Parse("10"))
            };

            var coins = unspentCoins.Select(o => o.AsCoin()).ToList();
            var amount = Money.Parse("9");

            var mockFee = total - amount;
            // two outputs (contains change output)
            var twoOutputSize = 226;
            var feeRate = new FeeRate(new Money(mockFee.Satoshi / twoOutputSize * 1000));


            var builder = this.Build(ToAddress, ChangeAddress, coins, amount, mockFee);
            var tx = builder.BuildTransaction(false);

            // one output
            var output = tx.Outputs;

            // 192
            var size = builder.EstimateSize(tx);

            // 0.84955584
            var fee = new Money(size * feeRate.SatoshiPerByte, MoneyUnit.Satoshi);



            var builder1 = this.Build(ToAddress, ChangeAddress, coins, amount, fee);
            var tx1 = builder1.BuildTransaction(false);

            // two outputs
            var output1 = tx1.Outputs;

            // 226
            var size1 = builder1.EstimateSize(tx1);

            // 0.99999802 
            var fee1 = new Money(size1 * feeRate.SatoshiPerByte, MoneyUnit.Satoshi);



            var builder3 = this.Build(ToAddress, ChangeAddress, coins, amount, fee1);
            var tx3 = builder3.BuildTransaction(false);

            // one output
            var output3 = tx3.Outputs;

            // 192
            var size3 = builder3.EstimateSize(tx3);

            // 0.84955584 
            var fee3 = new Money(size3 * feeRate.SatoshiPerByte, MoneyUnit.Satoshi);

        }


        private TransactionBuilder Build(BitcoinAddress toAddress, 
                                         BitcoinAddress changeAddress,
                                         List<Coin> coins,
                                         Money amount,
                                         Money fee)
        {
            var builder = Network.RegTest.CreateTransactionBuilder();
            var tx = builder.AddCoins(coins)
                            .Send(ToAddress, amount)
                            .SetChange(ChangeAddress)
                            .SendFees(fee);

            return builder;
        }




        [TestMethod]
        public void DetectDeadLoop()
        {
            var total = Money.Parse("10");
            var sendingAmount = Money.Parse("9");

            var unspentCoins = new List<Coin>
            {
                new Tool().NewUnspentCoin(FromAddress, total).AsCoin()
            };


            // predefined fee 
            var fee = total - sendingAmount;
            // Presupposed size which includes two outputs (contains change output)
            var preSupposedSize = 226;

            var feeRate = new FeeRate(new Money(fee.Satoshi / preSupposedSize * 1000));

            var previousFee = Money.Zero;

            while (fee != previousFee)
            {
                var builder = Network.RegTest.CreateTransactionBuilder();
                var tx = builder.AddCoins(unspentCoins)
                                .Send(ToAddress, sendingAmount)
                                .SetChange(ChangeAddress)
                                .SendEstimatedFees(feeRate)
                                .BuildTransaction(false);

                // number of outputs
                var outputCount = tx.Outputs.Count;

                // transaction size
                var size = builder.EstimateSize(tx);

                previousFee = new Money(fee.Satoshi);

                // calculated fee
                fee = new Money(size * feeRate.SatoshiPerByte, MoneyUnit.Satoshi);

            }

            //while (fee != previousFee)
            //{
            //    var builder = Network.RegTest.CreateTransactionBuilder();
            //    var tx = builder.AddCoins(unspentCoins)
            //                    .Send(ToAddress, sendingAmount)
            //                    .SetChange(ChangeAddress)
            //                    .SendFees(fee)
            //                    .BuildTransaction(false);

            //    // number of outputs
            //    var outputCount = tx.Outputs.Count;

            //    // transaction size
            //    var size = builder.EstimateSize(tx);

            //    previousFee = new Money(fee.Satoshi);

            //    // calculated fee
            //    fee = new Money(size * feeRate.SatoshiPerByte, MoneyUnit.Satoshi);

            //}



        }
       

    }
}
