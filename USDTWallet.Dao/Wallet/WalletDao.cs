﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Dao.Common;
using USDTWallet.DataContext.Context;
using USDTWallet.Models.Models.Wallets;

namespace USDTWallet.Dao.Wallet
{
    public class WalletDao : DaoBase
    {
        public bool AnyWalletExisted()
        {
            using (var db = this.GetWalletContext())
            {
                var query = from w in db.BASE_WALLET
                            select w;

                return query.Any();
            }
        }

        public void Create(WalletInfo model)
        {
            using (var db = this.GetWalletContext())
            {
                var entity = new BASE_WALLET
                {
                    ID = model.Id,
                    WALLET_NAME = model.WalletName,
                    PASSWORD = model.Password,
                    MNEMONIC_WORDS = model.MnemonicWords,
                    DESCRIPTION = model.Description,
                    IS_ACTIVE = true,
                    CREATE_DATE = DateTime.Now
                };

                db.BASE_WALLET.Add(entity);
                db.SaveChanges();
            }
        }
    }
}
