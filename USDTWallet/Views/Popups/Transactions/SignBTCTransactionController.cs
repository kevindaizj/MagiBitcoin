using NBitcoin;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common;
using USDTWallet.Common.Operators;
using USDTWallet.PopupNotifications;

namespace USDTWallet.Views.Popups.Transactions
{
    public class SignBTCTransactionController : ControllerBase, IInteractionRequestAware
    {
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

        private string _privKey;
        public string PrivKey
        {
            get { return _privKey; }
            set { SetProperty(ref _privKey, value); }
        }

        private Transaction Transaction { get; set; }

        public Action FinishInteraction { get; set; }

        private INotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, value);
                this.TxHex = (string)_notification.Content;
                this.InitData();
            }
        }

        public DelegateCommand SendCommand { get; set; }

        public SignBTCTransactionController()
        {
            this.SendCommand = new DelegateCommand(SignAndSendTx);
        }


        private void InitData()
        {
            this.Transaction = BTCOperator.Instance.ParseTransaction(this.TxHex);
            this.TxJson = this.Transaction.ToString();
        }


        private async void SignAndSendTx()
        {
            if (string.IsNullOrWhiteSpace(PrivKey))
                return;

            await BTCOperator.Instance.SignAndSendTransaction(PrivKey, this.TxHex);
            this.FinishInteraction?.Invoke();
        }
    }
}
