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
    }
}
