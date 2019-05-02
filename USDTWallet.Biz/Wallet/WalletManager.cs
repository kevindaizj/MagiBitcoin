using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Helpers;
using USDTWallet.Common.Operators;
using USDTWallet.Dao.Address;
using USDTWallet.Dao.Wallet;
using USDTWallet.Models.Enums.Address;
using USDTWallet.Models.Models.Addresses;
using USDTWallet.Models.Models.Mnemonics;
using USDTWallet.Models.Models.Wallets;

namespace USDTWallet.Biz.Wallet
{
    public class WalletManager : BizBase
    {
        private WalletDao WalletDao { get; set; }
        private AddressDao AddressDao { get; set; }

        public WalletManager(WalletDao walletDao, AddressDao addressDao)
        {
            this.WalletDao = walletDao;
            this.AddressDao = addressDao;
        }

        public bool CheckAnyWalletExisted()
        {
            return WalletDao.AnyWalletExisted();
        }

        public MnemonicResult CreateWallet(string password)
        {
            var result = KeyOperator.Instance.CreateMnemonicRoot(password);
            var wallet = new WalletInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                WalletName = "Wallet_" + DateTime.Now.ToString("yyyyMMddHHmm"),
                Password = MD5Helper.ToMD5(password),
                MnemonicWords = string.Join(" ", result.MnemonicWords)
            };
            var rootAddress = new AddressInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                Address = result.RootAddress,
                ExtPubKeyWif = result.RootExtPubKeyWif,
                WalletId = wallet.Id,
                Network = result.Network,
                KeyPath = null,
                AddressType = (long)CustomAddressType.Root,
                AddressCategory = (long)AddressCategory.Default, 
                Name = "Coinbase"
            };

            WalletDao.Create(wallet);
            AddressDao.Create(rootAddress);

            return result;
        }

        public WalletInfo GetActiveWallet()
        {
            return WalletDao.GetActiveWallet();
        }

        public bool Login(string pwd)
        {
            var wallet = WalletDao.GetActiveWallet();
            if (MD5Helper.ToMD5(pwd) != wallet.Password)
                return false;
            return true;
        }
    }
}
