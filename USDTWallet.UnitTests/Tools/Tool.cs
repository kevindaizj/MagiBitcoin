using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.UnitTests.Tools
{
    public class Tool
    {
        private int VoutIdx { get; set; }

        public UnspentCoin NewUnspentCoin(BitcoinAddress address, Money amount)
        {
            var testJson =
@"{
	""txid"" : ""d54994ece1d11b19785c7248868696250ab195605b469632b7bd68130e880c9a"",
	""account"" : ""test label"",
	""confirmations"" : 6210,
	""spendable"" : false
}";
            var testData = JObject.Parse(testJson);
            testData.Add("vout", JToken.FromObject(VoutIdx++));
            testData.Add("address", JToken.FromObject(address.ToString()));
            testData.Add("scriptPubKey", JToken.FromObject(address.ScriptPubKey.ToHex()));
            testData.Add("amount", JToken.FromObject(amount.ToString()));

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            CultureInfo culture = null;

            var param = new List<object> { testData, Network.TestNet };
            object instantiatedType = Activator.CreateInstance(typeof(UnspentCoin), flags, null, param.ToArray(), culture);

            return instantiatedType as UnspentCoin;
        }

        public TxOut NewOpReturnOutput()
        {
            TxOut opReturnOutput = new TxOut(Money.Zero, new Script("OP_RETURN 6f6d6e690000000080000003000000003b9aca00"));
            return opReturnOutput;
        }
    }
}
