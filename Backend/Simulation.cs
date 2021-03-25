using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EpidSimulation.Backend
{
    class Simulation
    {
        private QuadTree _root;
        public QuadTree Root { get => _root; }

        public string DebugString = "";

        //===== Параметры карты
        public readonly double SizeX;
        public readonly double SizeY;

        //===== Статистика
        private int _stContacts = 0;        
        public int StContacts { get => _stContacts; }
        
        private int _stContactsInf = 0;     
        public int StContactsInf { get => _stContactsInf; }

        private int _stHandshakes = 0;
        public int StHandShakes { get => _stHandshakes; }

        private int _stHandshakesInf = 0;
        public int StHandshakesInf { get => _stHandshakesInf; }
        public int _stChecks = 0;

        private int _iter = 0;
        public int Iter { get => _iter; }
        
        private int _iterFinal = 0;
        public int IterFinal { get => _iterFinal; }

        //===== Количество людей
        private int _amountZd = 0;          // Здоровых
        public int AmountZd { get => _amountZd; }

        private int _amountInfInc = 0;       // Инфицированных в инкубационном периоде
        public int AmountInfInc { get => _amountInfInc; }

        private int _amountInfSymp = 0;     // Инфицированных в клиническом периоде
        public int AmountInfSymp { get => _amountInfSymp; }

        private int _amountInfNotSymp = 0;  // Инфицированных бессимптомных
        public int AmountInfNotSymp { get => _amountInfNotSymp; }

        private int _amountVzd = 0;         // Выздоровевших
        public int AmountVzd { get => _amountVzd; }

        private int _amountDied = 0;        // Умерших
        public int AmountDied { get => _amountDied; }

        public Simulation(
            // Размер области
            double sizeX, double sizeY,
            // Настройки здоровых:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountZd, int amountMaskZd, int amountSocDistZd, int amountGoodHumanZd,
            // Настройки инфицированных в инкубационном периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountInfInc, int amountMaskInfInc, int amountSocDistInfInc, int amountGoodHumanInfInc,
            // Настройки инфицированных в клиническом периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountInfSymp, int amountMaskInfSymp, int amountSocDistInfSymp, int amountGoodHumanInfSymp,
            // Настройки инфицированных бессимптомных:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountInfNotSymp, int amountMaskInfNotSymp, int amountSocDistInfNotSymp, int amountGoodHumanInfNotSymp,
            // Настройки выздоровевших:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountVzd, int amountMaskVzd, int amountSocDistVzd, int amountGoodHumanVzd,
            // Настройки 
            ConfigDisease config)
        {
            Human.Config = config;  
            QuadTree.RADIUS = Math.Max(
                Math.Max(config.RadiusMan, config.RadiusSoc),
                Math.Max(config.RadiusHandshake, config.RadiusInf));        
            _root = new QuadTree(new Rectangle(0, 0, sizeX, sizeY), null);
            //===== Параметры карты
            SizeX = sizeX;
            SizeY = sizeY;

            //===== Количество людей в состояниях
            _amountZd = amountZd + amountMaskZd + amountSocDistZd + amountGoodHumanZd;
            _amountInfInc = amountInfInc + amountMaskInfInc + amountSocDistInfInc + amountGoodHumanInfInc;
            _amountInfSymp = amountInfSymp + amountMaskInfSymp + amountSocDistInfSymp + amountGoodHumanInfSymp;
            _amountInfNotSymp = amountInfNotSymp + amountMaskInfNotSymp + amountSocDistInfNotSymp + amountGoodHumanInfNotSymp;
            _amountVzd = amountVzd + amountMaskVzd + amountSocDistVzd + amountGoodHumanVzd;

            int[,] amounts = new int[5,4];
            // Здоровые
            amounts[0, 0] = amountZd; amounts[0, 1] = amountMaskZd; amounts[0, 2] = amountSocDistZd;
            amounts[0, 3] = amountGoodHumanZd;
            // Инфицированные в инкубационном периоде
            amounts[1, 0] = amountInfInc; amounts[1, 1] = amountMaskInfInc; amounts[1, 2] = amountSocDistInfInc;
            amounts[1, 3] = amountGoodHumanInfInc;
            // Инфицированные в клиническом периоде
            amounts[2, 0] = amountInfSymp; amounts[2, 1] = amountMaskInfSymp; amounts[2, 2] = amountSocDistInfSymp;
            amounts[2, 3] = amountGoodHumanInfSymp;
            // Выздоровевшие
            amounts[3, 0] = amountVzd; amounts[3, 1] = amountMaskVzd; amounts[3, 2] = amountSocDistVzd;
            amounts[3, 3] = amountGoodHumanVzd;
            // Инфицированные бессимптомные
            amounts[4, 0] = amountInfNotSymp; amounts[4, 1] = amountMaskInfNotSymp; amounts[4, 2] = amountSocDistInfNotSymp;
            amounts[4, 3] = amountGoodHumanInfNotSymp;

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
                                    f = distance < 3 * Human.Config.RadiusMan;
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
                                        _amountInfInc--;
                                        break;
                                    case 2:
                                        _amountInfSymp--;
                                        break;
                                    case 3:
                                        _amountVzd--;
                                        break;
                                    case 4:
                                        _amountInfNotSymp--;
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

        

        public void Iterate()
        {
            LinkedList<(QuadTree, int, LinkedList<Human>)> nodes = new LinkedList<(QuadTree, int, LinkedList<Human>)>();
            _root.GetAllPeople(nodes);
            
            int countDel = 0;
            DebugString = ""; 

            foreach ((QuadTree node, int countNode, LinkedList<Human> list) pair in nodes) 
            {
                LinkedListNode<Human> currentHuman = pair.list.First;     
                int n = pair.countNode;
                for (int i = 0; i < n; ++i)
                {
                    countDel++;
                    currentHuman.Value.DoDela(this);

                    LinkedListNode<Human> temp = currentHuman;
                    currentHuman = currentHuman.Next;
                    pair.node.Update(temp.Value);   
                }
            }

            DebugString += "Количество совершенных делишек: " + countDel + "\n";
            DebugString += "Количество узлов дерева: " + QuadTree.AmountNodes + "\n";
            //DebugIter += _root.GetInfo(0);

            _root.Join();


            if (_amountInfSymp == 0 && _amountInfInc == 0 && _amountInfNotSymp == 0)
                _iterFinal = _iter;

            _iter++;
        }

        public void MakeMeet(Human human)
        {
            LinkedList<Human> tempList = GetRegionPeople(human);
            foreach (Human tempHuman in tempList)
            {
                if (Math.Pow(human.X - tempHuman.X, 2) + Math.Pow(human.Y - tempHuman.Y, 2) < Human.Config.RadiusInfOptim)
                {
                    IncStContacts();
                    if ((human.Condition == 2 || human.Condition == 4) && tempHuman.Condition == 0)
                    {
                        if (Human.Config.GetPermissionInfect(human.Mask, tempHuman.Mask))
                        {
                            tempHuman.Condition = 1;
                            SetAmountCond(0, 1);
                            IncStContactsInf();
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
                if (Math.Pow(human.X - tempHuman.X, 2) + Math.Pow(human.Y - tempHuman.Y,2) < Human.Config.RadiusHandshakeOptim)
                {
                    IncStHandshakes();
                    if (human.InfectHand)
                        tempHuman.InfectHand = true;
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
                _stChecks++;
                foreach (Human tempHuman in tempList){
                    double distance = Math.Pow(human.X - tempHuman.X, 2) + Math.Pow(human.Y - tempHuman.Y, 2);
                    if (distance < resDistance)
                        resDistance = distance;
                }
                _stChecks++;
            }
            return resDistance;
        }

        public bool CheckBarrier(double x, double y)
        {
            return ((0 <= x - Human.Config.RadiusMan) && (x + Human.Config.RadiusMan <= SizeX) && 
                    (0 <= y - Human.Config.RadiusMan) && (y + Human.Config.RadiusMan <= SizeY));
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
                _root.GetAllPeople(nodes);
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
                        _amountZd--;
                        break;
                    case 1:
                        _amountInfInc--;
                        break;
                    case 2:
                        _amountInfSymp--;
                        break;
                    case 3:
                        _amountVzd--;
                        break;
                    case 4:
                        _amountInfNotSymp--;
                        break;
                    case 5:
                        _amountDied--;
                        break;
                }
                switch (newCond)
                {
                    case 0:
                        _amountZd++;
                        break;
                    case 1:
                        _amountInfInc++;
                        break;
                    case 2:
                        _amountInfSymp++;
                        break;
                    case 3:
                        _amountVzd++;
                        break;
                    case 4:
                        _amountInfNotSymp++;
                        break;
                    case 5:
                        _amountDied++;
                        break;
                }
            }
        }

        public void IncStContacts()
        {
            _stContacts++;
        }

        public void IncStContactsInf()
        {
            _stContactsInf++;
        }

        public void IncStHandshakes()
        {
            _stHandshakes++;
        }

        public void IncStHandshakeInf()
        {
            _stHandshakesInf++;
        }
    }
}
