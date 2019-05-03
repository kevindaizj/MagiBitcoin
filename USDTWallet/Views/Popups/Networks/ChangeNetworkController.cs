using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common;
using USDTWallet.Common.Helpers;
using USDTWallet.Common.Operators;
using USDTWallet.Events;
using USDTWallet.Models.Enums.Network;

namespace USDTWallet.Views.Popups.Networks
{
    public class ChangeNetworkController : ControllerBase, IInteractionRequestAware
    {
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        private string url;
        public string RpcUrl
        {
            get { return url; }
            set { this.SetProperty(ref url, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private SecureString _password;
        public SecureString Password
        {
            get { return _password; }
            set { this.SetProperty(ref _password, value); }
        }

        private string _btnContent = "提交";
        public string BtnContent
        {
            get { return _btnContent; }
            set { this.SetProperty(ref _btnContent, value); }
        }

        public DelegateCommand ConfirmCommand { get; set; }
        private IEventAggregator EventAggregator { get; set; }
        public object NetworksOperator { get; private set; }

        public ChangeNetworkController(IEventAggregator eventAggregator)
        {
            this.RpcUrl = NetworkOperator.Instance.RpcUri.AbsoluteUri;
            this.UserName = NetworkOperator.Instance.Credential.UserName;
            this.EventAggregator = eventAggregator;
            ConfirmCommand = new DelegateCommand(Confirm);
        }

        private void Confirm()
        {
            if (null == this.Password)
                return;
            
            this.BtnContent = "提交中...";

            var pwd = SecureStringHelper.SecureStringToString(Password);
            NetworkOperator.Instance.ChangeNetwork(CustomNetworkType.Regtest, RpcUrl, UserName, pwd);
            this.EventAggregator.GetEvent<ChangeNetworkEvent>().Publish();

            this.BtnContent = "提交";
            this.FinishInteraction?.Invoke();
        }
    }
}
