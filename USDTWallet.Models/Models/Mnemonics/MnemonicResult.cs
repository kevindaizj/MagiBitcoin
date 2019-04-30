using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Enums.Network;

namespace USDTWallet.Models.Models.Mnemonics
{
    public class MnemonicResult
    {
        public string[] MnemonicWords { get; set; }
        public string RootAddress { get; set; }
        public int Network { get; set; }
    }
}
