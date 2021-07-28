using System.Windows;
using EpidSimulation.ViewModels;

namespace EpidSimulation.Views
{
    public partial class F_ConfigDisease : Window
    {
        
        public F_ConfigDisease(VMF_Workplace mwvm)
        {
            InitializeComponent();
            DataContext = new VMF_ConfigDisease(mwvm);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
