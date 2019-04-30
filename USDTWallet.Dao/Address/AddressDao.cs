using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Dao.Common;
using USDTWallet.DataContext.Context;
using USDTWallet.Models.Models.Addresses;

namespace USDTWallet.Dao.Address
{
    public class AddressDao : DaoBase
    {
        public AddressInfo GetByAddress(string address)
        {
            using (var db = this.GetWalletContext())
            {
                var query = from a in db.BASE_ADDRESS
                            where a.ADDRESS == address
                            select new AddressInfo
                            {
                                Id = a.ID,
                                Address = a.ADDRESS,
                                KeyPath = a.KEY_PATH,
                                Network = a.NETWORK,
                                Name = a.NAME,
                                Balance = a.BALANCE,
                                Account = a.ACCOUNT
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
                    KEY_PATH = model.KeyPath,
                    NETWORK = model.Network,
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
