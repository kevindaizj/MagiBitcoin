using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Wallet;
using USDTWallet.Common;
using USDTWallet.Common.Helpers;
using USDTWallet.ControlService.MsgBox;
using USDTWallet.PopupNotifications;

namespace USDTWallet.Views.Popups.Wallets
{
    public class LoginWalletController : BindableBase, IInteractionRequestAware
    {
        private string _walletName;
        public string WalletName
        {
            get { return _walletName; }
            set { SetProperty(ref _walletName, value); }
        }

        private SecureString _password;
        public SecureString Password
        {
            get { return _password; }
            set { this.SetProperty(ref _password, value); }
        }

        private bool _isLogining;
        public bool IsLogining
        {
            get { return _isLogining; }
            set { SetProperty(ref _isLogining, value); }
        }

        public Action FinishInteraction { get; set; }

        private WalletInitNotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, (WalletInitNotification)value);
            }
        }

        public DelegateCommand LoginCommand { get; set; }

        private WalletManager WalletManager { get; set; }
        private MessageBoxService Msgbox { get; set; }

        public LoginWalletController(WalletManager walletManager, MessageBoxService msgbox)
        {
            this.LoginCommand = new DelegateCommand(Login, CanLogin);
            this.Msgbox = msgbox;
            this.WalletManager = walletManager;
            this.initialize();
        }

        private void initialize()
        {
            var wallet = WalletManager.GetActiveWallet();
            this.WalletName = wallet.WalletName;
        }

        private bool CanLogin()
        {
            return true;
        }

        private void Login()
        {
            if (null == this.Password)
                return;
            
            this.IsLogining = true;
            var pwd = SecureStringHelper.SecureStringToString(Password);
            bool status = WalletManager.Login(pwd);
            if(!status)
            {
                this.IsLogining = false;
                this.Msgbox.Show("密码错误");
                return;
            }
            this._notification.Success = true;
            this.FinishInteraction?.Invoke();
        }
    }
}
