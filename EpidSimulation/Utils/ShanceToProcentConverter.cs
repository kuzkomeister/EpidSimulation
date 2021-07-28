using System;
using System.Globalization;
using System.Windows.Data;

namespace EpidSimulation.Utils
{
    class ShanceToProcentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 100.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double res;
            double.TryParse(value as string, out res);
            return res / 100.0;
        }
    }
}
