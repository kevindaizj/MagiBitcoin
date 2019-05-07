using NBitcoin;
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

        public async Task<Money> GetBalanceByAddress(string address)
        {
            var addr = BitcoinAddress.Create(address, NetworkOperator.Instance.Network);
            var unspentList = await Client.ListUnspentAsync(0, int.MaxValue, addr);
            return unspentList.Select(o => o.Amount).Sum();
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


        public async Task<List<UnspentCoin>> ListUnspentAsync(string address)
        {
            var addr = BitcoinAddress.Create(address, NetworkOperator.Instance.Network);
            var unspentList = await Client.ListUnspentAsync(0, int.MaxValue, addr);
            return unspentList.ToList();
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
        
        public async Task SignAndSendTransactionByPrivateKey(string privKey, string transactionHex, List<Coin> spentCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var privateKey = Key.Parse(privKey, network);
            var tx = Transaction.Parse(transactionHex, network);
            tx.Sign(privateKey, spentCoins.ToArray());

            var a = tx.ToString();

            await Client.SendRawTransactionAsync(tx);
        }

        public string SerailizeUnsignedTxResult(UnsignTransactionResult txInfo)
        {
            var joint = new UnsignedTxJoint
            {
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


    }
}
