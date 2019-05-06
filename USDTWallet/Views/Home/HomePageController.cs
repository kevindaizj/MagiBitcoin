using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using USDTWallet.Biz.Accounts;
using USDTWallet.Biz.Addresses;
using USDTWallet.Biz.Wallet;
using USDTWallet.Common;
using USDTWallet.Common.Operators;
using USDTWallet.ControlService.Clipb;
using USDTWallet.ControlService.MsgBox;
using USDTWallet.ControlService.Toast;
using USDTWallet.Events;
using USDTWallet.Models.Enums.Address;
using USDTWallet.Models.Models.Addresses;

namespace USDTWallet.Views.Home
{
    public class HomePageController : ControllerBase
    {
        public string Title { get { return "主页"; } }
        public string IconScr { get { return "/Images/home.png"; } }
        public string SelectedIconScr { get { return "/Images/home_blue.png"; } }

        private bool _ready;
        public bool Ready
        {
            get { return _ready; }
            set { SetProperty(ref _ready, value); }
        }

        private AddressVM _rootAddress;
        public AddressVM RootAddress
        {
            get { return _rootAddress; }
            set { SetProperty(ref _rootAddress, value); }
        }

        private bool _anyCompanyAddress;
        public bool AnyCompanyAddress
        {
            get { return _anyCompanyAddress; }
            set { SetProperty(ref _anyCompanyAddress, value); }
        }

        private bool _anyCustomerAddress;
        public bool AnyCustomerAddress
        {
            get { return _anyCustomerAddress; }
            set { SetProperty(ref _anyCustomerAddress, value); }
        }

        public ObservableCollection<AddressVM> CompanyAddresses { get; set; }
        public ObservableCollection<AddressVM> CustomerAddresses { get; set; }

        
        public AddressManager AddressManager { get; set; }
        private MessageBoxService MsgBox { get; set; }
        private ClipboardService Clipboard { get; set; }
        private ToastService Toast { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public DelegateCommand<string> CreateAddressCommand { get; set; }
        public InteractionRequest<INotification> CreateAddressPopupRequest { get; set; }

        public DelegateCommand<string> AddWachtOnlyAddressCommand { get; set; }
        public InteractionRequest<INotification> AddWachtOnlyAddressPopupRequest { get; set; }
        
        public DelegateCommand<string> AccountDetailCommand { get; set; }

        private DispatcherTimer BalanceTimer { get; set; }


        public HomePageController(AddressManager addressManager, IEventAggregator eventAggregator,
                    MessageBoxService msgBox, ClipboardService clip, ToastService toast)
        {
            this.AddressManager = addressManager;
            this.MsgBox = msgBox;
            this.Clipboard = clip;
            this.Toast = toast;
            this.EventAggregator = eventAggregator;

            this.RootAddress = new AddressVM();
            this.CompanyAddresses = new ObservableCollection<AddressVM>();
            this.CustomerAddresses = new ObservableCollection<AddressVM>();

            this.EventAggregator.GetEvent<LoginSuccessEvent>().Subscribe(Initialize);
            this.CreateAddressCommand = new DelegateCommand<string>(CreateAddress);
            this.CreateAddressPopupRequest = new InteractionRequest<INotification>();

            this.AddWachtOnlyAddressCommand = new DelegateCommand<string>(AddWatchOnlyAddress);
            this.AddWachtOnlyAddressPopupRequest = new InteractionRequest<INotification>();

            this.AccountDetailCommand = new DelegateCommand<string>(addr =>
            {
                this.Clipboard.SetText(addr);
                this.Toast.Success("成功复制地址");
            });

            EventAggregator.GetEvent<CreateAddressSuccessEvent>().Subscribe(AfterCreateAddress);
        }

        private void Initialize()
        {
            this.Ready = true;
            
            this.RootAddress = AddressManager.GetRootAddress();

            var companyAddresses = AddressManager.GetRootAddressesByType(CustomAddressType.Company);
            this.AnyCompanyAddress = companyAddresses.Count() > 0;
            this.CompanyAddresses.Clear();
            this.CompanyAddresses.AddRange(companyAddresses);

            var customerAddresses = AddressManager.GetRootAddressesByType(CustomAddressType.Customer);
            this.AnyCustomerAddress = companyAddresses.Count() > 0;
            this.CustomerAddresses.Clear();
            this.CustomerAddresses.AddRange(customerAddresses);

            ResetBalanceTimer();
        }

        private void CreateAddress(string type)
        {
            long addressType = long.Parse(type);
            var title = "创建新公司地址";
            if (addressType == (long)CustomAddressType.Customer)
                title = "创建新客户地址";

            CreateAddressPopupRequest.Raise(new Notification { Title = title, Content = addressType });
        }

        private void AfterCreateAddress(long addressType)
        {
            var addresses = AddressManager.GetRootAddressesByType((CustomAddressType)addressType);
            if(addressType == (long)CustomAddressType.Company)
            {
                this.AnyCompanyAddress = addresses.Count() > 0;
                CompanyAddresses.Clear();
                CompanyAddresses.AddRange(addresses);
            }
            else
            {
                this.AnyCustomerAddress = addresses.Count() > 0;
                CustomerAddresses.Clear();
                CustomerAddresses.AddRange(addresses);
            }
        }


        private void AddWatchOnlyAddress(string type)
        {
            long addressType = long.Parse(type);
            var title = "Watch Only公司地址";
            if (addressType == (long)CustomAddressType.Customer)
                title = "Watch Only客户地址";

            AddWachtOnlyAddressPopupRequest.Raise(new Notification { Title = title, Content = addressType });
        }


        private void ResetBalanceTimer()
        {
            BalanceTimer?.Stop();
            BalanceTimer = null;

            BalanceTimer = new DispatcherTimer();
            BalanceTimer.Interval = new TimeSpan(0, 0, 3);
            BalanceTimer.Tick += GetBalances;
            BalanceTimer.Start();
        }

        private async void GetBalances(object sender, EventArgs e)
        {
            var addressList = CompanyAddresses.Select(q => q.Address).ToList();
            addressList.AddRange(CustomerAddresses.Select(q => q.Address));

            var usdtBalanceDict = await AddressManager.BatchGetUSDTBalanceViaNode(addressList);
            var balanceDict = AddressManager.BatchGetBTCBalanceViaNode(addressList);

            foreach (var addr in CompanyAddresses)
            {
                addr.Balance = balanceDict[addr.Address];
                addr.USDTBalance = usdtBalanceDict[addr.Address];
            }
            foreach (var addr in CustomerAddresses)
            {
                addr.Balance = balanceDict[addr.Address];
                addr.USDTBalance = usdtBalanceDict[addr.Address];
            }
        }

    }
}
