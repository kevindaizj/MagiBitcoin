using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Wallet;
using USDTWallet.Common;
using USDTWallet.PopupNotifications;

namespace USDTWallet.Views
{
    public class MainWindowController : ControllerBase
    {
        public InteractionRequest<WalletInitNotification> CreateWalletPopupRequest { get; set; }
        public InteractionRequest<WalletInitNotification> LoginWalletPopupRequest { get; set; }

        public DelegateCommand OnLoadCommand { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string network;
        public string Network
        {
            get { return network; }
            set { SetProperty(ref network, value); }
        }

        private WalletManager WalletManager { get; set; }

        public MainWindowController(WalletManager walletManager)
        {
            this.Name = "Kevin";
            this.Network = "Localhost:8889";
            this.CreateWalletPopupRequest = new InteractionRequest<WalletInitNotification>();
            this.LoginWalletPopupRequest = new InteractionRequest<WalletInitNotification>();
            this.OnLoadCommand = new DelegateCommand(OnLoadHandler);
            this.WalletManager = walletManager;
        }

        private void OnLoadHandler()
        {
            bool hasWallet = WalletManager.CheckAnyWalletExisted();

            if(!hasWallet)
            {
                CreateWalletPopupRequest.Raise(new WalletInitNotification { Title = "创建新钱包" }, this.Callback);
            }
            else
            {
                LoginWalletPopupRequest.Raise(new WalletInitNotification { Title = "登录" }, this.Callback);
            }
        }

        private void Callback(WalletInitNotification obj)
        {
            var a = "";
        }
    }
}
