using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Addresses;
using USDTWallet.Common;
using USDTWallet.Common.Helpers;
using USDTWallet.Common.Operators;
using USDTWallet.ControlService.MsgBox;
using USDTWallet.Models.Models.Transfer;

namespace USDTWallet.Views.BTC
{
    public class BTCPageController : ControllerBase
    {
        public string Title { get { return "比特币"; } }
        public string IconScr { get { return "/Images/transfer.png"; } }
        public string SelectedIconScr { get { return "/Images/transfer_blue.png"; } }

        private string _estFeebtnContent = "估算";
        public string EstFeeBtnContent
        {
            get { return _estFeebtnContent; }
            set { this.SetProperty(ref _estFeebtnContent, value); }
        }

        private string _btnContent = "生成交易";
        public string BtnContent
        {
            get { return _btnContent; }
            set { this.SetProperty(ref _btnContent, value); }
        }


        private BTCTransferVM _transferInfo;
        public BTCTransferVM TransferInfo
        {
            get { return _transferInfo; }
            set { this.SetProperty(ref _transferInfo, value); }
        }

        public DelegateCommand EstimateFeeRateCommand { get; set; }
        
        public DelegateCommand GenerateTransactionCommand { get; set; }

        public DelegateCommand OnFromAddressChanged { get; set; }

        private IEventAggregator EventAggregator { get; set; }
        private MessageBoxService MessageBoxService { get; set; }

        private AddressManager AddressManager { get; set; }


        public BTCPageController(IEventAggregator eventAggregator, MessageBoxService msgBox, AddressManager addressManager)
        {
            MessageBoxService = msgBox;
            this.TransferInfo = new BTCTransferVM();

            this.AddressManager = addressManager;

            this.OnFromAddressChanged = new DelegateCommand(async () => await GetBalance());
            this.GenerateTransactionCommand = new DelegateCommand(GenerateUnsignedTransaction);
            this.EstimateFeeRateCommand = new DelegateCommand(EstimateFeeRate);
        }
        

        private void GenerateUnsignedTransaction()
        {
            this.TransferInfo.TriggerValidation();
            if (this.TransferInfo.HasErrors)
                return;

            //this.BtnContent = "Sending...";
            //ConfirmTransactionPopupRequest.Raise(new ConfirmTxNotification { Title = "确认", Transaction = TransferInfo });
            //this.BtnContent = "Send";
        }

        private async Task GetBalance()
        {
            var fromAddress = this.TransferInfo.FromAddress;
            if (!AddressHelper.IsValidAccountAddress(fromAddress))
                return;
            TransferInfo.BalanceOf = await AddressManager.GetBalance(fromAddress);
        }

        private async void EstimateFeeRate()
        {
            EstFeeBtnContent = "估算中...";
            TransferInfo.EstimateFeeRate = await BTCOperator.Instance.EstimateFeeRate();
            EstFeeBtnContent = "估算";
        }

    }
}
