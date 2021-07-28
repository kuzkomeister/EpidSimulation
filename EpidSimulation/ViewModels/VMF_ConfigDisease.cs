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
using EpidSimulation.Models;
using System.ComponentModel;
using EpidSimulation.Views.PagesConfigDisease;

namespace EpidSimulation.ViewModels
{
    public class VMF_ConfigDisease : ViewModelBase
    {   
        private Page _diseaseParam;
        private Page _incidence;
        private Page _interactionLevel;
        private Page _prevalence;

        private Config _config;
        private VMF_Workplace _mwvm;

        #region Свойства для ввода
        //===== Вероятности
        private double _maskProtectionFor;
        public double MaskProtectionFor
        {
            set
            {
                _maskProtectionFor = 0 <= value && value <= 1 ? value : _maskProtectionFor;
                _config.MaskProtectionFor = MaskProtectionFor;

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
                _config.MaskProtectionFrom = MaskProtectionFrom;

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
                _config.ProbabilityInfAirborne = ProbabilityInfNN;

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
                _config.ProbabilityInfContact = ProbabilityInfContact;
            }
            get => _probabilityInfContact;
        }

        //===== Радиусы
        private double _rMeet;
        public double RMeet
        {
            set
            {
                RadiusMeet = value > 0 ? value : 0;
                _rMeet = RadiusMeet - RadiusHuman > 0 ? RadiusMeet - RadiusHuman : 0;
                RaisePropertyChanged("RMeet");
            }
            get => _rMeet;
        }

        private double _rHandshake;
        public double RHandshake
        {
            set
            {
                RadiusHandshake = value > 0 ? value : 0;
                _rHandshake = RadiusHandshake - RadiusHuman > 0 ? RadiusHandshake - RadiusHuman : 0;
                RaisePropertyChanged("RHandshake");
            }
            get => _rHandshake;
        }

        private double _rSocDist;
        public double RSocDist
        {
            set
            {
                RadiusSocDist = value > 0 ? value : 0;
                _rSocDist = RadiusSocDist - RadiusHuman > 0 ? RadiusSocDist - RadiusHuman : 0;
                RaisePropertyChanged("RSocDist");
            }
            get => _rSocDist;
        }

        // радиусы для отображения и ввода в настройки
        public double CenterEll { get => 100; }

        private double _radiusHuman;
        public double RadiusHuman
        {
            set
            {
                _config.RadiusHuman = value;
                _radiusHuman = _config.RadiusHuman;
                RMeet = _config.RadiusAirborne - RadiusHuman;
                RHandshake = _config.RadiusContact - RadiusHuman;
                RSocDist = _config.RadiusSocDist - RadiusHuman;
                LeftTopHuman = CenterEll - RadiusHuman * 50;
                RaisePropertyChanged("RadiusHuman");
            }
            get => _radiusHuman;
        }

        private double _radiusMeet;
        public double RadiusMeet
        {
            set
            {
                _config.RadiusAirborne = value;
                _radiusMeet = _config.RadiusAirborne;
                LeftTopMeet = CenterEll - RadiusMeet * 50;
                RaisePropertyChanged("RadiusMeet");
            }
            get => _radiusMeet;
        }

        private double _radiusHandshake;
        public double RadiusHandshake
        {
            set
            {
                _config.RadiusContact = value;
                _radiusHandshake = _config.RadiusContact;
                LeftTopHandshake = CenterEll - RadiusHandshake * 50;
                RaisePropertyChanged("RadiusHandshake");
            }
            get => _radiusHandshake;
        }

        private double _radiusSocDist;
        public double RadiusSocDist
        {
            set
            {
                _config.RadiusSocDist = value;
                _radiusSocDist = _config.RadiusSocDist;
                LeftTopSocDist = CenterEll - RadiusSocDist * 50;
                RaisePropertyChanged("RadiusSocDist");
            }
            get => _radiusSocDist;
        }

        // координаты для отображения
        private double _leftTopHuman;
        public double LeftTopHuman
        {
            set
            {
                _leftTopHuman = value;
                RaisePropertyChanged("LeftTopHuman");
            }
            get => _leftTopHuman;
        }

        private double _leftTopMeet;
        public double LeftTopMeet
        {
            set
            {
                _leftTopMeet = value;
                RaisePropertyChanged("LeftTopMeet");
            }
            get => _leftTopMeet;
        }

        private double _leftTopHandshake;
        public double LeftTopHandshake
        {
            set
            {
                _leftTopHandshake = value;
                RaisePropertyChanged("LeftTopHandshake");
            }
            get => _leftTopHandshake;
        }

        private double _leftTopSocDist;
        public double LeftTopSocDist
        {
            set
            {
                _leftTopSocDist = value;
                RaisePropertyChanged("LeftTopSocDist");
            }
            get => _leftTopSocDist;
        }
        //=======================================
        #endregion

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
                return new RelayCommand(() => _mwvm.Config = _config);
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

        public VMF_ConfigDisease(VMF_Workplace mwvm)
        {
            _config = (Config)mwvm.Config.Clone();
            _mwvm = mwvm;
            ProbabilityInfContact = _config.ProbabilityInfContact;
            ProbabilityInfNN = _config.ProbabilityInfAirborne;
            MaskProtectionFor = _config.MaskProtectionFor;
            MaskProtectionFrom = _config.MaskProtectionFrom;

            RadiusHuman = _config.RadiusHuman;
            RMeet = _config.RadiusAirborne - RadiusHuman;
            RHandshake = _config.RadiusContact - RadiusHuman;
            RSocDist = _config.RadiusSocDist - RadiusHuman;

            _diseaseParam = new P_DiseaseParam();
            _diseaseParam.DataContext = _config;
            _incidence = new P_Incidence();
            _incidence.DataContext = _config;
            _interactionLevel = new P_InteractionLevel();
            _interactionLevel.DataContext = this;
            _prevalence = new P_Prevalence();
            _prevalence.DataContext = this;

            FinalPageVisible = Visibility.Visible;
            FirstPageVisible = Visibility.Hidden;

            FrameOpacity = 1;
            CurrentPage = _diseaseParam;
            _numPage = 1;
        }

        

    }
}
