using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Transactions;
using USDTWallet.Common;
using USDTWallet.Common.Operators;

namespace USDTWallet.Views.Tools
{
    public class ToolPageController : ControllerBase
    {
        public string Title { get { return "工具"; } }
        public string SelectedIconScr { get { return "/Images/token_blue.png"; } }
        public string IconScr { get { return "/Images/token.png"; } }

        private string _txId;
        public string TransactionId
        {
            get { return _txId; }
            set { SetProperty(ref _txId, value); }
        }

        private string _txInfo;
        public string TxInfo
        {
            get { return _txInfo; }
            set { SetProperty(ref _txInfo, value); }
        }

        private string _omniTxInfo;
        public string OmniTxInfo
        {
            get { return _omniTxInfo; }
            set { SetProperty(ref _omniTxInfo, value); }
        }

        public DelegateCommand GetTxCommand { get; set; }
        public DelegateCommand GetOmniTxCommand { get; set; }

        private TransactionManager TxManager { get; set; }

        public ToolPageController(TransactionManager txManager)
        {
            this.TxManager = txManager;
            this.GetTxCommand = new DelegateCommand(GetTransaction);
            this.GetOmniTxCommand = new DelegateCommand(GetOmniTransaction);
        }


        private async void GetTransaction()
        {
            if (string.IsNullOrWhiteSpace(this.TransactionId))
                return;

            var tx = await BTCOperator.Instance.GetTransaction(this.TransactionId);
            this.TxInfo = tx.ToString();
        }

        private async void GetOmniTransaction()
        {
            if (string.IsNullOrWhiteSpace(this.TransactionId))
                return;

            this.OmniTxInfo = await USDTOperator.Instance.GetOmniTransaction(this.TransactionId);
        }

    }
}
