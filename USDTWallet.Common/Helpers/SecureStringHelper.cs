using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Common.Helpers
{
    public static class SecureStringHelper
    {
        public static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static SecureString StringToSecureString(string value)
        {
            if (null == value)
                throw new ArgumentNullException("value could not be null");

            var result = new SecureString();
            foreach (var c in value)
            {
                result.AppendChar(c);
            }

            return result;
        }
    }
}
