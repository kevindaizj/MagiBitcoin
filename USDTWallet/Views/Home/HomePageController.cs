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
using USDTWallet.Biz.Accounts;
using USDTWallet.Biz.Addresses;
using USDTWallet.Biz.Wallet;
using USDTWallet.Common;
using USDTWallet.ControlService.MsgBox;
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

        public ObservableCollection<AddressVM> CompanyAddresses { get; set; }
        public ObservableCollection<AddressVM> CustomerAddresses { get; set; }


        public WalletManager WalletManager { get; set; }
        public AddressManager AddressManager { get; set; }
        private MessageBoxService MsgBox { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public DelegateCommand<string> CreateAddressCommand { get; set; }

        public HomePageController(WalletManager walletManager, AddressManager addressManager, MessageBoxService msgBox, IEventAggregator eventAggregator)
        {
            this.WalletManager = walletManager;
            this.AddressManager = addressManager;
            this.MsgBox = msgBox;
            this.EventAggregator = eventAggregator;

            this.RootAddress = new AddressVM();
            this.CompanyAddresses = new ObservableCollection<AddressVM>();
            this.CustomerAddresses = new ObservableCollection<AddressVM>();

            this.EventAggregator.GetEvent<LoginSuccessEvent>().Subscribe(Initialize);
            this.CreateAddressCommand = new DelegateCommand<string>(CreateAddress);
        }
        

        private void Initialize()
        {
            this.Ready = true;

            var wallet = WalletManager.GetActiveWallet();
            this.RootAddress = AddressManager.GetRootAddress(wallet.Id);

            var companyAddresses = AddressManager.GetRootAddressesByType(wallet.Id, CustomAddressType.Company);
            this.CompanyAddresses.Clear();
            this.CompanyAddresses.AddRange(companyAddresses);

            var customerAddresses = AddressManager.GetRootAddressesByType(wallet.Id, CustomAddressType.Customer);
            this.CustomerAddresses.Clear();
            this.CustomerAddresses.AddRange(customerAddresses);
        }

        private void CreateAddress(string type)
        {
            //throw new NotImplementedException();
        }
    }
}
