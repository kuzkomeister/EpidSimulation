﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EpidSimulation.Utils
{
    class IntToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush resColor = Brushes.White;
            
            
            switch (parameter.ToString()) {
                case "Cond":
                    switch ((int)value)
                    {
                        case 0:
                            resColor = Brushes.Green;
                            break;
                        case 1:
                            resColor = Brushes.LightPink;
                            break;
                        case 2:
                            resColor = Brushes.HotPink;
                            break;
                        case 3:
                            resColor = Brushes.Red;
                            break;
                        case 4:
                            resColor = Brushes.Blue;
                            break;
                        case 5:
                            resColor = Brushes.Yellow;
                            break;
                        case 6:
                            resColor = Brushes.DarkGray;
                            break;
                    }
                    break;
                    
                case "Hand":
                    if ((bool)value)
                        resColor = Brushes.Red;
                    else
                        resColor = Brushes.Black;
                    break;
            }
            
            return resColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
