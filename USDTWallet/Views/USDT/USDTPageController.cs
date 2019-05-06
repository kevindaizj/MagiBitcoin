using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
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

namespace USDTWallet.Views.USDT
{
    public class USDTPageController : ControllerBase
    {
        public string Title { get { return "USDT"; } }
        public string SelectedIconScr { get { return "/Images/token_blue.png"; } }
        public string IconScr { get { return "/Images/token.png"; } }

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

        private string _signbtnContent = "签署";
        public string SignBtnContent
        {
            get { return _signbtnContent; }
            set { this.SetProperty(ref _signbtnContent, value); }
        }


        private USDTTransferVM _transferInfo;
        public USDTTransferVM TransferInfo
        {
            get { return _transferInfo; }
            set { this.SetProperty(ref _transferInfo, value); }
        }

        private string _unsignedTxInfo;
        public string UnsignedTxInfo
        {
            get { return _unsignedTxInfo; }
            set { SetProperty(ref _unsignedTxInfo, value); }
        }

        public DelegateCommand EstimateFeeRateCommand { get; set; }

        public DelegateCommand GenerateTransactionCommand { get; set; }
        public InteractionRequest<INotification> GenTransactionPopupRequest { get; set; }

        public DelegateCommand SignTransactionCommand { get; set; }
        public InteractionRequest<INotification> SignTransactionPopupRequest { get; set; }

        public DelegateCommand OnFromAddressChanged { get; set; }

        private IEventAggregator EventAggregator { get; set; }
        private MessageBoxService MessageBoxService { get; set; }

        private AddressManager AddressManager { get; set; }


        public USDTPageController(IEventAggregator eventAggregator, MessageBoxService msgBox, AddressManager addressManager)
        {
            MessageBoxService = msgBox;
            this.TransferInfo = new USDTTransferVM();

            this.AddressManager = addressManager;

            this.OnFromAddressChanged = new DelegateCommand(async () => await GetBalance());
            this.GenerateTransactionCommand = new DelegateCommand(GenerateUnsignedTransaction);
            this.GenTransactionPopupRequest = new InteractionRequest<INotification>();

            this.SignTransactionCommand = new DelegateCommand(SignTransaction);
            this.SignTransactionPopupRequest = new InteractionRequest<INotification>();

            this.EstimateFeeRateCommand = new DelegateCommand(EstimateFeeRate);
        }

        private void GenerateUnsignedTransaction()
        {
            this.TransferInfo.TriggerValidation();
            if (this.TransferInfo.HasErrors)
                return;

            this.BtnContent = "生成中...";
            GenTransactionPopupRequest.Raise(new Notification { Title = "BTC交易 (未签名)", Content = TransferInfo });
            this.BtnContent = "生成交易";
        }

        private void SignTransaction()
        {
            if (string.IsNullOrEmpty(this.UnsignedTxInfo))
                return;

            bool valid = BTCOperator.Instance.CheckUnsignedTxInfo(this.UnsignedTxInfo);
            if (!valid)
            {
                this.MessageBoxService.Show("交易字符串不符合规定，请确认");
                return;
            }

            var result = BTCOperator.Instance.DeserailizeUnsignedTxResult(this.UnsignedTxInfo);

            this.SignBtnContent = "签署中...";
            SignTransactionPopupRequest.Raise(new Notification { Title = "签署", Content = result });
            this.SignBtnContent = "签署";
        }

        private async Task GetBalance()
        {
            var fromAddress = this.TransferInfo.FromAddress;
            if (!AddressHelper.IsValidAccountAddress(fromAddress))
                return;
            TransferInfo.BalanceOf = await AddressManager.GetUSDTBalance(fromAddress);
            TransferInfo.BTCBalanceOf = await AddressManager.GetBTCBalance(fromAddress);
        }

        private async void EstimateFeeRate()
        {
            EstFeeBtnContent = "估算中...";
            TransferInfo.EstimateFeeRate = await BTCOperator.Instance.EstimateFeeRate();
            EstFeeBtnContent = "估算";
        }
    }
}
