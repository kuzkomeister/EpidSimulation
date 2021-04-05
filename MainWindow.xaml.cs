using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using EpidSimulation.Backend;
using EpidSimulation.Frontend;

namespace EpidSimulation
{
    public partial class MainWindow : Window
    {
        Simulation Sim;
        ConfigDisease Config;
        LinkedList<FigureHuman> Figures;

        QuadTree curNode;
        System.Windows.Shapes.Rectangle regionBackground;
        Line lineVMP;
        Line lineHMP;
 
        ScaleTransform scaleNode;
        TranslateTransform transNode;

        DispatcherTimer timer;
        bool StatusTimer = false;

        bool Debug = false;

        private void CreateFigures(LinkedList<Human> people)
        {
            Figures = new LinkedList<FigureHuman>();
            foreach (Human human in people)
            {
                FigureHuman figure = new FigureHuman(human);
                Figures.AddFirst(figure);
            }
        }

        private void AddFigures()
        {
            foreach(FigureHuman figure in Figures)
            {
                if (figure.SocDistCircle != null)
                    CanvasMap.Children.Add(figure.SocDistCircle);
            }

            foreach (FigureHuman figure in Figures)
            {
                CanvasMap.Children.Add(figure.CondCircle);
                if (figure.MaskCircle != null)
                    CanvasMap.Children.Add(figure.MaskCircle);

            }
        }

        private void ClearDeads()
        {
            for (LinkedListNode<FigureHuman> figure = Figures.First; figure != null;)
            {
                if (figure.Value.CondCircle.Fill == Brushes.DarkGray)
                {
                    CanvasMap.Children.Remove(figure.Value.CondCircle);
                    if (figure.Value.MaskCircle != null)
                    {
                        CanvasMap.Children.Remove(figure.Value.MaskCircle);
                    }
                    if (figure.Value.SocDistCircle != null)
                    {
                        CanvasMap.Children.Remove(figure.Value.SocDistCircle);
                    }
                    LinkedListNode<FigureHuman> temp = figure;
                    figure = figure.Next;
                    Figures.Remove(temp);
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
            curNode.GetListObjects(tempList);

            foreach (FigureHuman figure in Figures)
            {
                bool vis = false;
                foreach(LinkedList<Human> l in tempList)
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
                    figure.SetVisible(0, Debug);
                }
                else
                {
                    figure.SetVisible(1, Debug);
                }
            }
        }

        public MainWindow()
        {
            Config = new ConfigDisease(
                100, 120,   // Meet
                140, 160,   // HandToFaceContact
                130, 200,   // Wash
                50, 100,    // ChangeDirect
                50, 200,    // Handshake
                50, 100,    // InfHand
                90, 100,    // InfInc
                900, 1000,  // Recovery
                0.2f,       // ProbabilityNotSymp
                0.1f,       // ProbabilityDied
                0.1f,       // ProbabilityInfHand
                0.0f, 0.1f,// MaskProtection: For, From
                2.0f, 1.0f, 1.5f, 1.0f, // Radius: SocDist, Man, Inf, Handshake
                0.3f, 2);   // MaxDist, MaxTryes

            Sim = new Simulation(150, 150,
            0, 0, 250, 0,
            0, 0, 0, 0, 
            10, 0, 40, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            Config);
            curNode = Sim.Root;

            InitializeComponent();

            regionBackground = new System.Windows.Shapes.Rectangle
            {
                Width = Sim.SizeX,
                Height = Sim.SizeY,
                Fill = Brushes.LightGray,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            Canvas.SetLeft(regionBackground, 0);
            Canvas.SetTop(regionBackground, 0);
            CanvasMap.Children.Add(regionBackground);

            lineVMP = new Line
            {
                X1 = Sim.SizeX / 2,
                Y1 = 0,
                X2 = Sim.SizeX / 2,
                Y2 = Sim.SizeY,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(lineVMP);

            lineHMP = new Line
            {
                X1 = 0,
                Y1 = Sim.SizeY / 2,
                X2 = Sim.SizeX,
                Y2 = Sim.SizeY / 2,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(lineHMP);
            
            CreateFigures(Sim.People);
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

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(10000);            
        }


        
        // Итерация основного таймера 
        private void TimerTick(object sender, EventArgs e)
        {
            Sim.Iterate();

            if (curNode != Sim.Root)
                MakeVisibleNodeHumans();

            ClearDeads();

            curIter.Content = "Текущая итерация: " + Sim.Iter;
            stat.Content = "Здоровых: " + Sim.AmountZd + "\n" +
                           "Инфицированных в инкубационном периоде: " + Sim.AmountInfInc + "\n" +
                           "Инфицированных в клиническом периоде: " + Sim.AmountInfSymp + "\n" +
                           "Инфицированных бессимптомных: " + Sim.AmountInfNotSymp + "\n" +
                           "С иммунитетом: " + Sim.AmountVzd + "\n" +
                           "Умерших: " + Sim.AmountDied;

            debug.Content = "Статистика:\n" +
                           "Всего контактов: " + Sim.StContacts + "\n" +
                           "Инфицированных контактов: " + Sim.StContactsInf + "\n" +
                           "Всего рукопожатий: " + Sim.StHandShakes + "\n" +
                           "Заражений с рук: " + Sim.StHandshakesInf + "\n";
        }

        // Включить/Выключить симуляцию
        private void miSimOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (StatusTimer)
            {
                timer.Stop();
                StatusTimer = false;
                miSimOnOff.Header = "Включить";
            }
            else
            {
                timer.Start();
                StatusTimer = true;
                miSimOnOff.Header = "Выключить";
            }
        }

        // Включить/Выключить отображение соц дистанции
        private void miDebugOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (!Debug)
            {
                foreach (FigureHuman figure in Figures)
                {
                    if (figure.SocDistCircle != null && figure.CondCircle.Visibility == 0)
                        figure.SocDistCircle.Visibility = Visibility.Visible;
                }
                miDebugOnOff.Header = "Выключить дебаг";
                Debug = true;
            }
            else
            {
                foreach (FigureHuman figure in Figures)
                {
                    if (figure.SocDistCircle != null)
                        figure.SocDistCircle.Visibility = Visibility.Hidden;
                }
                miDebugOnOff.Header = "Включить дебаг";
                Debug = false;
            }
        }


        //===== Переключение между клетками
        private void miNodeChild_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            if (curNode.Child(Convert.ToInt32(mi.Tag)) != null)
            {
                curNode = curNode.Child(Convert.ToInt32(mi.Tag));
                (double x, double y) p = curNode.LVPoint;
                double tsx = scaleNode.ScaleX;
                double tsy = scaleNode.ScaleY;
                scaleNode.ScaleX = 1;
                scaleNode.ScaleY = 1;
                transNode.X -= p.x;
                transNode.Y -= p.y;
                scaleNode.ScaleX = tsx;
                scaleNode.ScaleY = tsy;
                MakeVisibleNodeHumans();
            }
        }

        private void miNodeParent_Click(object sender, RoutedEventArgs e)
        {
            if (curNode.Parent != null)
            {
                curNode = curNode.Parent;
                MakeVisibleNodeHumans();
            }
        }





    }
}
