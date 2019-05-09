﻿using NBitcoin;
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

        public static readonly decimal SentBTCPerTx = 0.00000546M;

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

        public async Task<string> CreatePayloadSimpleSend(Money amount)
        {
            var resp = await Client.SendCommandAsync("omni_createpayload_simplesend", PropertyId, amount.ToString());
            return resp.Result.ToString();
        }

        public async Task<string> GenerateOpRetrun(string transactionHex, string payloadHex)
        {
            var resp = await Client.SendCommandAsync("omni_createrawtx_opreturn", transactionHex, payloadHex);
            return resp.Result.ToString();
        }

        public async Task<string> GenerateReference(string opreturn, string toAddress)
        {
            var resp = await Client.SendCommandAsync("omni_createrawtx_reference", opreturn, toAddress);
            return resp.Result.ToString();
        }




        public async Task<Transaction> BuildUnsignedTx(string fromAddress, string toAddress, string changeAddress, Money amount, FeeRate estimateFeeRate, List<Coin> spentCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(toAddress, network);
            var change = BitcoinAddress.Create(changeAddress, network);

            var builder = network.CreateTransactionBuilder();
            var tx = builder.AddCoins(spentCoins)
                            .SetChange(change)
                            .SendEstimatedFees(estimateFeeRate)
                            .BuildTransaction(false);

            var detail = tx.ToString();
            
            var amountPayload = await this.CreatePayloadSimpleSend(amount);
            var opreturn = await this.GenerateOpRetrun(tx.ToHex(), amountPayload);
            var receiveRef = await this.GenerateReference(opreturn, toAddress);

            var finalTx = Transaction.Parse(receiveRef, network);

            return finalTx;
        }

    }
}
