using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace USDTWallet.ControlService.MsgBox
{
    public class MessageBoxService
    {
        public void Show(string messageBoxText)
        {
            MessageBox.Show(messageBoxText);
        }

        public void Show(string messageBoxText, string caption)
        {
            MessageBox.Show(messageBoxText, caption);
        }

        public void Confirm(string messageBoxText, Action confirmCallback = null, Action cancelCallback = null, string caption = "确认")
        {
            var result = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (null != confirmCallback)
                    confirmCallback.Invoke();
            }
            else
            {
                if (null != cancelCallback)
                    cancelCallback.Invoke();
            }
        }
    }
}
