using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Models.USDT;

namespace USDTWallet.Common.Operators
{
    public class USDTOperator
    {
        private static Lazy<USDTOperator> _instance = new Lazy<USDTOperator>(() => new USDTOperator());
        public static USDTOperator Instance
        {
            get { return _instance.Value; }
        }

        private RPCClient Client { get; set; }

        public static readonly UInt64 PropertyId = 2147483651;

        private USDTOperator()
        {
            this.ChangeNetwork(NetworkOperator.Instance.Credential,
                               NetworkOperator.Instance.RpcUri,
                               NetworkOperator.Instance.Network);
        }

        public void ChangeNetwork(NetworkCredential credential, Uri rpcUri, Network network)
        {
            this.Client = new RPCClient(credential, rpcUri, network);
        }

        public async Task<Money> GetBalanceByAddress(string address)
        {
            var resp = await Client.SendCommandAsync("omni_getbalance", address, PropertyId);
            var result = resp.Result.ToObject<BalanceResult>();
            return Money.Parse(result.balance);
        }


        public Transaction BuildUnsignedTx(string fromAddress, string toAddress, string changeAddress, Money amount, FeeRate estimateFeeRate, List<Coin> spentCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(toAddress, network);
            var change = BitcoinAddress.Create(changeAddress, network);

            var builder = network.CreateTransactionBuilder();
            var tx = builder.AddCoins(spentCoins)
                            .SendFees("0.00001")
                            .SetChange(change)
                            .SendEstimatedFees(estimateFeeRate)
                            .BuildTransaction(false);

            var txHash = tx.ToHex();

            var amountPayloadRes = Client.SendCommand("omni_createpayload_simplesend", PropertyId, amount.ToString());
            var amountPayload = amountPayloadRes.Result.ToString();

            var opreturnRes = Client.SendCommand("omni_createrawtx_opreturn", txHash, amountPayload);
            var opreturn = opreturnRes.Result.ToString();

            var receiveRefRes = Client.SendCommand("omni_createrawtx_reference", opreturn, toAddress);
            var receiveRef = receiveRefRes.Result.ToString();

            var finalTx = Transaction.Parse(receiveRef, network);

            return finalTx;
        }

    }
}
