using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.USDT
{
    public class BalanceResult
    {
        public string balance { get; set; }
        public string reserved { get; set; }
        public string frozen { get; set; }
    }
}
