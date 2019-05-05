using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using USDTWallet.Biz.Wallet;
using USDTWallet.Common;
using USDTWallet.Common.Helpers;
using USDTWallet.Common.Operators;
using USDTWallet.Events;
using USDTWallet.PopupNotifications;

namespace USDTWallet.Views
{
    public class MainWindowController : ControllerBase
    {
        public InteractionRequest<WalletInitNotification> CreateWalletPopupRequest { get; set; }
        public InteractionRequest<WalletInitNotification> LoginWalletPopupRequest { get; set; }

        public InteractionRequest<INotification> ChangeNetworkPopupRequest { get; set; }

        public DelegateCommand OnLoadCommand { get; set; }

        public DelegateCommand OpenDatabaseDirCommand { get; set; }
        public DelegateCommand OpenLogDirCommand { get; set; }

        public DelegateCommand ChangeNetworkCommand { get; set; }

        public DelegateCommand<string> CreateAddressCommand { get; set; }

        private DispatcherTimer CheckNetworkTimer { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string network = "连接中...";
        public string Network
        {
            get { return network; }
            set { SetProperty(ref network, value); }
        }

        private string _networkTagBg = "#E8e8ed";
        public string NetworkTagBg
        {
            get { return _networkTagBg; }
            set { SetProperty(ref _networkTagBg, value); }
        }
        
        private bool _networkActive;
        public bool IsNetworkActive
        {
            get { return _networkActive; }
            set
            {
                SetProperty(ref _networkActive, value);
                if (_networkActive)
                {
                    this.NetworkTagBg = "#00D55F";
                }
                else
                {
                    this.NetworkTagBg = "#E8e8ed";
                }
            }
        }



        private WalletManager WalletManager { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public MainWindowController(WalletManager walletManager, IEventAggregator eventAggregator)
        {
            this.CreateWalletPopupRequest = new InteractionRequest<WalletInitNotification>();
            this.LoginWalletPopupRequest = new InteractionRequest<WalletInitNotification>();
            this.ChangeNetworkPopupRequest = new InteractionRequest<INotification>();
            this.OnLoadCommand = new DelegateCommand(OnLoadHandler);
            this.OpenDatabaseDirCommand = new DelegateCommand(OpenDatabaseDirectory);
            this.OpenLogDirCommand = new DelegateCommand(OpenLogDirectory);
            this.ChangeNetworkCommand = new DelegateCommand(this.OpenChangeNetworkPopup);
            this.WalletManager = walletManager;
            this.EventAggregator = eventAggregator;
            this.EventAggregator.GetEvent<ChangeNetworkEvent>().Subscribe(() => this.ChangeNetwork());
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

            this.ChangeNetwork();
        }

        private void Callback(WalletInitNotification notification)
        {
            if (notification.Success)
                this.EventAggregator.GetEvent<LoginSuccessEvent>().Publish();
        }

        private void OpenDatabaseDirectory()
        {
            var path = DataDirectoryHelper.GetDatabaseDirectoryPath();
            System.Diagnostics.Process.Start(path);
        }

        private void OpenLogDirectory()
        {
            var path = DataDirectoryHelper.GetLogDirectoryPath();
            System.Diagnostics.Process.Start(path);
        }

        private void OpenChangeNetworkPopup()
        {
            ChangeNetworkPopupRequest.Raise(new Notification { Title = "更改RPC连接" });
        }

        private void ChangeNetwork()
        {
            this.Network = NetworkOperator.Instance.RpcUri.AbsoluteUri;
            DispatcherHelper.Invoke(() =>
            {
                this.CheckNetwork(null, null);
                this.ResetCheckNetworkTimer();
            });
        }


        private void ResetCheckNetworkTimer()
        {
            CheckNetworkTimer?.Stop();
            CheckNetworkTimer = null;

            CheckNetworkTimer = new DispatcherTimer();
            CheckNetworkTimer.Interval = new TimeSpan(0, 0, 3);
            CheckNetworkTimer.Tick += CheckNetwork;
            CheckNetworkTimer.Start();
        }

        private async void CheckNetwork(object sender, EventArgs e)
        {
            this.IsNetworkActive = await NetworkOperator.Instance.CheckNetwork();
            if (this.IsNetworkActive)
                this.Network = NetworkOperator.Instance.RpcUri.AbsoluteUri;
            else
                this.Network = "连接中断";
        }
    }
}
