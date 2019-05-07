using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Helpers;
using USDTWallet.Common.Operators;
using USDTWallet.Dao.Address;
using USDTWallet.Dao.Wallet;

namespace USDTWallet.Biz.Transactions
{
    public class TransactionManager : BizBase
    {
        private WalletDao WalletDao { get; set; }
        private AddressDao AddressDao { get; set; }

        public TransactionManager(WalletDao walletDao, AddressDao addressDao)
        {
            this.WalletDao = walletDao;
            this.AddressDao = addressDao;
        }

        public async Task SignAndSendTransaction(string password, string transactionHex, List<Coin> spentCoins)
        {
            var wallet = WalletDao.GetWalletById(CurrentWallet.Id);
            if (MD5Helper.ToMD5(password) != wallet.Password)
                throw new WTException(ExceptionCode.WrongWalletPassword, "密码错误");

            var network = NetworkOperator.Instance.Network;
            var address = spentCoins.Select(o => o.ScriptPubKey.GetDestinationAddress(network).ToString()).First();
            var addressInfo = AddressDao.GetByAddress(CurrentWallet.Id, address);
            if (null == addressInfo)
                throw new WTException(ExceptionCode.AddressNotExisted, "当前钱包找不到相关比特币地址");

            var keyPath = new KeyPath(addressInfo.KeyPath);
            var rootXPrivKey = ExtKey.Parse(CurrentWallet.RootXPrivKey, network);
            var xPrivKey = rootXPrivKey.Derive(keyPath);
            var privKeyWif = xPrivKey.PrivateKey.GetWif(network).ToString();

            await BTCOperator.Instance.SignAndSendTransactionByPrivateKey(privKeyWif, transactionHex, spentCoins);
        }
    }
}
