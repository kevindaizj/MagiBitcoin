using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Common;

namespace USDTWallet.Models.Models.Addresses
{
    public class AddressVM : ViewModelBase
    {
        private string _address;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _keyPath;
        public string KeyPath
        {
            get { return _keyPath; }
            set { SetProperty(ref _keyPath, value); }
        }

        private long _pathIndex;
        public long PathIndex
        {
            get { return _pathIndex; }
            set { SetProperty(ref _pathIndex, value); }
        }

        private long _addressType;
        public long AddressType
        {
            get { return _addressType; }
            set { SetProperty(ref _addressType, value); }
        }

        private long _addressCategory;
        public long AddressCategory
        {
            get { return _addressCategory; }
            set { SetProperty(ref _addressCategory, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private Money _balance;
        public Money Balance
        {
            get { return _balance; }
            set { SetProperty(ref _balance, value); }
        }

        private Money _usdtbalance;
        public Money USDTBalance
        {
            get { return _usdtbalance; }
            set { SetProperty(ref _usdtbalance, value); }
        }

        private string _account;
        public string Account
        {
            get { return _account; }
            set { SetProperty(ref _account, value); }
        }
    }
}
