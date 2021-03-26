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
        ScaleTransform st;

        DispatcherTimer timer;
        bool StatusTimer = false;

        bool Debug = true;

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
                0.5f, 0.25f,// MaskProtection: For, From
                2.0f, 1.0f, 1.5f, 1.0f, // Radius: SocDist, Man, Inf, Handshake
                0.3f, 2);   // MaxDist, MaxTryes

            Sim = new Simulation(150, 150,
            350, 0, 0, 0,
            0, 0, 0, 0,
            50, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            Config);

            InitializeComponent();

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
            {
                Width = Sim.SizeX,
                Height = Sim.SizeY,
                Fill = Brushes.LightGray,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            Canvas.SetLeft(rectangle, 0);
            Canvas.SetTop(rectangle, 0);
            CanvasMap.Children.Add(rectangle);

            Line lineVMP = new Line
            {
                X1 = Sim.SizeX / 2,
                Y1 = 0,
                X2 = Sim.SizeX / 2,
                Y2 = Sim.SizeY,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            };
            CanvasMap.Children.Add(lineVMP);

            Line lineHMP = new Line
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

            st = new ScaleTransform();
            CanvasMap.RenderTransform = st;
            st.ScaleX += 2.5;
            st.ScaleY += 2.5;
            

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(10000);
            
        }


        

        private void TimerTick(object sender, EventArgs e)
        {
            Sim.Iterate();

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

        private void miSimOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (StatusTimer)
            {
                timer.Stop();
                StatusTimer = false;
                miSimOnOff.Header = "Выключить";
            }
            else
            {
                timer.Start();
                StatusTimer = true;
                miSimOnOff.Header = "Выключить";
            }
        }

        private void miDebugOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (Debug)
            {
                foreach (FigureHuman figure in Figures)
                {
                    if (figure.SocDistCircle != null)
                        figure.SocDistCircle.Visibility = Visibility.Visible;
                }
                miDebugOnOff.Header = "Выключить дебаг";
                Debug = false;
            }
            else
            {
                foreach (FigureHuman figure in Figures)
                {
                    if (figure.SocDistCircle != null)
                        figure.SocDistCircle.Visibility = Visibility.Hidden;
                }
                miDebugOnOff.Header = "Включить дебаг";
                Debug = true;
            }
        }

       
    }
}
