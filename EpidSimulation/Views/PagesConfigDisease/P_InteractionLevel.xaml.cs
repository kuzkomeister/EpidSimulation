using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace EpidSimulation.Views.PagesConfigDisease
{
    /// <summary>
    /// Логика взаимодействия для InteractionLevel.xaml
    /// </summary>
    public partial class P_InteractionLevel : Page
    {
        public P_InteractionLevel()
        {
            InitializeComponent();
        }

        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }

        private void DoublePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (DS_Count(((TextBox)sender).Text) < 1))));
        }
    }
}
