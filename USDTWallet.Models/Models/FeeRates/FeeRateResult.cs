using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.FeeRates
{
    public class FeeRateResult
    {
        public uint fastestFee { get; set; }
        public uint halfHourFee { get; set; }
        public uint hourFee { get; set; }
    }
}
