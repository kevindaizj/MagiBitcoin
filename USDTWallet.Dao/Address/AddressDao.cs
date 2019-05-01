using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Dao.Common;
using USDTWallet.DataContext.Context;
using USDTWallet.Models.Enums.Address;
using USDTWallet.Models.Models.Addresses;

namespace USDTWallet.Dao.Address
{
    public class AddressDao : DaoBase
    {
        public AddressInfo GetByAddress(string walletId, string address)
        {
            using (var db = this.GetWalletContext())
            {
                var query = from a in db.BASE_ADDRESS
                            where a.ADDRESS == address &&
                                  a.WALLET_ID == walletId
                            select new AddressInfo
                            {
                                Id = a.ID,
                                Address = a.ADDRESS,
                                WalletId = a.WALLET_ID,
                                Network = a.NETWORK,
                                KeyPath = a.KEY_PATH,
                                PathIndex = a.PATH_INDEX,
                                AddressType = a.ADDRESS_TYPE,
                                AddressCategory = a.ADDRESS_CATEGORY,
                                Name = a.NAME,
                                Balance = a.BALANCE,
                                Account = a.ACCOUNT,
                                Description = a.DESCRIPTION
                            };

                return query.SingleOrDefault();
            }
        }

        public List<AddressInfo> GetRootAddressesByType(string walletId, long type)
        {
            using (var db = this.GetWalletContext())
            {
                var query = from a in db.BASE_ADDRESS
                            where a.WALLET_ID == walletId &&
                                  a.ADDRESS_TYPE == type
                            select new AddressInfo
                            {
                                Id = a.ID,
                                Address = a.ADDRESS,
                                WalletId = a.WALLET_ID,
                                Network = a.NETWORK,
                                KeyPath = a.KEY_PATH,
                                PathIndex = a.PATH_INDEX,
                                AddressType = a.ADDRESS_TYPE,
                                AddressCategory = a.ADDRESS_CATEGORY,
                                Name = a.NAME,
                                Balance = a.BALANCE,
                                Account = a.ACCOUNT,
                                Description = a.DESCRIPTION
                            };

                return query.ToList();
            }
        }

        public AddressInfo GetRootAddress(string walletId)
        {
            using (var db = this.GetWalletContext())
            {
                var query = from a in db.BASE_ADDRESS
                            where a.WALLET_ID == walletId &&
                                  a.ADDRESS_TYPE == (long)CustomAddressType.Root
                            select new AddressInfo
                            {
                                Id = a.ID,
                                Address = a.ADDRESS,
                                WalletId = a.WALLET_ID,
                                Network = a.NETWORK,
                                KeyPath = a.KEY_PATH,
                                PathIndex = a.PATH_INDEX,
                                AddressType = a.ADDRESS_TYPE,
                                AddressCategory = a.ADDRESS_CATEGORY,
                                Name = a.NAME,
                                Balance = a.BALANCE,
                                Account = a.ACCOUNT,
                                Description = a.DESCRIPTION
                            };

                return query.SingleOrDefault();
            }
        }

        public void Create(AddressInfo model)
        {
            using (var db = this.GetWalletContext())
            {
                var entity = new BASE_ADDRESS
                {
                    ID = model.Id,
                    ADDRESS = model.Address,
                    WALLET_ID = model.WalletId,
                    NETWORK = model.Network,
                    KEY_PATH = model.KeyPath,
                    PATH_INDEX = model.PathIndex,
                    ADDRESS_TYPE = model.AddressType,
                    ADDRESS_CATEGORY = model.AddressCategory,
                    NAME = model.Name,
                    BALANCE = model.Balance,
                    ACCOUNT = model.Account,
                    DESCRIPTION = model.Description,
                    CREATE_DATE = DateTime.Now
                };

                db.BASE_ADDRESS.Add(entity);
                db.SaveChanges();
            }
        }
    }
}
