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
        MainWindow _main;

        public ConfigDiseaseWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.ConfigDiseaseWindowViewModel();
        }

        public ConfigDiseaseWindow(MainWindow main)
        {
            _main = main;
            InitializeComponent();
            DataContext = new ViewModel.ConfigDiseaseWindowViewModel();
        }

    }
}
