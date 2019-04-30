using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace USDTWallet.ValueConverters
{
    public class PipedValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Aggregate(value, (current, converter) =>
            {
                return converter.Convert(current, targetType, parameter, culture);
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var revertGroup = this.ToList();
            revertGroup.Reverse();

            return revertGroup.Aggregate(value, (current, converter) =>
            {
                return converter.ConvertBack(current, targetType, parameter, culture);
            });
        }
    }
}
