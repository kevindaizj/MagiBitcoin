using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Mempools;
using USDTWallet.Common;
using USDTWallet.Models.Models.Transactions;

namespace USDTWallet.Views.Mempool
{
    public class MempoolController : ControllerBase
    {
        public string Title { get { return "Mempool"; } }
        public string IconScr { get { return "/Images/home.png"; } }
        public string SelectedIconScr { get { return "/Images/home_blue.png"; } }



        public DelegateCommand RefreshCommand { get; set; }

        private MempoolManager MempoolMgr { get; set; }

        public ObservableCollection<MempoolTxItem> Transactions { get; set; }

        private int _TxCount;
        public int TxCount
        {
            get { return _TxCount; }
            set { SetProperty(ref _TxCount, value); }
        }

        public MempoolController(MempoolManager mempoolMgr)
        {
            this.MempoolMgr = mempoolMgr;
            this.Transactions = new ObservableCollection<MempoolTxItem>();
            this.RefreshCommand = new DelegateCommand(RefreshTxs);
        }

        private async void RefreshTxs()
        {
            var txs = await MempoolMgr.GetTxsJsonFromMempool();
            this.TxCount = txs.Count;
            Transactions.Clear();
            Transactions.AddRange(txs);
        }
    }

    
}
