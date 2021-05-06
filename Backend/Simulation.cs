using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EpidSimulation.Backend
{
    public class Simulation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private QuadTree _root;
        public QuadTree Root { get => _root; }

        //===== Параметры карты
        public readonly double SizeX;
        public readonly double SizeY;

        //===== Статистика

        /*
         *            СДЕЛАТЬ СТАТИСТИКУ: 
         *              1. ЛЮДЕЙ КОНТАКТИРОВАВШИХ С БОЛЬНЫМ, НО НЕ ЗАРАЗИВШИМСЯ
         *              2. Средняя по параметрам
         *              
         *             ПОсле инкубационного чел начинает действовать, это типо результат информирования людей об важности выполнения противоэпид действий
         *              
         */

        private int _stContacts = 0;        
        public int StContacts 
        { 
            set
            {
                _stContacts = value;
                OnPropertyChanged("StContacts");
            }
            get => _stContacts; 
        }
        
        private int _stContactsInf = 0;     
        public int StContactsInf 
        { 
            set
            {
                _stContactsInf = value;
                OnPropertyChanged("StContactsInf");
            } 
            get => _stContactsInf; 
        }

        private int _stHandshakes = 0;
        public int StHandShakes 
        { 
            set
            {
                _stHandshakes = value;
                OnPropertyChanged("StHandshakes");
            }
            get => _stHandshakes; 
        }

        private int _stHandshakesInf = 0;
        public int StHandshakesInf 
        { 
            set
            {
                _stHandshakesInf = value;
                OnPropertyChanged("StHandshakesInf");
            } 
            get => _stHandshakesInf; 
        }
      
        private int _stChecks = 0;
        public int StChecks
        {
            set
            {
                _stChecks = value;
                OnPropertyChanged("StChecks");
            }
            get => _stChecks;
        }

        private int _iter = 0;
        public int Iter 
        { 
            set
            {
                _iter = value;
                OnPropertyChanged("Iter");
            }
            get => _iter; 
        }
        
        private int _iterFinal = 0;
        public int IterFinal 
        { 
            set
            {
                _iterFinal = value;
                OnPropertyChanged("IterFinal");
            }
            get => _iterFinal; 
        }

        //===== Количество людей
        private int _amountZd = 0;          // Здоровых
        public int AmountZd 
        {
            set
            {
                _amountZd = value;
                OnPropertyChanged("AmountZd");
            } 
            get => _amountZd; 
        }

        private int _amountInc = 0;   // Инфицированных в инкубационном и латентном периоде
        public int AmountInc
        {
            set
            {
                _amountInc = value;
                OnPropertyChanged("AmountInc");
            }
            get => _amountInc;
        }

        private int _amountProdorm = 0;       // Инфицированных в инкубационном периоде
        public int AmountProdorm 
        {
            set
            {
                _amountProdorm = value;
                OnPropertyChanged("AmountProdorm");
            }
            get => _amountProdorm; 
        }

        private int _amountClin = 0;     // Инфицированных в клиническом периоде
        public int AmountClin 
        {
            set
            {
                _amountClin = value;
                OnPropertyChanged("AmountClin");
            }
            get => _amountClin; 
        }

        private int _amountVzd = 0;         // Выздоровевших
        public int AmountVzd 
        {
            set
            {
                _amountVzd = value;
                OnPropertyChanged("AmountVzd");
            }
            get => _amountVzd; 
        }

        private int _amountDied = 0;        // Умерших
        public int AmountDied 
        {
            set
            {
                _amountDied = value;
                OnPropertyChanged("AmountDied");
            }
            get => _amountDied; 
        }

        private int _amountAsympt = 0;      // Бессимптомные
        public int AmountAsympt
        {
            set
            {
                _amountAsympt = value;
                OnPropertyChanged("AmountAsympt");
            }
            get => _amountAsympt;
        }

        public Simulation(
            // Размер области
            double sizeX, double sizeY,
            // Настройки здоровых:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountZd, int amountMaskZd, int amountSocDistZd, int amountGoodHumanZd,
            // Настройки инфицированных в латентном периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountLat, int amountMaskLat, int amountSocDistLat, int amountGoodHumanLat,
            // Настройки инфицированных в инкубационном периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountInc, int amountMaskInc, int amountSocDistInc, int amountGoodHumanInc,
            // Настройки инфицированных в клиническом периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountClin, int amountMaskClin, int amountSocDistClin, int amountGoodHumanClin,
            // Настройки выздоровевших:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountVzd, int amountMaskVzd, int amountSocDistVzd, int amountGoodHumanVzd,
            // Настройки 
            ConfigDisease config)
        {
            Human.Config = config;  
            QuadTree.RADIUS = Math.Max(
                Math.Max(config.RadiusHuman, config.RadiusSocDist),
                Math.Max(config.RadiusContact, config.RadiusAirborne));        
            _root = new QuadTree(new NodeRegion(0, 0, sizeX, sizeY), null);
            //===== Параметры карты
            SizeX = sizeX;
            SizeY = sizeY;

            //===== Количество людей в состояниях
            AmountZd = amountZd + amountMaskZd + amountSocDistZd + amountGoodHumanZd;
            AmountInc = amountLat + amountMaskLat + amountSocDistLat + amountGoodHumanLat;
            AmountProdorm = amountInc + amountMaskInc + amountSocDistInc + amountGoodHumanInc;
            AmountClin = amountClin + amountMaskClin + amountSocDistClin + amountGoodHumanClin;
            AmountVzd = amountVzd + amountMaskVzd + amountSocDistVzd + amountGoodHumanVzd;

            int[,] amounts = new int[5,4];
            // Здоровые
            amounts[0, 0] = amountZd; amounts[0, 1] = amountMaskZd; amounts[0, 2] = amountSocDistZd;
            amounts[0, 3] = amountGoodHumanZd;
            // Инфицированные в инкубационном и латентном периоде
            amounts[1, 0] = amountLat; amounts[1, 1] = amountMaskLat; amounts[1, 2] = amountSocDistLat;
            amounts[1, 3] = amountGoodHumanLat;
            // Инфицированные в инкубационном периоде
            amounts[2, 0] = amountInc; amounts[2, 1] = amountMaskInc; amounts[2, 2] = amountSocDistInc;
            amounts[2, 3] = amountGoodHumanInc;
            // Инфицированные в клиническом периоде
            amounts[3, 0] = amountClin; amounts[3, 1] = amountMaskClin; amounts[3, 2] = amountSocDistClin;
            amounts[3, 3] = amountGoodHumanClin;
            // Выздоровевшие
            amounts[4, 0] = amountVzd; amounts[4, 1] = amountMaskVzd; amounts[4, 2] = amountSocDistVzd;
            amounts[4, 3] = amountGoodHumanVzd;
            
            Random random = new Random(322);
            int maxTryes = 10;

            for (int cond = 0; cond < 5; ++cond)
            {
                for (int attr = 0; attr < 4; ++attr)
                {
                    for (int i = 0; i < amounts[cond, attr]; ++i)
                    {
                        bool mask = false;
                        bool socDist = false;

                        switch (attr)
                        {
                            case 1:
                                mask = true;
                                break;
                            case 2:
                                socDist = true;
                                break;
                            case 3:
                                mask = true;
                                socDist = true;
                                break;
                        }

                        Human human = new Human(cond, mask, socDist, 0, 0);
                        double rx, ry;
                        bool f = true;

                        int numTry = 0;

                        do
                        {
                            rx = random.NextDouble() * (SizeX - 1) + 1;
                            ry = random.NextDouble() * (SizeY - 1) + 1;
                            numTry++;

                            if (CheckBarrier(rx, ry))
                            {
                                human.X = rx;
                                human.Y = ry;
                                double distance = GetNearDistance(human);
                                if (distance != -1)
                                {
                                    f = distance < 3 * Human.Config.RadiusHuman;
                                }
                                else
                                {
                                    f = false;
                                }
                            }
                            else
                            {
                                f = true;
                            }

                            if (numTry == maxTryes)
                            {
                                f = false;
                                switch (cond)
                                {
                                    case 0:
                                        _amountZd--;
                                        break;
                                    case 1:
                                        _amountInc--;
                                        break;
                                    case 2:
                                        _amountProdorm--;
                                        break;
                                    case 3:
                                        _amountClin--;
                                        break;
                                    case 4:
                                        _amountVzd--;
                                        break;
                                }
                            }

                        } while (f);


                        if (numTry < maxTryes)
                        {
                            _root.Insert(human);
                        }
                    }
                }
            }
            _stChecks = 0;
        }

        
        // Выполнение итерации симуляции
        public void Iterate()
        {
            // Получение списка узлов и их списка людей, с количеством людей на момент получения
            LinkedList<(QuadTree, int, LinkedList<Human>)> nodes = new LinkedList<(QuadTree, int, LinkedList<Human>)>();
            _root.GetNodeWithObjects(nodes);
            
            // Проход по списку узлов
            foreach ((QuadTree node, int countNode, LinkedList<Human> list) pair in nodes) 
            {
                // Проход по списку людей для их обработки
                LinkedListNode<Human> currentHuman = pair.list.First;   // Ссылка на текущий узел списка
                for (int i = 0; i < pair.countNode; ++i)
                {
                    currentHuman.Value.DoDela(this);                    // Обработка человека
                    LinkedListNode<Human> updateHuman = currentHuman;   // Ссылка на предыдущий, для обновления
                    currentHuman = currentHuman.Next;                   // Переход на следующий узел
                    pair.node.Update(updateHuman.Value);                // Обновление человека в дереве
                }
            }

            _root.Join();   // Объединение неполных узлов

            // Вычисление итерации на которой закончились инфицированные
            if (_amountClin == 0 && _amountProdorm == 0 && _amountInc == 0)
                IterFinal = Iter;

            Iter++;
        }

        // Произвести общение/контакт/беседу/встречу
        public void MakeMeet(Human human)
        {
            LinkedList<Human> tempList = GetRegionPeople(human);    
            foreach (Human tempHuman in tempList)
            {
                if (Math.Pow(human.X - tempHuman.X, 2) + Math.Pow(human.Y - tempHuman.Y, 2) < Human.Config.RadiusAirborneOptim)
                {
                    if ((human.Condition == 2 || human.Condition == 3) && tempHuman.Condition == 0)
                    {
                        StContacts++;
                        if (Human.Config.GetPermissionInfect(human.Mask, tempHuman.Mask))
                        {
                            tempHuman.Condition = 1;
                            SetAmountCond(0, 1);
                            StContactsInf++;
                        }
                    }
                    else if ((tempHuman.Condition == 2 || tempHuman.Condition == 3) && human.Condition == 0)
                    {
                        StContacts++;
                        if (Human.Config.GetPermissionInfect(tempHuman.Mask, human.Mask))
                        {
                            human.Condition = 1;
                            StContactsInf++;
                            break;
                        }
                    }
                }
            }
        }

        public void MakeHandshake(Human human)
        {
            LinkedList<Human> tempList = GetRegionPeople(human);
            foreach (Human tempHuman in tempList)
            {
                if (Math.Pow(human.X - tempHuman.X, 2) + Math.Pow(human.Y - tempHuman.Y,2) < Human.Config.RadiusContactOptim)
                {

                    if (human.InfectHand)
                    {
                        tempHuman.InfectHand = true;
                        StHandShakes++;
                    }
                    if (tempHuman.InfectHand)
                    {
                        human.InfectHand = true;
                        StHandShakes++;
                        break;
                    }
                }
            }
        }

        public double GetNearDistance(Human human)
        {
            LinkedList<Human> tempList = GetRegionPeople(human);
            double resDistance = -1;
            if (tempList.Count != 0)
            {
                resDistance = Math.Pow(human.X - tempList.First.Value.X, 2) + Math.Pow(human.Y - tempList.First.Value.Y, 2);
                tempList.RemoveFirst();
                StChecks++;
                foreach (Human tempHuman in tempList){
                    double distance = Math.Pow(human.X - tempHuman.X, 2) + Math.Pow(human.Y - tempHuman.Y, 2);
                    if (distance < resDistance)
                        resDistance = distance;
                }
                StChecks++;
            }
            return resDistance;
        }

        public bool CheckBarrier(double x, double y)
        {
            return ((0 <= x - Human.Config.RadiusHuman) && (x + Human.Config.RadiusHuman <= SizeX) && 
                    (0 <= y - Human.Config.RadiusHuman) && (y + Human.Config.RadiusHuman <= SizeY));
        }

        private LinkedList<Human> GetRegionPeople(Human human)
        {
            LinkedList<Human> tempList = new LinkedList<Human>();
            tempList = _root.Retrieve(tempList, human);
            tempList.Remove(human);
            return tempList;
        }

        public LinkedList<Human> People
        {
            get
            {
                LinkedList<(QuadTree, int, LinkedList<Human>)> nodes = new LinkedList<(QuadTree, int, LinkedList<Human>)>();
                _root.GetNodeWithObjects(nodes);
                LinkedList<Human> people = new LinkedList<Human>();
                foreach ((QuadTree, int, LinkedList<Human> nodePeople) pair in nodes)
                {
                    foreach (Human human in pair.nodePeople)
                    {
                        people.AddFirst(human);
                    }
                }
                return people;
            }
        }


        //===== Статистика
        public void SetAmountCond(int oldCond, int newCond)
        {
            if (oldCond != newCond)
            {
                switch (oldCond)
                {
                    case 0:
                        AmountZd--;
                        break;
                    case 1:
                        AmountInc--;
                        break;
                    case 2:
                        AmountProdorm--;
                        break;
                    case 3:
                        AmountClin--;
                        break;
                    case 4:
                        AmountVzd--;
                        break;
                    case 5:
                        AmountDied--;
                        break;
                    case 6:
                        AmountAsympt--;
                        break;
                }
                switch (newCond)
                {
                    case 0:
                        AmountZd++;
                        break;
                    case 1:
                        AmountInc++;
                        break;
                    case 2:
                        AmountProdorm++;
                        break;
                    case 3:
                        AmountClin++;
                        break;
                    case 4:
                        AmountVzd++;
                        break;
                    case 5:
                        AmountDied++;
                        break;
                    case 6:
                        AmountAsympt++;
                        break;
                }
            }
        }

    }
}
