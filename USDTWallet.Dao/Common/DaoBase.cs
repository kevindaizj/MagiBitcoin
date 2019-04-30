using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.DataContext.Context;

namespace USDTWallet.Dao.Common
{
    public class DaoBase
    {
        protected WalletContext GetWalletContext()
        {
            var context = new WalletContext();
            context.Database.Log = o =>
            {
                System.Diagnostics.Debug.WriteLine(o);
                //if (ConfigurationManager.AppSettings["OutputSQL"] == "true")
                //{
                //    Logger.Instance.Debug(o);
                //}

            };
            return context;
        }
    }
}
