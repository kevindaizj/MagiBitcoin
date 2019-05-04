using NBitcoin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace USDTWallet.ValueConverters
{
    public class MoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
                return null;

            var number = (Money)value;
            string str = number.ToString();
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
            {
                var type = value.GetType();
                if (Nullable.GetUnderlyingType(type) != null)
                    return null;
            }

            string str = value.ToString();
            if (string.IsNullOrWhiteSpace(str))
                return new Money((long)0);

            return Money.Parse(str);
        }
    }
}
