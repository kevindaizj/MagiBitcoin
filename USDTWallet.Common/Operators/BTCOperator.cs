using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.JsonConverters;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common.Helpers;
using USDTWallet.Models.Models.Transactions;

namespace USDTWallet.Common.Operators
{
    public class BTCOperator
    {
        private static Lazy<BTCOperator> _instance = new Lazy<BTCOperator>(() => new BTCOperator());
        public static BTCOperator Instance
        {
            get { return _instance.Value; }
        }
        
        private RPCClient Client { get; set; }

        private readonly string EncryptUnsignedTxKey = "PtquPkgNaG1DEC0rQwZP";
        
        private BTCOperator()
        {
            this.ChangeNetwork(NetworkOperator.Instance.Credential, 
                               NetworkOperator.Instance.RpcUri,
                               NetworkOperator.Instance.Network);
        }

        public void ChangeNetwork(NetworkCredential credential, Uri rpcUri, Network network)
        {
            this.Client = new RPCClient(credential, rpcUri, network);
        }


        public List<AddressGrouping> ListAddressGroupings()
        {
            return Client.ListAddressGroupings().ToList();
        }

        public async Task<Money> GetBalance()
        {
            var balance = await Client.GetBalanceAsync();
            return balance;
        }

        public async Task<Money> GetBalanceByAddress(string address, int minConf = 1)
        {
            var unspentList = await this.ListUnspentAsync(address, minConf);
            return unspentList.Select(o => o.Amount).Sum();
        }

        public async Task<List<UnspentCoin>> ListUnspentAsync(string address, int minConf = 1)
        {
            var addr = BitcoinAddress.Create(address, NetworkOperator.Instance.Network);
            var unspentList = await Client.ListUnspentAsync(minConf, int.MaxValue, addr);
            return unspentList.ToList();
        }


        public async Task<FeeRate> EstimateFeeRate()
        {
            try
            {
                var result = await Client.EstimateSmartFeeAsync(1);
                var feeRate = result.FeeRate;
                return feeRate;
            }
            catch (NoEstimationException)
            {
                return new FeeRate((decimal)0);
            }

        }
        

        public List<Coin> SelectCoinsToSpent(List<UnspentCoin> unspentCoins, Money totalAmount)
        {
            var confirmedCoins = new List<Coin>();
            var unconfirmedCoins = new List<Coin>();
            foreach(var c in unspentCoins)
            {
                if (c.Confirmations > 0)
                    confirmedCoins.Add(c.AsCoin());
                else
                    unconfirmedCoins.Add(c.AsCoin());
            }

            var results = SelectCoinsFrom(confirmedCoins, totalAmount);
            if (results.Count > 0)
                return results;

            results = SelectCoinsFrom(unconfirmedCoins, totalAmount);
            return results;
        }
        

        private List<Coin> SelectCoinsFrom(List<Coin> coins, Money totalAmount)
        {
            var coinsToSpend = new List<Coin>();

            foreach (var coin in coins.OrderByDescending(x => x.Amount))
            {
                coinsToSpend.Add(coin);
                // if doesn't reach amount, continue adding next coin
                if (coinsToSpend.Sum(x => x.Amount) < totalAmount) continue;
                
                break;
            }

            return coinsToSpend;
        }

        public Transaction BuildUnsignedTx(string fromAddress, string toAddress, string changeAddress, 
                                    Money amount, FeeRate estimateFeeRate, List<Coin> spentCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var to = BitcoinAddress.Create(toAddress, network);
            var change = BitcoinAddress.Create(changeAddress, network);

            var builder = network.CreateTransactionBuilder();
            var tx = builder.AddCoins(spentCoins)
                            .Send(to, amount)
                            .SetChange(change)
                            .SendEstimatedFees(estimateFeeRate)
                            .BuildTransaction(false);

            return tx;
        }

        public bool CheckTx(string unsignedTxHex)
        {
            var network = NetworkOperator.Instance.Network;
            try
            {
                var tx = Transaction.Parse(unsignedTxHex, network);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public Transaction ParseTransaction(string txHex)
        {
            return Transaction.Parse(txHex, NetworkOperator.Instance.Network);
        }
        
        public async Task<uint256> SignAndSendTransactionByPrivateKey(List<string> privKeys, string transactionHex, List<Coin> spentCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var tx = Transaction.Parse(transactionHex, network);

            var secrets = new List<BitcoinSecret>();
            foreach (var pk in privKeys)
            {
                var key = Key.Parse(pk, network);
                var secret = key.GetBitcoinSecret(network);
                secrets.Add(secret);
            }

            tx.Sign(secrets.ToArray(), spentCoins.ToArray());

            var a = tx.ToString();
            var status = tx.Check();

            var txId = await Client.SendRawTransactionAsync(tx);
            return txId;
        }

        public string SerailizeUnsignedTxResult(UnsignTransactionResult txInfo)
        {
            var joint = new UnsignedTxJoint
            {
                OperationId = txInfo.OperationId,
                TransactionHex = txInfo.Transaction.ToHex(),
                ToSpentCoins = txInfo.ToSpentCoins
            };

            var json = Serializer.ToString(joint);

            var result = CryptHelper.AESEncryptText(json, EncryptUnsignedTxKey);
            return result;
        }

        public UnsignTransactionResult DeserailizeUnsignedTxResult(string str)
        {
            var json = CryptHelper.AESDecryptText(str, EncryptUnsignedTxKey);
            var joint = Serializer.ToObject<UnsignedTxJoint>(json);
            var tx = Transaction.Parse(joint.TransactionHex, NetworkOperator.Instance.Network);

            return new UnsignTransactionResult
            {
                OperationId = joint.OperationId,
                Transaction = tx,
                ToSpentCoins = joint.ToSpentCoins
            };
        }

        public bool CheckUnsignedTxInfo(string info)
        {
            try
            {
                var json = CryptHelper.AESDecryptText(info, EncryptUnsignedTxKey);
                var joint = Serializer.ToObject<UnsignedTxJoint>(json);
                return this.CheckTx(joint.TransactionHex);
            }
            catch(Exception)
            {
                return false;
            }
        }


        public bool ValidateAddresses(List<string> addresses)
        {
            foreach(var a in addresses)
            {
                bool valid = this.ValidateAddress(a);
                if (!valid)
                    return false;
            }

            return true;
        }

        public bool ValidateAddress(string address)
        {
            try
            {
                BitcoinAddress.Create(address, NetworkOperator.Instance.Network);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }


        public async Task ImportWatchOnlyAddresses(List<string> addresses, string label, bool rescan = false)
        {
            foreach(var addr in addresses)
            {
                var address = BitcoinAddress.Create(addr, NetworkOperator.Instance.Network);
                await Client.ImportAddressAsync(address, label, rescan);
            }
        }

        public async Task ImportPrivateKeyToNode(List<string> privateKeys, string label, bool rescan = false)
        {
            foreach (var key in privateKeys)
            {
                var privateKey = Key.Parse(key, NetworkOperator.Instance.Network);
                await Client.ImportPrivKeyAsync(privateKey.GetBitcoinSecret(NetworkOperator.Instance.Network), label, rescan);
            }
        }

        public async Task<List<CustomRawTransactionInfo>> GetTxIdsFromMempool()
        {
            var txIds = Client.GetRawMempool();
            var txs = new List<CustomRawTransactionInfo>();
            foreach(var txId in txIds)
            {
                var tx = await GetRawTransactionInfoAsync(txId);
                txs.Add(tx);
            }

            return txs;
        }


        public async Task<CustomRawTransactionInfo> GetRawTransactionInfoAsync(uint256 txId)
        {
            var request = new RPCRequest(RPCOperations.getrawtransaction, new object[] { txId, 1 });
            var response = await Client.SendCommandAsync(request);
            var json = response.Result;

            return new CustomRawTransactionInfo
            {
                Transaction = ParseTxHex(json.Value<string>("hex")),
                TransactionId = uint256.Parse(json.Value<string>("txid")),
                TransactionTime = json["time"] != null ? NBitcoin.Utils.UnixTimeToDateTime(json.Value<long>("time")) : (DateTimeOffset?)null,
                Hash = uint256.Parse(json.Value<string>("hash")),
                Size = json.Value<uint>("size"),
                VirtualSize = json.Value<uint>("vsize"),
                Version = json.Value<uint>("version"),
                LockTime = new LockTime(json.Value<uint>("locktime")),
                BlockHash = json["blockhash"] != null ? uint256.Parse(json.Value<string>("blockhash")) : null,
                Confirmations = json.Value<uint>("confirmations"),
                BlockTime = json["blocktime"] != null ? NBitcoin.Utils.UnixTimeToDateTime(json.Value<long>("blocktime")) : (DateTimeOffset?)null
            };
        }

        private Transaction ParseTxHex(string hex)
        {
            var tx = NetworkOperator.Instance.Network.Consensus.ConsensusFactory.CreateTransaction();
            tx.ReadWrite(Encoders.Hex.DecodeData(hex), NetworkOperator.Instance.Network);
            return tx;
        }


        public async Task<Transaction> GetTransaction(string transactionId)
        {
            var txId = uint256.Parse(transactionId);
            var raw = await this.GetRawTransactionInfoAsync(txId);
            return raw.Transaction;
        }


    }

}
