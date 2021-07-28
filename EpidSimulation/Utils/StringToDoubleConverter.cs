using System;
using System.Globalization;
using System.Windows.Data;

namespace EpidSimulation.Utils
{
    class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double res;
            double.TryParse(value as string, out res);
            return res;
        }
    }
}
