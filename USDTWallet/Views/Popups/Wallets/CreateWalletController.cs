using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Wallet;
using USDTWallet.Common.Helpers;
using USDTWallet.ControlService.MsgBox;
using USDTWallet.PopupNotifications;

namespace USDTWallet.Views.Popups.Wallets
{
    public class CreateWalletController : BindableBase, IInteractionRequestAware
    {
        private SecureString _password;
        public SecureString Password
        {
            get { return _password; }
            set { this.SetProperty(ref _password, value); }
        }

        private SecureString _confirmPassword;
        public SecureString ConfirmPassword
        {
            get { return _confirmPassword; }
            set { this.SetProperty(ref _confirmPassword, value); }
        }
        
        public bool ShowEdit
        {
            get { return !this.IsCreating && !this.IsCompleted; }
        }

        private bool _isCreating;
        public bool IsCreating
        {
            get { return _isCreating; }
            set { this.SetProperty(ref _isCreating, value); this.RaisePropertyChanged("ShowEdit"); }
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { SetProperty(ref _isCompleted, value); this.RaisePropertyChanged("ShowEdit"); }
        }

        private string _mnemonicWords;
        public string MnemonicWords
        {
            get { return _mnemonicWords; }
            set { SetProperty(ref _mnemonicWords, value); }
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

        public DelegateCommand ConfirmCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        private MessageBoxService MessageBoxService { get; set; }
        private IEventAggregator EventAggregator { get; set; }
        private WalletManager WalletManager { get; set; }

        public CreateWalletController(MessageBoxService msgBoxService, IEventAggregator eventAggregator, WalletManager walletManager)
        {
            this.ConfirmCommand = new DelegateCommand(CreateWallet, CanCreateWallet);
            this.CloseCommand = new DelegateCommand(CloseWin);
            this.MessageBoxService = msgBoxService;
            this.EventAggregator = eventAggregator;
            this.WalletManager = walletManager;
        }
        
        private bool CanCreateWallet()
        {
            return true;
        }

        private void CreateWallet()
        {
            var pwd = SecureStringHelper.SecureStringToString(Password);
            var confimrPwd = SecureStringHelper.SecureStringToString(ConfirmPassword);
            if (pwd != confimrPwd)
            {
                MessageBoxService.Show("两次输入的密码不匹配");
                return;
            }

            this.IsCreating = true;

            var result = WalletManager.CreateWallet(pwd);
            this.MnemonicWords = string.Join(" ", result.MnemonicWords);

            this.IsCreating = false;
            this.IsCompleted = true;
        }

        private void CloseWin()
        {
            this._notification.Success = true;
            FinishInteraction?.Invoke();
        }
    }
}
