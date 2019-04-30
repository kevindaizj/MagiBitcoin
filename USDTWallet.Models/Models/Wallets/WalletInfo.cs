using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Wallets
{
    public class WalletInfo
    {
        public string Id { get; set; }
        public string WalletName { get; set; }
        public string Password { get; set; }
        public string MnemonicWords { get; set; }
        public string Description { get; set; }
    }
}
