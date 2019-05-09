using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Common;
using USDTWallet.Models.ValidationAttributes;

namespace USDTWallet.Models.Models.Transfer
{
    public class USDTTransferVM : ViewModelBase
    {
        private string _from = "mtYpuP45hjDBgFuYfRmeG7i63QYQCwSuPN";

        [Required(ErrorMessage = "发送地址不能为空")]
        [RegularExpression("[^OIl0]{25,34}$", ErrorMessage = "地址格式不正确")]
        public string FromAddress
        {
            get { return _from; }
            set { this.SetProperty(ref _from, value); }
        }

        private string _to = "n3uXG3c3ouZUtVZUbzCZCQomAguGxdwQcQ";

        [Required(ErrorMessage = "接收地址不能为空")]
        [RegularExpression("[^OIl0]{25,34}$", ErrorMessage = "地址格式不正确")]
        [NotEqualTo("FromAddress", ErrorMessage = "发送地址与接收地址不允许一样")]
        public string ToAddress
        {
            get { return _to; }
            set { this.SetProperty(ref _to, value); }
        }


        private string _feeAddr = "mm2PNN7Ybj2MajV9oai8eWBaD2KhAjMEBS";

        [Required(ErrorMessage = "付手续费地址不能为空")]
        [RegularExpression("[^OIl0]{25,34}$", ErrorMessage = "地址格式不正确")]
        public string FeeAddress
        {
            get { return _feeAddr; }
            set { this.SetProperty(ref _feeAddr, value); }
        }


        private Money _amount = Money.Parse("1000");

        [Required(ErrorMessage = "金额不能为空")]
        [MoneyCompare("BalanceOf", Operator = MoneyCompareAttribute.CompareOperator.LessThanOrEqual, ErrorMessage = "转账金额不允许超过账户余额")]
        [MoneyGT(0, ErrorMessage = "金额不能小于0")]
        public Money Amount
        {
            get { return _amount; }
            set
            {
                this.SetProperty(ref _amount, value);
            }
        }

        private Money _balanceOf;
        public Money BalanceOf
        {
            get { return _balanceOf; }
            set
            {
                this.SetProperty(ref _balanceOf, value);
            }
        }

        private Money _btcBalanceOf;
        public Money BTCBalanceOf
        {
            get { return _btcBalanceOf; }
            set
            {
                this.SetProperty(ref _btcBalanceOf, value);
            }
        }


        private FeeRate _feeRate = new FeeRate(Money.Parse("0.1"));
        public FeeRate EstimateFeeRate
        {
            get { return _feeRate; }
            set
            {
                this.SetProperty(ref _feeRate, value);
            }
        }
    }
}
