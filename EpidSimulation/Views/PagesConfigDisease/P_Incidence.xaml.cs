using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace EpidSimulation.Views.PagesConfigDisease
{
    /// <summary>
    /// Логика взаимодействия для Incidence.xaml
    /// </summary>
    public partial class P_Incidence : Page
    {
        public P_Incidence()
        {
            InitializeComponent();
        }

        private void IntPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }
    }
}
