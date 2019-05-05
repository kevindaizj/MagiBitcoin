using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Transactions;
using USDTWallet.Common;
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
        
        private BTCTransactionManager TxManager { get; set; }

        public GenBTCTransactionController(BTCTransactionManager txManager)
        {
            this.TxManager = txManager;
        }

        private async void GenTx()
        {
            var tx = await TxManager.BuildUnsignedTransaction(this.TransferInfo);
            this.TxHex = tx.ToHex();
            this.TxJson = tx.ToString();
        }
    }
}
