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
        private Page _diseaseParam;
        private Page _incidence;
        private Page _interactionLevel;
        private Page _prevalence;

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
                    result = _diseaseParam;
                    break;
                case 3:
                    result = _interactionLevel;
                    break;
                case 4:
                    result = _prevalence;
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
                    break;
                case 2:
                    result = _prevalence;
                    break;
                case 3:
                    result = _incidence;
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




        public ConfigDiseaseWindowViewModel()
        {
            _diseaseParam = new Pages.ConfigDisease.DiseaseParam();
            _incidence = new Pages.ConfigDisease.Incidence();
            _interactionLevel = new Pages.ConfigDisease.InteractionLevel();
            _prevalence = new Pages.ConfigDisease.Prevalence();

            FrameOpacity = 1;
            CurrentPage = _diseaseParam;
            _numPage = 1;
        }

        // STAGES
        private int _timeInfInc_C = 0;
        public int TimeInfInc_C
        {
            get => _timeInfInc_C;
            set 
            {
                _timeInfInc_C = value;
                RaisePropertyChanged("TimeInfInc_C");
            }
        }

        private int _timeInfInc_AB = 0;
        public int TimeInfInc_AB
        {
            get => _timeInfInc_AB;
            set
            {
                _timeInfInc_AB = value;
                RaisePropertyChanged("TimeInfInc_AB");
            }
        }

        private int _timeRecovery_C = 0;
        public int TimeRecovery_C
        {
            get => _timeRecovery_C;
            set
            {
                _timeRecovery_C = value;
                RaisePropertyChanged("TimeRecovery_C");
            }
        }

        private int _timeRecovery_AB = 0;
        public int TimeRecovery_AB
        {
            get => _timeRecovery_AB;
            set
            {
                _timeRecovery_AB = value;
                RaisePropertyChanged("TimeRecovery_AB");
            }
        }

        private int _timeLatent_C = 0;
        public int TimeLatent_C
        {
            get => _timeLatent_C;
            set
            {
                _timeLatent_C = value;
                RaisePropertyChanged("TimeLatent_C");
            }
        }

        private int _timeLatent_AB = 0;
        public int TimeLatent_AB
        {
            get => _timeLatent_AB;
            set
            {
                _timeLatent_AB = value;
                RaisePropertyChanged("TimeLatent_AB");
            }
        }

        // PROBABILITIES
        private double _probabilityDied = 0;
        public double ProbabilityDied
        {
            get => _probabilityDied;
            set
            {
                _probabilityDied = value;
                RaisePropertyChanged("ProbabilityDied");
            }
        }

        private double _probabilityInfHandContact = 0;
        public double ProbabilityInfHandContact
        {
            get => _probabilityInfHandContact;
            set
            {
                _probabilityInfHandContact = value;
                RaisePropertyChanged("ProbabilityInfHandContact");
            }
        }

        private double _probabilityInfMeetContact = 0;
        public double ProbabilityInfMeetContact
        {
            get => _probabilityInfMeetContact;
            set
            {
                _probabilityInfMeetContact = value;
                RaisePropertyChanged("ProbabilityInfMeetContact");
            }
        }

        // TIMERS
        
    }
}
