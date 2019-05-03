using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common.Gloabal;
using USDTWallet.Models.Models.Wallets;

namespace USDTWallet.Biz.Common
{
    public class BizBase
    {
        protected ActiveWallet CurrentWallet
        { 
            get { return GlobalWallet.Wallet; }
        }
    }
}
