using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using USDTWallet.Biz.Transactions;
using USDTWallet.Common;
using USDTWallet.Events;
using USDTWallet.Models.Models.Transactions;

namespace USDTWallet.Views.TransactionOverview
{
    class TransactionOverviewPageController : ControllerBase
    {
        public string Title { get { return "Transaction"; } }
        public string IconScr { get { return "/Images/transfer.png"; } }
        public string SelectedIconScr { get { return "/Images/transfer_blue.png"; } }

        public ObservableCollection<TransactionInfoVM> TransactionList { get; set; }

        public bool AnyTransactions
        {
            get { return TransactionList.Count > 0; }
        }

        private TransactionManager TransactionManager { get; set; }
        private IEventAggregator EventAggregator { get; set; }
        public DelegateCommand<string> TransactionDetailCommand { get; set; }
        public InteractionRequest<INotification> TranactionDetailPopupRequest { get; set; }

        private DispatcherTimer TransactionTimer { get; set; }

        public TransactionOverviewPageController(TransactionManager TxManager, IEventAggregator eventAggregator)
        {
            this.TransactionManager = TxManager;
            this.EventAggregator = eventAggregator;
            this.TransactionList = new ObservableCollection<TransactionInfoVM>();
            this.EventAggregator.GetEvent<LoginSuccessEvent>().Subscribe(Initialize);
            this.TransactionDetailCommand = new DelegateCommand<string>(OpenTransactionDetailPopup);
            this.TranactionDetailPopupRequest = new InteractionRequest<INotification>();
            this.EventAggregator.GetEvent<TransactionCreated>().Subscribe(() => RefreshTransactionList());
        }

        private void Initialize()
        {
            this.RefreshTransactionList();
        }

        private void RefreshTransactionList()
        {
            var transactions = TransactionManager.GetTransactions();
            this.TransactionList.Clear();
            this.TransactionList.AddRange(transactions);
            this.RaisePropertyChanged("AnyTransactions");

            ResetBalanceTimer();
        }

        private void ResetBalanceTimer()
        {
            TransactionTimer?.Stop();
            TransactionTimer = null;

            TransactionTimer = new DispatcherTimer();
            TransactionTimer.Interval = new TimeSpan(0, 0, 3);
            TransactionTimer.Tick += CheckTransactionStatus;
            TransactionTimer.Start();
        }
        

        private void CheckTransactionStatus(object sender, EventArgs e)
        {
            var uncompletedTxs = TransactionList.Where(q => q.IsConfirmed == false).ToList();

            foreach (var tx in uncompletedTxs)
            {
                DispatcherHelper.Invoke(async () =>
                {
                    try
                    {
                        var raw = await TransactionManager.CheckAndRecordTransactionStatus(tx.TransactionId);
                        if (null != raw)
                        {
                            tx.BlockHash = raw.BlockHash.ToString();
                            tx.Confirmations = raw.Confirmations;
                            tx.IsConfirmed = raw.Confirmations > 0;
                        }
                    }
                    catch(Exception)
                    {

                    }
                });
            }
        }

        private void OpenTransactionDetailPopup(string txId)
        {
            var tx = this.TransactionList.Single(q => q.TransactionId == txId);
            TranactionDetailPopupRequest.Raise(new Notification { Title = "Transaction详细", Content = tx });
        }

    }
}
