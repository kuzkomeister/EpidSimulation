using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EpidSimulation.ViewModel
{
    class ConfigDiseaseWindowViewModel : ViewModelBase
    {
        private Page _stages;
        private Page _probabilities;
        private Page _radiuses;
        private Page _timers;
        private Page _maskProtection;
        private Page _result;

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set 
            { 
                _currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }
        }
        private int _numPage;

        private double _frameOpacity;
        public double FrameOpacity
        {
            get => _frameOpacity;
            set
            {
                _frameOpacity = value;
                RaisePropertyChanged("FrameOpacity");
            }
        }

        private Page PreviousPage()
        {
            Page result = CurrentPage;
            switch (_numPage--)
            {
                case 2:
                    result = _stages;
                    break;
                case 3:
                    result = _probabilities;
                    break;
                case 4:
                    result = _radiuses;
                    break;
                case 5:
                    result = _timers;
                    break;
                case 6:
                    result = _maskProtection;
                    break;
            }
            return result;
        }

        public ICommand bPreviousPage_Click
        {
            get
            {
                return new RelayCommand(() => SlowOpacity(PreviousPage()));
            }
        }

        private Page NextPage()
        {
            Page result = CurrentPage;
            switch (_numPage++)
            {
                case 1:
                    result = _probabilities;
                    break;
                case 2:
                    result = _radiuses;
                    break;
                case 3:
                    result = _timers;
                    break;
                case 4:
                    result = _maskProtection;
                    break;
                case 5:
                    result = _result;
                    break;
            }
            return result;
        }

        public ICommand bNextPage_Click
        {
            get
            {
                return new RelayCommand(() => SlowOpacity(NextPage())); 
            }
        } 

        private async void SlowOpacity(Page page)
        {
            await Task.Factory.StartNew(() =>
            {
                for (double i = 1.0; i > 0.0; i -= 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(50);
                }
                CurrentPage = page;
                for (double i = 0.0; i < 1.1; i += 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(50);
                }
            }
            );
        }


        private void IntPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
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

        public ConfigDiseaseWindowViewModel()
        {
            _stages = new Pages.ConfigDisease.Stages();
            _probabilities = new Pages.ConfigDisease.Probabilities();
            _radiuses = new Pages.ConfigDisease.Radiuses();
            _timers = new Pages.ConfigDisease.Timers();
            _maskProtection = new Pages.ConfigDisease.MaskProtection();
            _result = new Pages.ConfigDisease.Result();

            FrameOpacity = 1;
            CurrentPage = _stages;
            _numPage = 1;
        }
    }
}
