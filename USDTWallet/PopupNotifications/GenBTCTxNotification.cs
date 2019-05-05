using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Models.Transfer;

namespace USDTWallet.PopupNotifications
{
    public class GenBTCTxNotification : Notification
    {
        public BTCTransferVM TransferInfo { get; set; }
    }
}
