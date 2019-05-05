using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Loggers;
using USDTWallet.ControlService.Toast;

namespace USDTWallet.Common
{
    public static class GlobalExceptionHandler
    {
        public static void HandleDispatcherException(DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            if (null == exception)
                return;

            var customException = exception as WTException;

            if (null != customException)
                HandleWTException(customException);
            else
                HandleOtherException(exception);

            e.Handled = true;
        }

        public static void HandleAppDomainException(UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (null == exception)
                return;

            var customException = exception as WTException;

            if (null != customException)
                HandleWTException(customException);
            else
                HandleOtherException(exception);

        }

        private static void HandleOtherException(Exception exception)
        {
            DispatcherHelper.Invoke(() =>
            {
                var toast = new ToastService();
                string errorMsg = string.Format("ERROR: {0}", exception.Message);
                toast.Error(errorMsg);
            });

            Logger.Instance.Fatal(exception.ToString());
        }

        private static void HandleWTException(WTException customException)
        {
            DispatcherHelper.Invoke(() =>
            {
                var toast = new ToastService();
                string errorMsg = string.Format("BIZ-EXP-{0}: {1}", customException.ExceptionCode, customException.ExceptionMessage);
                toast.Warning(errorMsg);
            });

            Logger.Instance.Error(customException.ToString());
        }


    }
}
