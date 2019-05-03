using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Wallets
{
    public class ActiveWallet
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RootXPrivKey { get; set; }
        public string RootXPubKey { get; set; }
    }
}
