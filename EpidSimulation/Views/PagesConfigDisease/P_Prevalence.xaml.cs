using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace EpidSimulation.Views.PagesConfigDisease
{
    /// <summary>
    /// Логика взаимодействия для Prevalence.xaml
    /// </summary>
    

    public partial class P_Prevalence : Page
    {
        

        public P_Prevalence()
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
