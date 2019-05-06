using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Transactions;
using USDTWallet.Common;
using USDTWallet.Common.Operators;
using USDTWallet.ControlService.Clipb;
using USDTWallet.ControlService.Toast;
using USDTWallet.Models.Models.Transfer;
using USDTWallet.PopupNotifications;

namespace USDTWallet.Views.Popups.Transactions
{
    public class GenBTCTransactionController : ControllerBase, IInteractionRequestAware
    {
        private BTCTransferVM _transferInfo;
        public BTCTransferVM TransferInfo
        {
            get { return _transferInfo; }
            set { this.SetProperty(ref _transferInfo, value); }
        }

        private string _txHex;
        public string TxHex
        {
            get { return _txHex; }
            set { this.SetProperty(ref _txHex, value); }
        }

        private string _txJson;
        public string TxJson
        {
            get { return _txJson; }
            set { this.SetProperty(ref _txJson, value); }
        }

        public Action FinishInteraction { get; set; }

        private GenBTCTxNotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, (GenBTCTxNotification)value);
                this.TransferInfo = _notification.TransferInfo;
                this.GenTx();
            }
        }
        
        public DelegateCommand CopyTxCommand { get; set; }

        private ToastService Toast { get; set; }
        private ClipboardService Clipboard { get; set; }

        private BTCTransactionManager TxManager { get; set; }

        public GenBTCTransactionController(BTCTransactionManager txManager, ToastService toast, ClipboardService clip)
        {
            this.TxManager = txManager;
            this.Toast = toast;
            this.Clipboard = clip;
            this.CopyTxCommand = new DelegateCommand(CopyTx);
        }

        private async void GenTx()
        {
            var result = await TxManager.BuildUnsignedTransaction(this.TransferInfo);
            this.TxJson = result.Transaction.ToString();
            this.TxHex = BTCOperator.Instance.SerailizeUnsignedTxResult(result);
        }


        private void CopyTx()
        {
            this.Clipboard.SetText(this.TxHex);
            this.Toast.Success("复制成功");
        }

    }
}
