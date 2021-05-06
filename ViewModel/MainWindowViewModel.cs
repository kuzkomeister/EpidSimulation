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
        
       
        private Rectangle _backgroundMap;

        private DispatcherTimer _timer;
        private bool _statusTimer;

        private bool _statusDebug;

        
        Rectangle regionBackground;
        Line lineVMP;
        Line lineHMP;

        ScaleTransform scaleNode;
        TranslateTransform transNode;
        

        public MainWindowViewModel()
        {
            Config = new ConfigDisease();


            _simulation = new Simulation(
                150, 150,               // Размеры карты
                250, 0, 0, 0,           // Здоровые
                50, 0, 0, 0,            // Инкубационные
                0, 0, 0, 0,             // Продормальные
                0, 0, 0, 0,             // Клинические
                0, 0, 0, 0,             // Выздоровевшие
                Config);                // Параметры 
            _curNode = _simulation.Root;
            
            
            
            regionBackground = new System.Windows.Shapes.Rectangle
            {
                Width = _simulation.SizeX,
                Height = _simulation.SizeY,
                Fill = Brushes.LightGray,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            Canvas.SetLeft(regionBackground, 0);
            Canvas.SetTop(regionBackground, 0);
            CanvasMap.Children.Add(regionBackground);

            lineVMP = new Line
            {
                X1 = _simulation.SizeX / 2,
                Y1 = 0,
                X2 = _simulation.SizeX / 2,
                Y2 = _simulation.SizeY,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(lineVMP);

            lineHMP = new Line
            {
                X1 = 0,
                Y1 = _simulation.SizeY / 2,
                X2 = _simulation.SizeX,
                Y2 = _simulation.SizeY / 2,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(lineHMP);
            
            CreateFigures(_simulation.People);
            AddFigures();
            
            TransformGroup tfGroupNode = new TransformGroup();
            //tfGroupNode.Children.Add(scaleNode);
            //tfGroupNode.Children.Add(transNode);

            scaleNode = new ScaleTransform();
            CanvasMap.RenderTransform = scaleNode;
            scaleNode.ScaleX = 4;
            scaleNode.ScaleY = 4;

            transNode = new TranslateTransform();
            //CanvasMap.RenderTransform = transNode;
            transNode.X = 0;
            transNode.Y = 0;

            //CanvasMap.RenderTransform = tfGroupNode;
            
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = new TimeSpan(10000);
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
