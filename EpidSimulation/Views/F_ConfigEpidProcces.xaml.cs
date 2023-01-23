﻿using System;
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

using EpidSimulation.ViewModels;

namespace EpidSimulation.Views
{
    /// <summary>
    /// Логика взаимодействия для F_Config.xaml
    /// </summary>
    public partial class F_ConfigEpidProcces : Window
    {
        public F_ConfigEpidProcces()
        {
            InitializeComponent();
            DataContext = new VMF_ConfigEpidProcces();
        }
    }
}
