﻿using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
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
                Balance = addressInfo.Balance,
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
                    Balance = a.Balance,
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
    }
}