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
using USDTWallet.Common.Helpers;
using USDTWallet.ControlService.MsgBox;

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

        private bool _isCreating;
        public bool IsCreating
        {
            get { return _isCreating; }
            set { this.SetProperty(ref _isCreating, value); }
        }

        public Action FinishInteraction { get; set; }

        private INotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, value);
            }
        }

        public DelegateCommand ConfirmCommand { get; set; }
        
        private MessageBoxService MessageBoxService { get; set; }
        private IEventAggregator EventAggregator { get; set; }

        public CreateWalletController(MessageBoxService msgBoxService, IEventAggregator eventAggregator)
        {
            this.ConfirmCommand = new DelegateCommand(CreateAccount, CanCreateAccount);
            this.MessageBoxService = msgBoxService;
            this.EventAggregator = eventAggregator;
        }

        private bool CanCreateAccount()
        {
            return true;
        }

        private async void CreateAccount()
        {
            var pwd = SecureStringHelper.SecureStringToString(Password);
            var confimrPwd = SecureStringHelper.SecureStringToString(ConfirmPassword);
            if (pwd != confimrPwd)
            {
                MessageBoxService.Show("两次输入的密码不匹配");
                return;
            }

            //this.IsCreating = true;

            //var service = new AccountBiz();
            //await service.CreateAccount(pwd);

            //this.EventAggregator.GetEvent<CreateNewAccountEvent>().Publish();
            //ToastService.Success("成功创建账户");

            //this.IsCreating = false;
            FinishInteraction?.Invoke();
        }
    }
}
