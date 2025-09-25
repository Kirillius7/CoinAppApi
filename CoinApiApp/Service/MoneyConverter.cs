using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoinApiApp.Models
{
    public class MoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            if (decimal.TryParse(value.ToString(), out decimal number))
            {
                if (number >= 1_000_000_000)
                    return $"{number / 1_000_000_000:0.##}B"; // мільярди
                if (number >= 1_000_000)
                    return $"{number / 1_000_000:0.##}M"; // мільйони
                if (number >= 1_000)
                    return $"{number / 1_000:0.##}K"; // тисячі

                return number.ToString("0.##"); // менші числа без скорочення
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
