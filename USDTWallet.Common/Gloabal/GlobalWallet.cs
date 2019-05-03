using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Models.Wallets;

namespace USDTWallet.Common.Gloabal
{
    public static class GlobalWallet
    {
        public static ActiveWallet Wallet { get; private set; }

        public static void Set(ActiveWallet wallet)
        {
            Wallet = wallet;
        }
    }
}
