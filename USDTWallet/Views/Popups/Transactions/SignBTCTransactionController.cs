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
using USDTWallet.Models.Models.Transactions;
using USDTWallet.PopupNotifications;

namespace USDTWallet.Views.Popups.Transactions
{
    public class SignBTCTransactionController : ControllerBase, IInteractionRequestAware
    {
        
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

        private List<Coin> SpentCoins { get; set; }

        public Action FinishInteraction { get; set; }

        private INotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, value);
                this.InitData((UnsignTransactionResult)_notification.Content);
            }
        }

        public DelegateCommand SendCommand { get; set; }

        public SignBTCTransactionController()
        {
            this.SendCommand = new DelegateCommand(SignAndSendTx);
        }


        private void InitData(UnsignTransactionResult txInfo)
        {
            this.Transaction = txInfo.Transaction;
            this.SpentCoins = txInfo.ToSpentCoins;
            this.TxJson = this.Transaction.ToString();
        }


        private async void SignAndSendTx()
        {
            if (string.IsNullOrWhiteSpace(PrivKey))
                return;

            await BTCOperator.Instance.SignAndSendTransaction(PrivKey, this.Transaction.ToHex(), this.SpentCoins);
            this.FinishInteraction?.Invoke();
        }
    }
}
