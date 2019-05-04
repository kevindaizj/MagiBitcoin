using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.ValidationAttributes
{
    public class MoneyCompareAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The value of {0} cannot meet requirement of comparison with the value of the {1}.";

        public enum CompareOperator
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual
        }

        public string OtherProperty { get; private set; }
        public CompareOperator Operator { get; set; }

        public MoneyCompareAttribute(string otherProperty) : base(DefaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty))
                throw new ArgumentNullException("otherProperty");

            OtherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (null == value)
                return ValidationResult.Success;

            var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);
            var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

            var num = (Money)value;
            var compared = new Money((long)0);
            if (otherPropertyValue != null)
                compared = (Money)otherPropertyValue;

            if (Compare(num, compared))
                return ValidationResult.Success;
            else
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }


        private bool Compare(Money value, Money compared)
        {
            if (Operator == CompareOperator.Equal)
                return value.Equals(compared);
            if (Operator == CompareOperator.NotEqual)
                return !value.Equals(compared);
            if (Operator == CompareOperator.GreaterThan)
                return value > compared;
            if (Operator == CompareOperator.LessThan)
                return value < compared;
            if (Operator == CompareOperator.GreaterThanOrEqual)
                return value >= compared;
            if (Operator == CompareOperator.LessThanOrEqual)
                return value <= compared;

            return true;
        }
    }
}
