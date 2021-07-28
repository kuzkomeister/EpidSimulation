using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EpidSimulation.Models;
using EpidSimulation.Views.PagesConfigDisease;
using EpidSimulation.Utils;

namespace EpidSimulation.ViewModels
{
    public class VMF_ConfigDisease : VM_BASIC
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
                OnPropertyChanged();
            }
            get => _probabilityInfMM;
        }

        private double _probabilityInfNM;
        public double ProbabilityInfNM
        {
            set
            {
                _probabilityInfNM = 0 <= value && value <= 1 ? value : _probabilityInfNM;
                OnPropertyChanged();
            }
            get => _probabilityInfNM;
        }

        private double _probabilityInfMN;
        public double ProbabilityInfMN
        {
            set 
            { 
                _probabilityInfMN = 0 <= value && value <= 1 ? value : _probabilityInfMN;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
            get => _leftTopHuman;
        }

        private double _leftTopMeet;
        public double LeftTopMeet
        {
            set
            {
                _leftTopMeet = value;
                OnPropertyChanged();
            }
            get => _leftTopMeet;
        }

        private double _leftTopHandshake;
        public double LeftTopHandshake
        {
            set
            {
                _leftTopHandshake = value;
                OnPropertyChanged();
            }
            get => _leftTopHandshake;
        }

        private double _leftTopSocDist;
        public double LeftTopSocDist
        {
            set
            {
                _leftTopSocDist = value;
                OnPropertyChanged();
            }
            get => _leftTopSocDist;
        }
        //=======================================
        #endregion

        // Текущая страница
        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set 
            { 
                _currentPage = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }


        #region [ Команды для кнопок переключения и сохранения]
        // Нажатие на кнопку переключения на предыдущую страницу
        public ICommand bPreviousPage_Click => new RelayCommand(_DoPreviousPage_Click, _AlwaysTrue);
        private void _DoPreviousPage_Click()
        {
            SlowOpacity(PreviousPage());
        }

        // Нажатие на кнопку переключения на следующую страницу
        public ICommand bNextPage_Click => new RelayCommand(_DoNextPage_Click, _AlwaysTrue);
        private void _DoNextPage_Click()
        {
            SlowOpacity(PreviousPage());
        }

        // Нажатие на кнопку сохранения изменений параметров
        public ICommand bComplete_Click => new RelayCommand(_DoNextPage_Click, _AlwaysTrue);
        private void _DoComplete_Click()
        {
            _mwvm.Config = _config;
        }


        /// <summary>
        /// Получить предыдущую страницу
        /// </summary>
        /// <returns>Предыдущая страница</returns>
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

        /// <summary>
        /// Получить следующую страницу
        /// </summary>
        /// <returns>Следующая страница</returns>
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

        /// <summary>
        /// Анимация переключения между страницами
        /// </summary>
        /// <param name="page">переключаемая страница</param>
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
        #endregion

        /// <summary>
        /// Конструктор ViewModel формы
        /// </summary>
        /// <param name="mwvm">ViewModel формы основного окна</param>
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
