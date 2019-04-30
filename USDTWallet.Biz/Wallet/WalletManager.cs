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
                WalletId = wallet.Id,
                KeyPath = null,
                Network = result.Network
            };

            WalletDao.Create(wallet);
            AddressDao.Create(rootAddress);

            return result;
        }
    }
}
