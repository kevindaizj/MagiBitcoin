using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.ValidationAttributes
{
    public class MoneyGTAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The value of {0} must greater than {1}.";

        private Money ComparedValue { get; }

        public MoneyGTAttribute(long comparedValue)
        {
            this.ComparedValue = comparedValue;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, ComparedValue);
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (null == value)
                return ValidationResult.Success;

            var num = (Money)value;
            if (num > ComparedValue)
                return ValidationResult.Success;
            else
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

    }
}
