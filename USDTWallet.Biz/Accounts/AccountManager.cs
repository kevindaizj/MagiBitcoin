using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Dao.Wallet;

namespace USDTWallet.Biz.Accounts
{
    public class AccountManager : BizBase
    {
        private WalletDao WalletDao { get; set; }

        public AccountManager(WalletDao walletDao)
        {
            this.WalletDao = walletDao;
        }

        
    }
}
