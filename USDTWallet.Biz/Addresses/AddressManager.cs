using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
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

        public AddressVM GetRootAddress(string walletId)
        {
            var addressInfo = AddressDao.GetRootAddress(walletId);
            return new AddressVM
            {
                Address = addressInfo.Address,
                KeyPath = addressInfo.KeyPath,
                PathIndex = addressInfo.PathIndex,
                AddressType = addressInfo.AddressType,
                AddressCategory = addressInfo.AddressCategory,
                Name = addressInfo.Name,
                Balance = addressInfo.Balance,
                Account = addressInfo.Account
            };
        }

        public List<AddressVM> GetRootAddressesByType(string walletId, CustomAddressType type)
        {
            var addresses = AddressDao.GetRootAddressesByType(walletId, (long)type);
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
                    Balance = a.Balance,
                    Account = a.Account
                });
            }

            return results;
        }
        
    }
}
