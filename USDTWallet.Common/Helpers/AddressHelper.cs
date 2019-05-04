using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace USDTWallet.Common.Helpers
{
    public static class AddressHelper
    {
        public static readonly string AccountRegex = @"[^OIl0]{25,34}$";

        public static bool IsValidAccountAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            return Regex.IsMatch(address, AccountRegex);
        }
    }
}
