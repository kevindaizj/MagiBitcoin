using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Accounts;
using USDTWallet.Biz.Wallet;
using USDTWallet.Common;
using USDTWallet.ControlService.MsgBox;

namespace USDTWallet.Views.Home
{
    public class HomePageController : ControllerBase
    {
        public string Title { get { return "主页"; } }
        public string IconScr { get { return "/Images/home.png"; } }
        public string SelectedIconScr { get { return "/Images/home_blue.png"; } }

        //public InteractionRequest<INotification> CreateWalletPopupRequest { get; set; }
        //public DelegateCommand OnLoadCommand { get; set; }

        public WalletManager WalletManager { get; set; }

        private MessageBoxService MsgBox { get; set; }

        public HomePageController(WalletManager walletManager, MessageBoxService msgBox)
        {
            this.WalletManager = walletManager;
            this.MsgBox = msgBox;
            //this.CreateWalletPopupRequest = new InteractionRequest<INotification>();
            //this.OnLoadCommand = new DelegateCommand(onLoadHandler);
            ////this.MsgBox.Show("hello");
            //CreateWalletPopupRequest.Raise(new Notification { Title = "创建新钱包" });
        }

        private void onLoadHandler()
        {
            //this.MsgBox.Show("hello ");
            //CreateWalletPopupRequest.Raise(new Notification { Title = "创建新钱包" });
        }

        
    }
}
