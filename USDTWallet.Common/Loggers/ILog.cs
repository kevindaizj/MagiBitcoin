using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Common.Loggers
{
    public interface ILog
    {
        void Fatal(string msg);
        void Error(string msg);
        void Error(Exception ex);
        void Info(string msg);
        void Debug(string msg);
    }
}
