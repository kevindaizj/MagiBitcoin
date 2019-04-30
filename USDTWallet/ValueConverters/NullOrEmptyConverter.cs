using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace USDTWallet.ValueConverters
{
    public class NullOrEmptyConverter : IValueConverter
    {
        private static readonly string Default = "--";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var displayTxt = Default;

            var paramStr = parameter as string;
            if (!string.IsNullOrWhiteSpace(paramStr))
                displayTxt = paramStr;

            if (null == value)
                return displayTxt;

            var str = value.ToString();
            if (string.IsNullOrWhiteSpace(str))
                return displayTxt;
            else
                return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
