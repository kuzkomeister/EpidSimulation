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
using EpidSimulation.Backend;
using System.ComponentModel;

namespace EpidSimulation.ViewModel
{
    class ConfigDiseaseWindowViewModel : ViewModelBase
    {   
        private Page _diseaseParam;
        private Page _incidence;
        private Page _interactionLevel;
        private Page _prevalence;

        public ConfigDisease Config;
        MainWindow _mainWindow;

        //===== Вероятности
        private double _maskProtectionFor;
        public double MaskProtectionFor
        {
            set
            {
                _maskProtectionFor = 0 <= value && value <= 1 ? value : _maskProtectionFor;
                Config.MaskProtectionFor = MaskProtectionFor;

                ProbabilityInfMM = ProbabilityInfNN * (1 - MaskProtectionFor) * (1 - MaskProtectionFrom);
                ProbabilityInfMN = ProbabilityInfNN * (1 - MaskProtectionFrom);
                ProbabilityInfNM = ProbabilityInfNN * (1 - MaskProtectionFor);
            }
            get => _maskProtectionFor;
        }

        private double _maskProtectionFrom;
        public double MaskProtectionFrom
        {
            set
            {
                _maskProtectionFrom = 0 <= value && value <= 1 ? value : _maskProtectionFrom;
                Config.MaskProtectionFrom = MaskProtectionFrom;

                ProbabilityInfMM = ProbabilityInfNN * (1 - MaskProtectionFor) * (1 - MaskProtectionFrom);
                ProbabilityInfMN = ProbabilityInfNN * (1 - MaskProtectionFrom);
                ProbabilityInfNM = ProbabilityInfNN * (1 - MaskProtectionFor);
            }
            get => _maskProtectionFrom;
        }

        private double _probabilityInfMM;
        public double ProbabilityInfMM
        {
            set
            {
                _probabilityInfMM = 0 <= value && value <= 1 ? value : _probabilityInfMM;
                RaisePropertyChanged("ProbabilityInfMM");
            }
            get => _probabilityInfMM;
        }

        private double _probabilityInfNM;
        public double ProbabilityInfNM
        {
            set
            {
                _probabilityInfNM = 0 <= value && value <= 1 ? value : _probabilityInfNM;
                RaisePropertyChanged("ProbabilityInfNM");
            }
            get => _probabilityInfNM;
        }

        private double _probabilityInfMN;
        public double ProbabilityInfMN
        {
            set 
            { 
                _probabilityInfMN = 0 <= value && value <= 1 ? value : _probabilityInfMN;
                RaisePropertyChanged("ProbabilityInfMN");
            }
            get => _probabilityInfMN;
        }

        private double _probabilityInfNN;
        public double ProbabilityInfNN
        {
            set
            {
                _probabilityInfNN = 0 <= value && value <= 1 ? value : _probabilityInfNN;
                Config.ProbabilityInfAirborne = ProbabilityInfNN;

                ProbabilityInfMM = ProbabilityInfNN * (1 - MaskProtectionFor) * (1 - MaskProtectionFrom);
                ProbabilityInfMN = ProbabilityInfNN * (1 - MaskProtectionFrom);
                ProbabilityInfNM = ProbabilityInfNN * (1 - MaskProtectionFor);
                RaisePropertyChanged("ProbabilityInfNN");
            }
            get => _probabilityInfNN;
        }

        private double _probabilityInfContact;
        public double ProbabilityInfContact
        {
            set
            {
                _probabilityInfContact = 0 <= value && value <= 1 ? value : _probabilityInfContact;
                Config.ProbabilityInfContact = ProbabilityInfContact;
            }
            get => _probabilityInfContact;
        }
        //=======================================

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

        // Статусы для видимости кнопок
        private Visibility _finalPageVisible;
        public Visibility FinalPageVisible
        {
            set 
            { 
                _finalPageVisible = value;
                RaisePropertyChanged("FinalPageVisible");
                CompleteBtnVisible = value == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
            get => _finalPageVisible;
        }

        private Visibility _completeBtnVisible;
        public Visibility CompleteBtnVisible 
        {
            set
            {
                _completeBtnVisible = value;
                RaisePropertyChanged("CompleteBtnVisible");
            }

            get 
            { 
                return FinalPageVisible == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private Visibility _firstPageVisible;
        public Visibility FirstPageVisible
        {
            set
            {
                _firstPageVisible = value;
                RaisePropertyChanged("FirstPageVisible");
            }
            get => _firstPageVisible;
        }

        // Прозрачность страницы
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
                    result = _diseaseParam;
                    FirstPageVisible = Visibility.Hidden;
                    break;
                case 3:
                    result = _interactionLevel;
                    break;
                case 4:
                    result = _prevalence;
                    FinalPageVisible = Visibility.Visible;
                    break;
                default:
                    _numPage++;
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
                    result = _interactionLevel;
                    FirstPageVisible = Visibility.Visible;
                    break;
                case 2:
                    result = _prevalence;
                    break;
                case 3:
                    result = _incidence;
                    FinalPageVisible = Visibility.Hidden;
                    break;
                default:
                    _numPage--;
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

        public ICommand bComplete_Click
        {
            get
            {
                return new RelayCommand(() => { _mainWindow.Config = Config; _mainWindow.SetNewConfig(); });
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

        public ConfigDiseaseWindowViewModel(MainWindow mainWindow) 
        {
            Config = (ConfigDisease)mainWindow.Config.Clone();
            _mainWindow = mainWindow;
            ProbabilityInfContact = Config.ProbabilityInfContact;
            ProbabilityInfNN = Config.ProbabilityInfAirborne;
            MaskProtectionFor = Config.MaskProtectionFor;
            MaskProtectionFrom = Config.MaskProtectionFrom;

            _diseaseParam = new Pages.ConfigDisease.DiseaseParam();
            _diseaseParam.DataContext = Config;
            _incidence = new Pages.ConfigDisease.Incidence();
            _incidence.DataContext = Config;
            _interactionLevel = new Pages.ConfigDisease.InteractionLevel();
            _interactionLevel.DataContext = Config;
            _prevalence = new Pages.ConfigDisease.Prevalence();
            _prevalence.DataContext = this;

            FinalPageVisible = Visibility.Visible;
            FirstPageVisible = Visibility.Hidden;

            FrameOpacity = 1;
            CurrentPage = _diseaseParam;
            _numPage = 1;
        }

       
        
    }
}
