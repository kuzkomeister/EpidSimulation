using System;
using System.Windows;
using System.Windows.Input;

using EpidSimulation.ViewModels;

namespace EpidSimulation.Views
{
    public partial class F_Workplace : Window
    {

        public F_Workplace()
        {
            InitializeComponent();
            DataContext = new VMF_Workplace();
        }

        private void IntPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

    }

}
