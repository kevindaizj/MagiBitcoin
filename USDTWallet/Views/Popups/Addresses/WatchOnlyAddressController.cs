using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Addresses;
using USDTWallet.ControlService.MsgBox;
using USDTWallet.Models.Enums.Address;

namespace USDTWallet.Views.Popups.Addresses
{
    public class WatchOnlyAddressController : BindableBase, IInteractionRequestAware
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
                if (!this.IsCustomer)
                    this.AccountName = "Company Watch-only";
                else
                    this.AccountName = "Customer Watch-only";
            }
        }

        private long AddressType { get; set; }

        private bool _isCustomer;
        public bool IsCustomer
        {
            get { return _isCustomer; }
            set { SetProperty(ref _isCustomer, value); }
        }

        private string _addresses;
        public string Addresses
        {
            get { return _addresses; }
            set { SetProperty(ref _addresses, value); }
        }

        private string _accountName;
        public string AccountName
        {
            get { return _accountName; }
            set { SetProperty(ref _accountName, value); }
        }

        public DelegateCommand ImportCommand { get; set; }

        private MessageBoxService MsgBox { get; set; }
        private AddressManager AddressManager { get; set; }

        public WatchOnlyAddressController(AddressManager addressManger, MessageBoxService msgBox)
        {
            this.ImportCommand = new DelegateCommand(ImportAddress);
            this.MsgBox = msgBox;
            this.AddressManager = addressManger;
        }

        private async void ImportAddress()
        {
            if (string.IsNullOrWhiteSpace(Addresses))
                return;

            var addrList = Addresses.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (addrList.Count <= 0)
            {
                MsgBox.Show("必须为逗号分隔的地址字符串");
            }
             
            await AddressManager.ImportWatchOnlyAddresses(addrList, this.AccountName);
            this.FinishInteraction?.Invoke();
        }
    }
}
