using System;
using System.Windows;
using System.Windows.Input;


namespace EpidSimulation.Views
{
    public partial class F_Workplace : Window
    {

        public F_Workplace()
        {
            InitializeComponent();
        }

        private void IntPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            F_Welcome wHelp = new F_Welcome();
            wHelp.ShowDialog();
        }
    }

}
