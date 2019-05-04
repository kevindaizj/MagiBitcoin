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
    public class FeeRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
                return null;

            var number = (FeeRate)value;
            string str = number.FeePerK.ToString();
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
                return new FeeRate((decimal)0);

            var money = Money.Parse(str);
            return new FeeRate(money);
        }
    }
}
