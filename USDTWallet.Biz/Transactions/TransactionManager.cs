using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.JsonConverters;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Helpers;
using USDTWallet.Common.Operators;
using USDTWallet.Dao.Address;
using USDTWallet.Dao.Transaction;
using USDTWallet.Dao.Wallet;
using USDTWallet.Models.Models.FeeRates;

namespace USDTWallet.Biz.Transactions
{
    public class TransactionManager : BizBase
    {
        private WalletDao WalletDao { get; set; }
        private AddressDao AddressDao { get; set; }
        private TransactionDao TransactionDao { get; set; }

        public TransactionManager(WalletDao walletDao, AddressDao addressDao, TransactionDao TxDao)
        {
            this.WalletDao = walletDao;
            this.AddressDao = addressDao;
            this.TransactionDao = TxDao;
        }

        public async Task SignAndSendBTCTransaction(string password, string transactionHex, List<Coin> spentCoins)
        {
            var privKeyWifs = this.GetPrivateKeys(password, spentCoins).Take(1).ToList();
            await this.SignAndSendTransactionByPrivateKey(privKeyWifs, transactionHex, spentCoins);
        }

        public async Task SignAndSendUSDTTransaction(string password, string transactionHex, List<Coin> spentCoins)
        {
            var privKeyWifs = this.GetPrivateKeys(password, spentCoins);
            await this.SignAndSendTransactionByPrivateKey(privKeyWifs, transactionHex, spentCoins);
        }


        public List<string> GetPrivateKeys(string password, List<Coin> spentCoins)
        {
            var wallet = WalletDao.GetWalletById(CurrentWallet.Id);
            if (MD5Helper.ToMD5(password) != wallet.Password)
                throw new WTException(ExceptionCode.WrongWalletPassword, "密码错误");

            var network = NetworkOperator.Instance.Network;
            var addresses = spentCoins.Select(o => o.ScriptPubKey.GetDestinationAddress(network).ToString()).Distinct().ToList();
            var addressInfos = AddressDao.GetByAddresses(CurrentWallet.Id, addresses);
            if (addressInfos.Count == 0 || addressInfos.Count != addresses.Count)
                throw new WTException(ExceptionCode.AddressNotExisted, "当前钱包找不到相关比特币地址");

            var privateKeys = new List<string>();

            foreach (var addr in addressInfos)
            {
                var keyPath = new KeyPath(addr.KeyPath);
                var rootXPrivKey = ExtKey.Parse(CurrentWallet.RootXPrivKey, network);
                var xPrivKey = rootXPrivKey.Derive(keyPath);
                var privKeyWif = xPrivKey.PrivateKey.GetWif(network).ToString();
                privateKeys.Add(privKeyWif);
            }

            return privateKeys;
        }


        public async Task<FeeRate> GetFeeRate()
        {
            using (var client = new HttpClient())
            {
                //https://bitcoinfees.21.co/api/v1/fees/recommended  （打不开）
                const string request = @"https://bitcoinfees.earn.com/api/v1/fees/recommended";
                var resp = await client.GetAsync(request, HttpCompletionOption.ResponseContentRead);
                string json = await resp.Content.ReadAsStringAsync();
                var result = Serializer.ToObject<FeeRateResult>(json);
                
                return new FeeRate((decimal)result.fastestFee);
            }
        }


        public async Task SignAndSendTransactionByPrivateKey(List<string> privKeys, string transactionHex, List<Coin> spentCoins)
        {
            var network = NetworkOperator.Instance.Network;
            var tx = Transaction.Parse(transactionHex, network);

            var txId = await BTCOperator.Instance.SignAndSendTransactionByPrivateKey(privKeys, transactionHex, spentCoins);

            TransactionDao.Sign(tx.GetHash().ToString(), txId.ToString());
        }

    }
}
