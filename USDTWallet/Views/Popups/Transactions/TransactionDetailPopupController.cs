using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common;
using USDTWallet.ControlService.Clipb;
using USDTWallet.ControlService.Toast;
using USDTWallet.Models.Models.Transactions;

namespace USDTWallet.Views.Popups.Transactions
{
    public class TransactionDetailPopupController : ControllerBase, IInteractionRequestAware
    {
        private TransactionInfoVM _transaction;
        public TransactionInfoVM Transaction
        {
            get { return _transaction; }
            set { this.SetProperty(ref _transaction, value); }
        }
        
        public Action FinishInteraction { get; set; }

        private INotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, value);
                this.Transaction = (TransactionInfoVM)_notification.Content;
            }
        }

        private ClipboardService ClipboardService { get; set; }
        private ToastService ToastService { get; set; }
        public DelegateCommand CopyTransactionHashCommand { get; set; }
        public DelegateCommand CopyBlockHashCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        public TransactionDetailPopupController(ClipboardService clipboardService, ToastService toastService)
        {
            ClipboardService = clipboardService;
            ToastService = toastService;
            CopyTransactionHashCommand = new DelegateCommand(CopyTransactionHash);
            CopyBlockHashCommand = new DelegateCommand(CopyBlockHash);
            CloseCommand = new DelegateCommand(ClosePopup);
        }

        private void ClosePopup()
        {
            FinishInteraction?.Invoke();
        }
        
        private void CopyBlockHash()
        {
            ClipboardService.SetText(Transaction.BlockHash);
            ToastService.Success("Block Hash 复制成功");
        }

        private void CopyTransactionHash()
        {
            ClipboardService.SetText(Transaction.TransactionId);
            ToastService.Success("Transaction ID 复制成功");
        }

    }
}
