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
using System.Windows.Shapes;
using EpidSimulation.Backend;
using EpidSimulation.ViewModel;

namespace EpidSimulation
{
    public partial class ConfigDiseaseWindow : Window
    {
        
        public ConfigDiseaseWindow(MainWindowViewModel mwvm)
        {
            InitializeComponent();
            DataContext = new ConfigDiseaseWindowViewModel(mwvm);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
