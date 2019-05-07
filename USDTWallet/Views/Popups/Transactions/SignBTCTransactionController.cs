using NBitcoin;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Transactions;
using USDTWallet.Common;
using USDTWallet.Common.Helpers;
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

        private bool _privKeyMethod;
        public bool PrivKeyMethod
        {
            get { return _privKeyMethod; }
            set { SetProperty(ref _privKeyMethod, value); }
        }

        private string _privKey;
        public string PrivKey
        {
            get { return _privKey; }
            set { SetProperty(ref _privKey, value); }
        }

        private bool _pwdMethod = true;
        public bool PwdMethod
        {
            get { return _pwdMethod; }
            set { SetProperty(ref _pwdMethod, value); }
        }

        private SecureString _password;
        public SecureString Password
        {
            get { return _password; }
            set { this.SetProperty(ref _password, value); }
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

        private TransactionManager TxManager { get; set; }

        public SignBTCTransactionController(TransactionManager txManager)
        {
            this.TxManager = txManager;
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
            if ((PwdMethod && Password == null) || (PrivKeyMethod && string.IsNullOrWhiteSpace(PrivKey)))
                return;

            if(PwdMethod)
            {
                var pwd = SecureStringHelper.SecureStringToString(Password);
                await TxManager.SignAndSendTransaction(pwd, this.Transaction.ToHex(), this.SpentCoins);
            }
            else
            {
                await BTCOperator.Instance.SignAndSendTransactionByPrivateKey(PrivKey, this.Transaction.ToHex(), this.SpentCoins);
            }

            
            this.FinishInteraction?.Invoke();
        }
    }
}
