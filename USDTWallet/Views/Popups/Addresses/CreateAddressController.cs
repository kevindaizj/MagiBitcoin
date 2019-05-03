using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Addresses;
using USDTWallet.Biz.Wallet;
using USDTWallet.ControlService.Clipb;
using USDTWallet.Events;
using USDTWallet.Models.Enums.Address;
using USDTWallet.Models.Models.Addresses;

namespace USDTWallet.Views.Popups.Addresses
{
    public class CreateAddressController : BindableBase, IInteractionRequestAware
    {
        public Action FinishInteraction { get; set; }

        private INotification _notification;
        public INotification Notification
        {
            get { return _notification; }
            set
            {
                SetProperty(ref _notification, value);
                this.AddressType = (long)_notification.Content;
                this.IsCustomer = AddressType == (long)CustomAddressType.Customer;
            }
        }

        private long AddressType { get; set; }

        private bool _isCustomer;
        public bool IsCustomer
        {
            get { return _isCustomer; }
            set { SetProperty(ref _isCustomer, value); }
        }

        public ObservableCollection<AddressCategorySelectItem> CategoryItems { get; set; }

        private long _selectedCategoryId;
        public long SelectedCategoryId
        {
            get { return _selectedCategoryId; }
            set { SetProperty(ref _selectedCategoryId, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string account;
        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value); }
        }

        private int _customerCount = 2;
        public int CustomerCount
        {
            get { return _customerCount; }
            set { SetProperty(ref _customerCount, value); }
        }

        private bool _export = true;
        public bool Export
        {
            get { return _export; }
            set { SetProperty(ref _export, value); }
        }

        private bool _showExport;
        public bool ShowExport
        {
            get { return _showExport; }
            set { SetProperty(ref _showExport, value); }
        }

        private string _exportAddresses;
        public string ExportAddresses
        {
            get { return _exportAddresses; }
            set { SetProperty(ref _exportAddresses, value); }
        }

        public DelegateCommand ConfirmCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        private IEventAggregator EventAggregator { get; set; }
        
        public AddressManager AddressManager { get; set; }

        private ClipboardService Clip { get; set; }

        public CreateAddressController(AddressManager addressManger, IEventAggregator eventAggregator, ClipboardService clip)
        {
            var categoryItems = new List<AddressCategorySelectItem>
            {
                new AddressCategorySelectItem { Label = "付款地址", Value = (long)AddressCategory.Payer },
                new AddressCategorySelectItem { Label = "收款地址", Value = (long)AddressCategory.Receiver },
            };
            this.CategoryItems = new ObservableCollection<AddressCategorySelectItem>(categoryItems);
            this.SelectedCategoryId = (long)AddressCategory.Payer;

            this.ConfirmCommand = new DelegateCommand(CreateNewAddress);
            this.CloseCommand = new DelegateCommand(Close);
            this.EventAggregator = eventAggregator;
            this.Clip = clip;
            
            this.AddressManager = addressManger;
        }

        private void CreateNewAddress()
        {
            if (AddressType == (long)CustomAddressType.Company)
                this.CreateCompanyAddress();
            else
                this.CreateCustomerAddress();
        }

        private void CreateCompanyAddress()
        {
            AddressManager.CreateCompanyAddress(SelectedCategoryId, Name, Account);
            this.Close();
        }

        private void CreateCustomerAddress()
        {
            var addresses = AddressManager.CreateCustomerAddresses(this.CustomerCount);
            this.ExportAddresses = string.Join("," + Environment.NewLine, addresses);
            this.Clip.SetText(ExportAddresses);
            if (this.Export)
                this.ShowExport = true;
            else
                this.Close();
        }


        private void Close()
        {
            this.EventAggregator.GetEvent<CreateAddressSuccessEvent>().Publish(AddressType);
            this.FinishInteraction?.Invoke();
        }
    }
}
