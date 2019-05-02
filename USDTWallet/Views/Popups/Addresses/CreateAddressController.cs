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
            }
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

        public DelegateCommand ConfirmCommand { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public WalletManager WalletManager { get; set; }
        public AddressManager AddressManager { get; set; }

        public CreateAddressController(WalletManager walletManager, AddressManager addressManger, IEventAggregator eventAggregator)
        {
            var categoryItems = new List<AddressCategorySelectItem>
            {
                new AddressCategorySelectItem { Label = "付款地址", Value = (long)AddressCategory.Payer },
                new AddressCategorySelectItem { Label = "收款地址", Value = (long)AddressCategory.Receiver },
            };
            this.CategoryItems = new ObservableCollection<AddressCategorySelectItem>(categoryItems);
            this.SelectedCategoryId = (long)AddressCategory.Payer;

            this.ConfirmCommand = new DelegateCommand(CreateNewAddress);
            this.EventAggregator = eventAggregator;

            this.WalletManager = walletManager;
            this.AddressManager = addressManger;
        }

        private void CreateNewAddress()
        {
            var wallet = WalletManager.GetActiveWallet();
            AddressManager.CreateNewAddress(wallet.Id, (long)Notification.Content, SelectedCategoryId, Name, Account);
            this.EventAggregator.GetEvent<CreateAddressSuccessEvent>().Publish((long)Notification.Content);
            this.FinishInteraction?.Invoke();
        }
    }
}
