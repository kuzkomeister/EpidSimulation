using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using EpidSimulation.Backend;
using EpidSimulation.Frontend;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EpidSimulation.ViewModel
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private Simulation _simulation;
        public Simulation CurSimulation
        {
            set => _simulation = value;
            get => _simulation;
        }

        private ConfigDisease _config;
        public ConfigDisease Config
        {
            set
            {
                _config = value;
                Human.Config = value;
            }
            get => _config;
        }

        private Canvas _canvasMap = new Canvas();
        public Canvas CanvasMap
        {
            get => _canvasMap;
        }

        private LinkedList<FigureHuman> _figures;
        private QuadTree _curNode;
        
        private DispatcherTimer _timer;
        private bool _statusTimer;

        private bool _statusDebug;

        
        private Rectangle _regionBackground;
        private Line _lineVMP;
        private Line _lineHMP;

        private ScaleTransform _scaleNode;
        
        public double WindowWidth { set; get; }
        public double WindowHeight { set; get; }


        public MainWindowViewModel()
        {
            Config = new ConfigDisease();
            _simulation = new Simulation();                
            _curNode = _simulation.Root;
            
            _regionBackground = new Rectangle
            {
                Width = _simulation.SizeWidth,
                Height = _simulation.SizeHeight,
                Fill = Brushes.LightGray,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            Canvas.SetLeft(_regionBackground, 0);
            Canvas.SetTop(_regionBackground, 0);
            CanvasMap.Children.Add(_regionBackground);

            _lineVMP = new Line
            {
                X1 = _simulation.SizeWidth / 2,
                Y1 = 0,
                X2 = _simulation.SizeWidth / 2,
                Y2 = _simulation.SizeHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(_lineVMP);

            _lineHMP = new Line
            {
                X1 = 0,
                Y1 = _simulation.SizeHeight / 2,
                X2 = _simulation.SizeWidth,
                Y2 = _simulation.SizeHeight / 2,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(_lineHMP);

            _scaleNode = new ScaleTransform();
            CanvasMap.RenderTransform = _scaleNode;
            _scaleNode.ScaleX = 4;
            _scaleNode.ScaleY = 4;            
        }

        private void CreateFigures(LinkedList<Human> people)
        {
            _figures = new LinkedList<FigureHuman>();
            foreach (Human human in people)
            {
                FigureHuman figure = new FigureHuman(human);
                _figures.AddFirst(figure);
            }
        }

        private void AddFigures()
        {
            foreach (FigureHuman figure in _figures)
            {
                if (figure.SocDistCircle != null)
                    CanvasMap.Children.Add(figure.SocDistCircle);
            }

            foreach (FigureHuman figure in _figures)
            {
                CanvasMap.Children.Add(figure.CondCircle);
                if (figure.MaskCircle != null)
                    CanvasMap.Children.Add(figure.MaskCircle);

            }
        }

        private void ClearDeads()
        {
            for (LinkedListNode<FigureHuman> figure = _figures.First; figure != null;)
            {
                if (figure.Value.CondCircle.Fill == Brushes.DarkGray)
                {
                    _canvasMap.Children.Remove(figure.Value.CondCircle);
                    if (figure.Value.MaskCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.Value.MaskCircle);
                    }
                    if (figure.Value.SocDistCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.Value.SocDistCircle);
                    }
                    LinkedListNode<FigureHuman> temp = figure;
                    figure = figure.Next;
                    _figures.Remove(temp);
                }
                else
                {
                    figure = figure.Next;
                }
            }
        }

        private void ClearMap()
        {
            if (_figures != null)
            {
                foreach (FigureHuman figure in _figures)
                {
                    _canvasMap.Children.Remove(figure.CondCircle);
                    if (figure.MaskCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.MaskCircle);
                    }
                    if (figure.SocDistCircle != null)
                    {
                        _canvasMap.Children.Remove(figure.SocDistCircle);
                    }
                }

                _figures = null;
            }
        }

        private void MakeVisibleNodeHumans()
        {
            LinkedList<LinkedList<Human>> tempList = new LinkedList<LinkedList<Human>>();
            _curNode.GetListObjects(tempList);

            foreach (FigureHuman figure in _figures)
            {
                bool vis = false;
                foreach (LinkedList<Human> l in tempList)
                {
                    if (l.Find(figure.human) != null)
                    {
                        vis = true;
                    }
                    if (vis)
                        break;
                }

                if (vis)
                {
                    figure.SetVisible(0, _statusDebug);
                }
                else
                {
                    figure.SetVisible(1, _statusDebug);
                }
            }
        }

        //==========
        private int _sizeWidth = 10;
        public int SizeWidth 
        { 
            set => _sizeWidth = value > 10 ? value : 10; 
            get => _sizeWidth; 
        }
        private int _sizeHeight = 10;
        public int SizeHeight 
        { 
            set => _sizeHeight = value > 10 ? value : 10; 
            get => _sizeHeight; 
        }

        public int AmountZdNothing { set; get; }
        public int AmountZdMask { set; get; }
        public int AmountZdSocDist { set; get; }
        public int AmountZdAll { set; get; }


        public int AmountIncNothing { set; get; }
        public int AmountIncMask { set; get; }
        public int AmountIncSocDist { set; get; }
        public int AmountIncAll { set; get; }


        public int AmountProdNothing { set; get; }
        public int AmountProdMask { set; get; }
        public int AmountProdSocDist { set; get; }
        public int AmountProdAll { set; get; }


        public int AmountClinNothing { set; get; }
        public int AmountClinMask { set; get; }
        public int AmountClinSocDist { set; get; }
        public int AmountClinAll { set; get; }


        public int AmountVzdNothing { set; get; }
        public int AmountVzdMask { set; get; }
        public int AmountVzdSocDist { set; get; }
        public int AmountVzdAll { set; get; }


        public int AmountAsymptNothing { set; get; }
        public int AmountAsymptMask { set; get; }
        public int AmountAsymptSocDist { set; get; }
        public int AmountAsymptAll { set; get; }

        // Итерация основного таймера 
        private void TimerTick(object sender, EventArgs e)
        {
            _simulation.Iterate();

            if (_curNode != _simulation.Root)
                MakeVisibleNodeHumans();

            ClearDeads();
            /*
            curIter.Content = "Текущая итерация: " + Sim.Iter;
            stat.Content = "Здоровых: " + Sim.AmountZd + "\n" +
                           "Инфицированных в латентном периоде: " + Sim.AmountLat + "\n" +
                           "Инфицированных в инкубационном периоде: " + Sim.AmountInc + "\n" +
                           "Инфицированных в клиническом периоде: " + Sim.AmountClin + "\n" +
                           "С иммунитетом: " + Sim.AmountVzd + "\n" +
                           "Умерших: " + Sim.AmountDied;

            debug.Content = "Статистика:\n" +
                           "Всего контактов: " + Sim.StContacts + "\n" +
                           "Инфицированных контактов: " + Sim.StContactsInf + "\n" +
                           "Всего рукопожатий: " + Sim.StHandShakes + "\n" +
                           "Заражений с рук: " + Sim.StHandshakesInf + "\n";
            */
        }

        // Включить/Выключить симуляцию
        public ICommand bStartStopSimulation_Click
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_statusTimer)
                    {
                        _timer.Stop();
                        _statusTimer = false;
                        // Header = "включить"
                    }
                    else
                    {
                        _timer.Start();
                        _statusTimer = true;
                        // Header = "выключить"
                    }
                });
                
            }
        }

        // Включить/Выключить отображение соц дистанции
        public ICommand bOnOffDebug_Click
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!_statusDebug)
                    {
                        foreach (FigureHuman figure in _figures)
                        {
                            if (figure.human.SocDist && figure.CondCircle.Visibility == 0)
                                figure.SocDistCircle.Visibility = Visibility.Visible;
                        }
                        //miDebugOnOff.Header = "Выключить дебаг";
                        _statusDebug = true;
                    }
                    else
                    {
                        foreach (FigureHuman figure in _figures)
                        {
                            if (figure.SocDistCircle != null)
                                figure.SocDistCircle.Visibility = Visibility.Hidden;
                        }
                        //miDebugOnOff.Header = "Включить дебаг";
                        _statusDebug = false;
                    }
                });
            }
        }

        public ICommand bCreateConfig_Click
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ConfigDiseaseWindow wConfig = new ConfigDiseaseWindow(this);
                    wConfig.ShowDialog();
                });
            }
        }

        public ICommand bCreateSimulation_Click
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurSimulation.SetNewSession(
                        SizeWidth, SizeHeight,
                        AmountZdNothing, AmountZdMask, AmountZdSocDist, AmountZdAll,
                        AmountIncNothing, AmountIncMask, AmountIncSocDist, AmountIncAll,
                        AmountProdNothing, AmountProdMask, AmountProdSocDist, AmountProdAll,
                        AmountClinNothing, AmountClinMask, AmountClinSocDist, AmountClinAll,
                        AmountVzdNothing, AmountVzdMask, AmountVzdSocDist, AmountVzdAll,
                        AmountAsymptNothing, AmountAsymptMask, AmountAsymptSocDist, AmountAsymptAll);
                    _curNode = CurSimulation.Root;
                    ClearMap();
                    CreateFigures(CurSimulation.People);
                    AddFigures();

                    _regionBackground.Width = CurSimulation.SizeWidth;
                    _regionBackground.Height = CurSimulation.SizeHeight;
                    _lineVMP.Y1 = CurSimulation.SizeHeight / 2;
                    _lineVMP.X2 = CurSimulation.SizeWidth;
                    _lineVMP.Y2 = CurSimulation.SizeHeight / 2;

                    _lineHMP.X1 = CurSimulation.SizeWidth / 2;
                    _lineHMP.X2 = CurSimulation.SizeWidth / 2;
                    _lineHMP.Y2 = CurSimulation.SizeHeight;

                    WindowHeight = 750;
                    WindowWidth = 1920 / 2 + 60;

                    _scaleNode.ScaleX = Math.Min(WindowWidth / CurSimulation.SizeWidth, WindowHeight / CurSimulation.SizeHeight);
                    _scaleNode.ScaleY = Math.Min(WindowWidth / CurSimulation.SizeWidth, WindowHeight / CurSimulation.SizeHeight);

                    _timer = new DispatcherTimer();
                    _timer.Tick += new EventHandler(TimerTick);
                    _timer.Interval = new TimeSpan(10000);
                    _timer.Stop();
                    _statusTimer = false;
                });
            }
        }

        public ICommand bClearFields_Click
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SizeWidth = 10; SizeHeight = 10;
                    AmountZdNothing = 0; AmountZdMask = 0; AmountZdSocDist = 0; AmountZdAll = 0;
                    AmountIncNothing = 0; AmountIncMask = 0; AmountIncSocDist = 0; AmountIncAll = 0;
                    AmountProdNothing = 0; AmountProdMask = 0; AmountProdSocDist = 0; AmountProdAll = 0;
                    AmountClinNothing = 0; AmountClinMask = 0; AmountClinSocDist = 0; AmountClinAll = 0;
                    AmountVzdNothing = 0; AmountVzdMask = 0; AmountVzdSocDist = 0; AmountVzdAll = 0;
                    AmountAsymptNothing = 0; AmountAsymptMask = 0; AmountAsymptSocDist = 0; AmountAsymptAll = 0;
                    RaisePropertyChanged("SizeWidth"); RaisePropertyChanged("SizeHeight");
                    RaisePropertyChanged("AmountZdNothing"); RaisePropertyChanged("AmountZdMask"); 
                    RaisePropertyChanged("AmountZdSocDist"); RaisePropertyChanged("AmountZdAll");
                    RaisePropertyChanged("AmountIncNothing"); RaisePropertyChanged("AmountIncMask"); 
                    RaisePropertyChanged("AmountIncSocDist"); RaisePropertyChanged("AmountIncAll");
                    RaisePropertyChanged("AmountProdNothing"); RaisePropertyChanged("AmountProdMask"); 
                    RaisePropertyChanged("AmountProdSocDist"); RaisePropertyChanged("AmountProdAll");
                    RaisePropertyChanged("AmountClinNothing"); RaisePropertyChanged("AmountClinMask"); 
                    RaisePropertyChanged("AmountClinSocDist"); RaisePropertyChanged("AmountClinAll");
                    RaisePropertyChanged("AmountVzdNothing"); RaisePropertyChanged("AmountVzdMask"); 
                    RaisePropertyChanged("AmountVzdSocDist"); RaisePropertyChanged("AmountVzdAll");
                    RaisePropertyChanged("AmountAsymptNothing"); RaisePropertyChanged("AmountAsymptMask"); 
                    RaisePropertyChanged("AmountAsymptSocDist"); RaisePropertyChanged("AmountAsymptAll");
                });
            }
        }


        //===== Переключение между клетками ПЕРЕДЕЛАТЬ НА КОМАНДЫ
        private void miNodeChild_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            if (_curNode.Child(Convert.ToInt32(mi.Tag)) != null)
            {
                _curNode = _curNode.Child(Convert.ToInt32(mi.Tag));
                (double x, double y) p = _curNode.LVPoint;
               /*
                double tsx = _scaleNode.ScaleX;
                double tsy = _scaleNode.ScaleY;
                scaleNode.ScaleX = 1;
                scaleNode.ScaleY = 1;
                transNode.X -= p.x;
                transNode.Y -= p.y;
                scaleNode.ScaleX = tsx;
                scaleNode.ScaleY = tsy; 
               */
                MakeVisibleNodeHumans();
            }
        }

        private void miNodeParent_Click(object sender, RoutedEventArgs e)
        {
            if (_curNode.Parent != null)
            {
                _curNode = _curNode.Parent;
                MakeVisibleNodeHumans();
            }
        }


       





    }
}
