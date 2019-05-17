using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json.Linq;
using USDTWallet.Biz.Transactions;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Operators;
using USDTWallet.Models.Enums.Network;
using USDTWallet.Models.Models.Transactions;
using USDTWallet.UnitTests.Tools;

namespace USDTWallet.UnitTests
{
    [TestClass]
    public class OmniTransactionTests
    {
        private void SetNetwork()
        {
            NetworkOperator.Instance.ChangeNetwork(CustomNetworkType.Regtest, "http://localhost:8339", "kevin", "123456");
        }

        private USDTTransactionManager TxManager
        {
            get { return new USDTTransactionManager(new Dao.Transaction.TransactionDao());  }
        }

        private Key FromPrivKey
        {
            get { return Key.Parse("cT6LC1FGBu78cD47D9K6yT8yyrsEcQKAfeMqUuFcVub26568VMCM", Network.RegTest); }
        }

        private BitcoinAddress FromAddress
        {
            get { return FromPrivKey.GetBitcoinSecret(Network.RegTest).GetAddress();  }
        }
        private BitcoinAddress ToAddress
        {
            get { return BitcoinAddress.Create("n3uXG3c3ouZUtVZUbzCZCQomAguGxdwQcQ", Network.RegTest); }
        }

        private Key FeePrivKey
        {
            get { return Key.Parse("cSijF7WaKNXbaTd3H2ydnHTHbB2aG4spPXH5xqwmUMCBFFjfxGnw", Network.RegTest); }
        }

        private BitcoinAddress FeeAddress
        {
            get { return FeePrivKey.GetBitcoinSecret(Network.RegTest).GetAddress(); }
        }
        private Money DustCostBTC
        {
            get { return Money.Parse("0.00000546"); }
        }
        private FeeRate EstimateFeeRate
        {
            get { return new FeeRate(Money.Parse("0.001")); }
        }
        private TxOut OpReturnOutput
        {
            get { return new Tool().NewOpReturnOutput(); }
        }


        //[TestMethod]
        //public void TestBuild()
        //{
        //    var manager = new USDTTransactionManager(new Dao.Transaction.TransactionDao());
        //    var fromAddress = BitcoinAddress.Create("mtYpuP45hjDBgFuYfRmeG7i63QYQCwSuPN", Network.RegTest);
        //    var toAddress = BitcoinAddress.Create("n3uXG3c3ouZUtVZUbzCZCQomAguGxdwQcQ", Network.RegTest);
        //    var feeAddress = BitcoinAddress.Create("n4FDWXj611q8ALYKw61jou8kLFcByKVXqs", Network.RegTest);
        //    var amount = Money.Parse("10");
        //    var feeRate = new FeeRate(Money.Parse("0.0001"));

        //    var result = manager.Build(fromAddress, toAddress, feeAddress, amount, feeRate).GetAwaiter().GetResult();
        //}


        //[TestMethod]
        //public void AreOmniFeaturedOutputsCorrect()
        //{
        //    this.SetNetwork();
        //    var toAddress = BitcoinAddress.Create("n3uXG3c3ouZUtVZUbzCZCQomAguGxdwQcQ", Network.RegTest);
        //    var amount = Money.Parse("10");

        //    var manager = new PrivateObject(TxManager);
        //    var task = manager.Invoke("GetOmniFeaturedOutputs", toAddress, amount) as Task<OmniFeaturedOutputs>;
        //    var result = task.GetAwaiter().GetResult();

        //    Assert.AreEqual(result.OpReturnOutput.ScriptPubKey.ToString(), "OP_RETURN 6f6d6e690000000080000003000000003b9aca00");
        //    Assert.IsTrue(result.ReferenceOutput.IsTo(toAddress));
        //}


        /// <summary>
        /// 发送、接收地址不能相同
        /// </summary>    
        [TestMethod]
        //[ExpectedException(typeof(WTException))]
        public void FromAddressNotAllowBeSameAsToAddress()
        {
            var address = BitcoinAddress.Create("mtYpuP45hjDBgFuYfRmeG7i63QYQCwSuPN", Network.RegTest);

            Assert.ThrowsException<WTException>(() =>
            {
                this.InvokeBuild(null, null, null, null, null, address, address, null);
            });
            
        }

        /// <summary>
        /// from地址的Coin只能支付最低费用，最终生成的tx会用到fee地址的Coin
        /// </summary>
        [TestMethod]
        public void UseCoinsOfBothFromAndFee()
        {
            var tool = new Tool();
            var dust = Money.Parse(this.DustCostBTC.ToString());
            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust + Money.Parse("0.00000001"))
            };
            var feeUnspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FeeAddress, Money.Parse("0.1")),
                tool.NewUnspentCoin(FeeAddress, Money.Parse("10"))
            };

            var result = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins);
            Assert.IsTrue(result.InputCoins.Count == 2);
            Assert.AreEqual(result.InputCoins[0].Outpoint, fromUnspentCoin[0].OutPoint);
        }

        /// <summary>
        /// from地址的Coin足够支付所有费用，最终只生成一个Input, 没有用到fee地址的Coin
        /// </summary>
        [TestMethod]
        public void OnlyUseFromCoin()
        {
            var tool = new Tool();
            var dust = Money.Parse(this.DustCostBTC.ToString());
            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust + Money.Parse("1"))
            };
            var feeUnspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FeeAddress, Money.Parse("0.1")),
                tool.NewUnspentCoin(FeeAddress, Money.Parse("10"))
            };

            var result = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins);
            Assert.IsTrue(result.InputCoins.Count == 1);
            Assert.AreEqual(result.InputCoins[0].Outpoint, fromUnspentCoin[0].OutPoint);

            fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust + Money.Parse("1000"))
            };
            result = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins);
            Assert.IsTrue(result.InputCoins.Count == 1);
            Assert.AreEqual(result.InputCoins[0].Outpoint, fromUnspentCoin[0].OutPoint);

        }


        ///// <summary>
        ///// 未签名的预计size与签名后的size一致(有找零的情况下)
        ///// </summary>
        [TestMethod]
        public void EstimatedUnsignedSizeIsSameAsSignedSize()
        {
            var tool = new Tool();
            var keys = new ISecret[]
            {
                FromPrivKey.GetBitcoinSecret(Network.RegTest),
                FeePrivKey.GetBitcoinSecret(Network.RegTest)
            };

            var dust = Money.Parse(this.DustCostBTC.ToString());
            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust + Money.Parse("0.0000001"))
            };

            var feeUnspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FeeAddress, Money.Parse("10"))
            };

            var result = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins);
            var tx = result.Transaction;
            tx.Sign(keys, result.InputCoins.ToArray());

            var esitmatedSize = result.EstimatedSize;
            var actualSize = tx.GetSerializedSize();
            var diff = esitmatedSize - actualSize;

            Assert.IsTrue(diff >= 0 && diff <= result.InputCoins.Count);
        }

        ///// <summary>
        ///// 未签名的预计size与签名后的size不一致（刚好用完Coin，没有找零）
        ///// </summary>
        [TestMethod]
        public void EstimatedUnsignedSizeIsSameAsSignedSize2()
        {
            var tool = new Tool();
            var keys = new ISecret[]
            {
                FromPrivKey.GetBitcoinSecret(Network.RegTest),
                FeePrivKey.GetBitcoinSecret(Network.RegTest)
            };

            var dust = Money.Parse(this.DustCostBTC.ToString());
            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust + Money.Parse("0.00000001"))
            };

            var feeUnspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FeeAddress, Money.Parse("2")),
                tool.NewUnspentCoin(FeeAddress, Money.Parse("3.53"))
            };


            var feeRate = new FeeRate(Money.Parse("10"));

            var result = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins, dust, feeRate);
            var tx = result.Transaction;
            
            tx.Sign(keys, result.InputCoins.ToArray());

            var esitmatedSize = result.SizeFromFee;
            var actualSize = tx.GetSerializedSize();
            var delta = esitmatedSize - actualSize;
            
            Assert.IsTrue(delta == 34 + tx.Inputs.Count);
        }


        /// <summary>
        /// 有找零时预计手续费与最终手续费一致
        /// </summary>
        [TestMethod]
        public void EstimatedFeeWithChangeOutput()
        {
            var tool = new Tool();

            var dust = Money.Parse(this.DustCostBTC.ToString());
            var feeAmount = Money.Parse("100");
            var totalAmount = dust + feeAmount;

            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust)
            };
            var fromCoin = fromUnspentCoin.Single().AsCoin();

            var feeUnspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FeeAddress, feeAmount)
            };

            // 包含找零output的tx总大小
            var size = 405;
            // 此费率刚好用完所有的Coin
            var exactFeeRateAmount = new Money(feeAmount.Satoshi / size * 1000);

            // 小于exactFeeRateAmount，则一定有找零output生成
            var feeRateAmount = exactFeeRateAmount / 100;
            var feeRate = new FeeRate(feeRateAmount);

            var txInfo = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins, estimateFeeRate: feeRate);
            var feeInfo = this.InvokeEstimateFinalFee(fromCoin, feeUnspentCoins, estimateFeeRate: feeRate);

            var tx = txInfo.Transaction;
            var outputAmount = tx.Outputs.Select(o => o.Value).Sum();
            var actualUsedFee = totalAmount - outputAmount;

            var currentTxFee = feeRate.GetFee(txInfo.EstimatedSize);
            
            Assert.IsTrue(feeInfo.Fee == actualUsedFee && actualUsedFee == currentTxFee);
        }


        /// <summary>
        /// 无找零时预计手续费与最终手续费一致
        /// </summary>
        [TestMethod]
        public void EstimatedFeeWithoutChangeOutput()
        {
            var tool = new Tool();

            var dust = Money.Parse(this.DustCostBTC.ToString());
            var feeAmount = Money.Parse("100");
            var totalAmount = dust + feeAmount;

            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust)
            };
            var fromCoin = fromUnspentCoin.Single().AsCoin();

            var feeUnspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FeeAddress, feeAmount)
            };

            // 包含找零output的tx总大小
            var size = 405;
            var exactFeeRateAmount = new Money(feeAmount.Satoshi / size * 1000);
            // 此费率刚好用完所有的Coin (意味着不会产生找零output)
            var feeRate = new FeeRate(exactFeeRateAmount);

            var txInfo = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins, estimateFeeRate: feeRate);
            var feeInfo = this.InvokeEstimateFinalFee(fromCoin, feeUnspentCoins, estimateFeeRate: feeRate);

            var tx = txInfo.Transaction;
            var outputAmount = tx.Outputs.Select(o => o.Value).Sum();
            
            var changeOutputFee = feeRate.GetFee(34);

            var actualUsedFee = totalAmount - outputAmount;
            var currentTxFee = feeRate.GetFee(txInfo.EstimatedSize);
            var delta = actualUsedFee - currentTxFee;
            var delta2 = feeInfo.Fee - currentTxFee;

            Assert.AreEqual(delta2, changeOutputFee);
            Assert.IsTrue(delta - changeOutputFee < dust);
        }



        /// <summary>
        /// 检查BTC找零金额计算是否正确
        /// </summary>
        [TestMethod]
        public void CheckIsBTCChangeAmountCorrect()
        {
            var tool = new Tool();

            var total = Money.Parse("10000");
            var dust = Money.Parse("10");
            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, total)
            };

            var feeUnspentCoins = new List<UnspentCoin>();
            var feeRate = new FeeRate(Money.Parse("1"));

            var result = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins, dust, feeRate);
            var tx = result.Transaction;

            var changeOutput = tx.Outputs.SingleOrDefault(o => o.Value != dust && o.Value != Money.Zero);
            Assert.IsNotNull(changeOutput);
            Assert.IsTrue(changeOutput.IsTo(FeeAddress));

            var txSize = Network.RegTest.CreateTransactionBuilder()
                                        .AddCoins(fromUnspentCoin.Select(o => o.AsCoin()).ToList())
                                        .ContinueToBuild(tx)
                                        .EstimateSize(tx);

            var fee = feeRate.GetFee(txSize);
            var expectedChangeAmount = total - dust - fee;
            var actualChangeAmount = changeOutput.Value;

            Assert.AreEqual(expectedChangeAmount, actualChangeAmount);
        }

        
        /// <summary>
        /// 检查BTC找零金额计算是否正确
        /// </summary>
        [TestMethod]
        public void CheckIsBTCChangeAmountCorrect2()
        {
            var tool = new Tool();
            
            var dust = Money.Parse("10");
            var fromUnspentCoin = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FromAddress, dust + Money.Parse("0.00000001"))
            };

            var feeUnspentCoins = new List<UnspentCoin>
            {
                tool.NewUnspentCoin(FeeAddress, Money.Parse("10"))
            };


            var allCoins = fromUnspentCoin.Concat(feeUnspentCoins).Select(o => o.AsCoin()).ToList();
            var total = allCoins.Select(o => o.Amount).Sum();
            
            var feeRate = new FeeRate(Money.Parse("1"));

            var result = this.InvokeBuild(fromUnspentCoin, feeUnspentCoins, dust, feeRate);
            var tx = result.Transaction;
            
            var changeOutput = tx.Outputs.SingleOrDefault(o => o.Value != dust && o.Value != Money.Zero);
            Assert.IsNotNull(changeOutput);
            Assert.IsTrue(changeOutput.IsTo(FeeAddress));

            var txSize = Network.RegTest.CreateTransactionBuilder()
                                        .AddCoins(allCoins)
                                        .ContinueToBuild(tx)
                                        .EstimateSize(tx);
            var fee = feeRate.GetFee(txSize);

            var expectedChangeAmount = total - dust - fee;
            var actualChangeAmount = changeOutput.Value;

            Assert.AreEqual(expectedChangeAmount, actualChangeAmount);
        }
        




        private OmniTransactionBuildResult InvokeBuild(
                                                 List<UnspentCoin> fromUnspentCoin, 
                                                 List<UnspentCoin> feeUnspentCoins,
                                                 Money dustCostBTC = null, 
                                                 FeeRate estimateFeeRate = null, 
                                                 TxOut opReturnOutput = null,
                                                 BitcoinAddress fromAddress = null,
                                                 BitcoinAddress toAddress = null,
                                                 BitcoinAddress feeAddress = null)
        {
            dustCostBTC = dustCostBTC ?? this.DustCostBTC;
            estimateFeeRate = estimateFeeRate ?? this.EstimateFeeRate;
            opReturnOutput = opReturnOutput ?? this.OpReturnOutput;
            fromAddress = fromAddress ?? this.FromAddress;
            toAddress = toAddress ?? this.ToAddress;
            feeAddress = feeAddress ?? this.FeeAddress;
            
            var result = TxManager.Build(fromAddress, toAddress, feeAddress, fromUnspentCoin, feeUnspentCoins, dustCostBTC, estimateFeeRate, opReturnOutput);
            return result;
        }


        private CalculatedOmniFeeInfo InvokeEstimateFinalFee(
                                                 Coin fromCoin,
                                                 List<UnspentCoin> feeUnspentCoins,
                                                 Money dustCostBTC = null,
                                                 FeeRate estimateFeeRate = null,
                                                 TxOut opReturnOutput = null,
                                                 BitcoinAddress toAddress = null,
                                                 BitcoinAddress feeAddress = null)
        {
            dustCostBTC = dustCostBTC ?? this.DustCostBTC;
            estimateFeeRate = estimateFeeRate ?? this.EstimateFeeRate;
            opReturnOutput = opReturnOutput ?? this.OpReturnOutput;
            toAddress = toAddress ?? this.ToAddress;
            feeAddress = feeAddress ?? this.FeeAddress;

            var result = TxManager.EstimateFinalFee(toAddress, feeAddress, fromCoin, feeUnspentCoins, dustCostBTC, estimateFeeRate, opReturnOutput);
            return result;
        }


    }
}
