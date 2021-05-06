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

namespace EpidSimulation
{
    public partial class ConfigDiseaseWindow : Window
    {
        
        public ConfigDiseaseWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = new ViewModel.ConfigDiseaseWindowViewModel(mainWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
