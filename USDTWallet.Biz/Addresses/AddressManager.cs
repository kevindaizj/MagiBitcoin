using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Operators;
using USDTWallet.Dao.Address;
using USDTWallet.Models.Enums.Address;
using USDTWallet.Models.Models.Addresses;

namespace USDTWallet.Biz.Addresses
{
    public class AddressManager : BizBase
    {
        private AddressDao AddressDao { get; set; }

        public AddressManager(AddressDao addressDao)
        {
            this.AddressDao = addressDao;
        }

        public AddressVM GetRootAddress()
        {
            var addressInfo = AddressDao.GetRootAddress(CurrentWallet.Id);
            return new AddressVM
            {
                Address = addressInfo.Address,
                KeyPath = addressInfo.KeyPath,
                PathIndex = addressInfo.PathIndex,
                AddressType = addressInfo.AddressType,
                AddressCategory = addressInfo.AddressCategory,
                Name = addressInfo.Name,
                Balance = new Money((decimal)addressInfo.Balance, MoneyUnit.BTC),
                Account = addressInfo.Account
            };
        }

        public List<AddressVM> GetRootAddressesByType(CustomAddressType type)
        {
            var addresses = AddressDao.GetAddressesByType(CurrentWallet.Id, (long)type);
            var results = new List<AddressVM>();

            foreach(var a in addresses)
            {
                results.Add(new AddressVM
                {
                    Address = a.Address,
                    KeyPath = a.KeyPath,
                    PathIndex = a.PathIndex,
                    AddressType = a.AddressType,
                    AddressCategory = a.AddressCategory,
                    Name = a.Name,
                    Balance = new Money((decimal)a.Balance, MoneyUnit.BTC),
                    Account = a.Account
                });
            }

            return results;
        }

        /// <summary>
        /// Create new non-root address
        /// </summary>
        public void CreateCompanyAddress(long addressCategory, string name, string account)
        {
            long addressType = (long)CustomAddressType.Company;
            var rootXPrivKey = ExtKey.Parse(CurrentWallet.RootXPrivKey);

            var parentKeyPath = new KeyPath($"/{addressType}'/{addressCategory}'");
            long maxIndex = AddressDao.GetMaxPathIndex(CurrentWallet.Id, parentKeyPath.ToString());
            long currentIndex = maxIndex + 1;

            var keyPath = parentKeyPath.Derive((uint)currentIndex);
            var address = KeyOperator.Instance.DeriveNewAddress(rootXPrivKey, keyPath);

            var addressInfo = new AddressInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                Address = address,
                ExtPubKeyWif = null,
                WalletId = CurrentWallet.Id,
                Network = (NetworkOperator.Instance.Network == Network.Main) ? (int)NetworkType.Mainnet : (int)NetworkType.Testnet,
                KeyPath = keyPath.ToString(),
                ParentKeyPath = parentKeyPath.ToString(),
                PathIndex = currentIndex,
                AddressType = addressType,
                AddressCategory = addressCategory,
                Name = (addressCategory == (long)AddressCategory.Receiver ? "收款" : "付款") + " " + currentIndex,
                Account = account
            };

            AddressDao.Create(addressInfo);

        }

        public List<string> CreateCustomerAddresses(int count)
        {
            var customerId = AddressDao.GetMaxCustomerId(CurrentWallet.Id);

            var rootXPrivKey = ExtKey.Parse(CurrentWallet.RootXPrivKey);

            var addresses = new List<AddressInfo>();

            for(int i = 0; i < count; i++)
            {
                ++customerId;
                var keyPath = new KeyPath($"/{(int)CustomAddressType.Customer}'/{customerId}'");
                var address = KeyOperator.Instance.DeriveNewAddress(rootXPrivKey, keyPath);
                addresses.Add(new AddressInfo
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Address = address,
                    WalletId = CurrentWallet.Id,
                    Network = (NetworkOperator.Instance.Network == Network.Main) ? (int)NetworkType.Mainnet : (int)NetworkType.Testnet,
                    KeyPath = keyPath.ToString(),
                    CustomerId = customerId,
                    PathIndex = customerId,
                    AddressType = (long)CustomAddressType.Customer,
                    AddressCategory = (long)AddressCategory.Receiver, 
                    Name = "客户" + customerId + "--充币"
                });
            }
            
            AddressDao.BatchCreate(addresses);

            return addresses.Select(o => o.Address).ToList();
        }
        
        public async Task<Money> GetBTCBalance(string address)
        {
            var balance = await BTCOperator.Instance.GetBalanceByAddress(address);
            return balance;
        }

        public async Task<Money> GetUSDTBalance(string address)
        {
            var balance = await USDTOperator.Instance.GetBalanceByAddress(address);
            return balance;
        }


        public async Task ImportWatchOnlyAddresses(List<string> addresses, string label)
        {
            bool valid = BTCOperator.Instance.ValidateAddresses(addresses);
            if (!valid)
                throw new WTException(ExceptionCode.InvalidAddress, "地址格式不正确");

            await BTCOperator.Instance.ImportWatchOnlyAddresses(addresses, label);
        }


        public async Task ImportWatchAddressesWithPrivKeys(List<string> addresses, string accountName)
        {
            bool valid = BTCOperator.Instance.ValidateAddresses(addresses);
            if (!valid)
                throw new WTException(ExceptionCode.InvalidAddress, "地址格式不正确");

            var addressInfos = AddressDao.GetByAddresses(CurrentWallet.Id, addresses);
            var privateKeys = new List<string>();
            var network = NetworkOperator.Instance.Network;

            foreach(var addr in addressInfos)
            {
                var keyPath = new KeyPath(addr.KeyPath);
                var rootXPrivKey = ExtKey.Parse(CurrentWallet.RootXPrivKey, network);
                var xPrivKey = rootXPrivKey.Derive(keyPath);
                var privKeyWif = xPrivKey.PrivateKey.GetWif(network).ToString();

                privateKeys.Add(privKeyWif);
            }

            await BTCOperator.Instance.ImportPrivateKeyToNode(privateKeys, accountName);
        }


        public Dictionary<string, Money> BatchGetBTCBalanceViaNode(List<string> addressList)
        {
            var result = new Dictionary<string, Money>();

            try
            {
                var group = BTCOperator.Instance.ListAddressGroupings();
                foreach (var address in addressList)
                {
                    var g = group.SingleOrDefault(q => q.PublicAddress.ToString() == address);
                    Money balance = null != g ? g.Amount : Money.Parse("0");
                    result.Add(address, balance);
                }
            }
            catch(Exception)
            {

            }

            return result;
        }

        public async Task<Dictionary<string, Money>> BatchGetUSDTBalanceViaNode(List<string> addressList)
        {
            var result = new Dictionary<string, Money>();

            try
            {
                foreach (var address in addressList)
                {
                    Money balance = await USDTOperator.Instance.GetBalanceByAddress(address);
                    result.Add(address, balance);
                }
            }
            catch (Exception)
            {

            }

            return result;
        }
    }
}
