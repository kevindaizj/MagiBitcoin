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

        private IEventAggregator EventAggregator { get; set; }

        public DelegateCommand<string> CreateAddressCommand { get; set; }

        public InteractionRequest<INotification> CreateAddressPopupRequest { get; set; }

        public HomePageController(AddressManager addressManager, MessageBoxService msgBox, IEventAggregator eventAggregator)
        {
            this.AddressManager = addressManager;
            this.MsgBox = msgBox;
            this.EventAggregator = eventAggregator;

            this.RootAddress = new AddressVM();
            this.CompanyAddresses = new ObservableCollection<AddressVM>();
            this.CustomerAddresses = new ObservableCollection<AddressVM>();

            this.EventAggregator.GetEvent<LoginSuccessEvent>().Subscribe(Initialize);
            this.CreateAddressCommand = new DelegateCommand<string>(CreateAddress);
            this.CreateAddressPopupRequest = new InteractionRequest<INotification>();

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
    }
}
