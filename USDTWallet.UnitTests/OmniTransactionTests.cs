using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using USDTWallet.Biz.Transactions;

namespace USDTWallet.UnitTests
{
    [TestClass]
    public class OmniTransactionTests
    {
        [TestMethod]
        public void TestBuild()
        {
            var manager = new USDTTransactionManager(new Dao.Transaction.TransactionDao());
            var fromAddress = BitcoinAddress.Create("mtYpuP45hjDBgFuYfRmeG7i63QYQCwSuPN", Network.RegTest);
            var toAddress = BitcoinAddress.Create("n3uXG3c3ouZUtVZUbzCZCQomAguGxdwQcQ", Network.RegTest);
            var feeAddress = BitcoinAddress.Create("n4FDWXj611q8ALYKw61jou8kLFcByKVXqs", Network.RegTest);
            var amount = Money.Parse("10");
            var feeRate = new FeeRate(Money.Parse("0.0001"));

            var result = manager.Build(fromAddress, toAddress, feeAddress, amount, feeRate).GetAwaiter().GetResult();
        }
    }
}
