using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Common.Exceptions
{
    /// <summary>
    /// 业务异常
    /// </summary>
    public class WTException : Exception
    {
        public WTException(ExceptionCode exceptionCode)
        {
            ExceptionCode = exceptionCode;
            ExceptionMessage = string.Empty;
        }

        public WTException(ExceptionCode exceptionCode, string errorMessage)
        {
            ExceptionCode = exceptionCode;
            ExceptionMessage = errorMessage;
        }

        public ExceptionCode ExceptionCode { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
