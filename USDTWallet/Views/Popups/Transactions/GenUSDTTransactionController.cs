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

namespace USDTWallet.Views.Popups.Transactions
{
    public class GenUSDTTransactionController : ControllerBase, IInteractionRequestAware
    {
        private USDTTransferVM _transferInfo;
        public USDTTransferVM TransferInfo
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

        private INotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, value);
                this.TransferInfo = (USDTTransferVM)_notification.Content;
                this.GenTx();
            }
        }

        public DelegateCommand CopyTxCommand { get; set; }

        private ToastService Toast { get; set; }
        private ClipboardService Clipboard { get; set; }

        private USDTTransactionManager TxManager { get; set; }

        public GenUSDTTransactionController(USDTTransactionManager txManager, ToastService toast, ClipboardService clip)
        {
            this.TxManager = txManager;
            this.Toast = toast;
            this.Clipboard = clip;
            this.CopyTxCommand = new DelegateCommand(CopyTx);
        }

        private async void GenTx()
        {
            var result = await TxManager.CreateTransaction(this.TransferInfo);
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
