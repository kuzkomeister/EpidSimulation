using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EpidSimulation.Views.Primitive
{
    /// <summary>
    /// Логика взаимодействия для UC_Period.xaml
    /// </summary>
    public partial class UC_Period : UserControl
    {
        public UC_Period()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register(nameof(Text), typeof(string), typeof(UC_Period), new PropertyMetadata(""));

        public string StartPeriod
        {
            get => (string)GetValue(StartPeriodProperty);
            set => SetValue(StartPeriodProperty, value);
        }
        public static readonly DependencyProperty StartPeriodProperty =
          DependencyProperty.Register(nameof(StartPeriod), typeof(string), typeof(UC_Period), new PropertyMetadata(""));

        public string EndPeriod
        {
            get => (string)GetValue(EndPeriodProperty);
            set => SetValue(EndPeriodProperty, value);
        }
        public static readonly DependencyProperty EndPeriodProperty =
          DependencyProperty.Register(nameof(EndPeriod), typeof(string), typeof(UC_Period), new PropertyMetadata(""));

    }
}
