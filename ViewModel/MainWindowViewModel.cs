using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using EpidSimulation.Backend;
using EpidSimulation.Frontend;
using GalaSoft.MvvmLight;

namespace EpidSimulation.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private Simulation _curSimulation;
        private ConfigDisease _curConfig;

        private Canvas _canvasMap;
        private LinkedList<FigureHuman> _figures;
        private QuadTree _curNode;
        public QuadTree CurNode
        {
            set
            {
                _curNode = value;
            }

            get
            {
                return _curNode;
            }
        }
        private Rectangle _backgroundMap;

        private DispatcherTimer _timer;
        private bool _statusTimer;

        private bool _statusDebug;

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
                    _canvasMap.Children.Add(figure.SocDistCircle);
            }

            foreach (FigureHuman figure in _figures)
            {
                _canvasMap.Children.Add(figure.CondCircle);
                if (figure.MaskCircle != null)
                    _canvasMap.Children.Add(figure.MaskCircle);

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
            _curSimulation.Iterate();

            if (_curNode != _curSimulation.Root)
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


    }
}
