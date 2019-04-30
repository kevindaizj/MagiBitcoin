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

        public InteractionRequest<INotification> CreateWalletPopupRequest { get; set; }

        public WalletManager WalletManager { get; set; }

        private MessageBoxService MsgBox { get; set; }

        public HomePageController(WalletManager walletManager, MessageBoxService msgBox)
        {
            this.WalletManager = walletManager;
            this.MsgBox = msgBox;
            this.CreateWalletPopupRequest = new InteractionRequest<INotification>();
            this.MsgBox.Show("hello");
            CreateWalletPopupRequest.Raise(new Notification { Title = "创建新钱包" });
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            bool hasWallet = WalletManager.CheckAnyWalletExisted();
            if (!hasWallet)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
                DispatcherHelper.Invoke(() =>
                {
                    this.MsgBox.Show("hello");
                    CreateWalletPopupRequest.Raise(new Notification { Title = "创建新钱包" });
                });

                //var result = WalletManager.CreateWallet("");
            }
        }

        //protected override void On()
        //{
            
        //}
    }
}
