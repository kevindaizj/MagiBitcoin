using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Transactions
{
    public class OmniFeatureOutputs
    {
        public TxOut OpReturnOutput { get; set; }
        public TxOut ReferenceOutput { get; set; }
    }
}
