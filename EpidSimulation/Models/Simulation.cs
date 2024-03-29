﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EpidSimulation.Models
{
    public class Simulation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private QuadTree _root;
        public QuadTree Root { get => _root; }

        //===== Параметры карты
        private double _sizeWidth;
        public double SizeWidth
        {
            set => _sizeWidth = value > 10 ? value : 10;
            get => _sizeWidth;
        }

        private double _sizeHeight;
        public double SizeHeight 
        { 
            set => _sizeHeight = value > 10 ? value : 10; 
            get => _sizeHeight; 
        }

        //===== Статистика

        public bool StatusChanges { set; get; }

        #region Statistics

        public int AmountPeople
        {
            get => AmountZd + AmountInc + AmountProdorm + AmountClin + AmountAsympt + AmountVzd;
        }

        private double _stR0 = 0;
        public double StR0
        {
            set
            {
                _stR0 = value;
                OnPropertyChanged("StR0");
            }
            get => _stR0;
        }

        public void CalculateR0()
        {
            int amountPeople = 0;
            int SumInfects = 0;
            foreach(Human human in People)
            {
                if (human.Condition == HumanConditions.ProdromalInfected || 
                    human.Condition == HumanConditions.ClinicallyInfected || 
                    human.Condition == HumanConditions.AsymptomaticInfected)
                {
                    SumInfects += human.AmountInfects;
                    amountPeople++;
                }
            }
            StR0 = amountPeople != 0 ? (double) SumInfects / amountPeople : 0;
        }

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

        private int _stTouchesTheFaceWithInfect = 0;
        public int StTouchesTheFaceWithInfect
        {
            set
            {
                _stTouchesTheFaceWithInfect = value;
                OnPropertyChanged("StTouchesTheFaceWithInfect");
            }
            get => _stTouchesTheFaceWithInfect;
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

        private int _amountInc = 0;         // Инфицированных в инкубационном периоде
        public int AmountInc
        {
            set
            {
                _amountInc = value;
                OnPropertyChanged("AmountInc");
            }
            get => _amountInc;
        }

        private int _amountProdorm = 0;     // Инфицированных в продормальном периоде
        public int AmountProdorm 
        {
            set
            {
                _amountProdorm = value;
                OnPropertyChanged("AmountProdorm");
            }
            get => _amountProdorm; 
        }

        private int _amountClin = 0;        // Инфицированных в клиническом периоде
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

        private double _densityPeople = 0;
        public double DensityPeople
        {
            set
            {
                _densityPeople = value;
                OnPropertyChanged("DensityPeople");
            }
            get => _densityPeople;
        }
        #endregion

        public Simulation()
        {
            
        }

        
        public void SetNewSession(
            // Размер области
            double sizeX, double sizeY,
            // Настройки здоровых:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountZd, int amountMaskZd, int amountSocDistZd, int amountGoodHumanZd,
            // Настройки инфицированных в инкубационном периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountInc, int amountMaskInc, int amountSocDistInc, int amountGoodHumanInc,
            // Настройки инфицированных в продормальном периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountProd, int amountMaskProd, int amountSocDistProd, int amountGoodHumanProd,
            // Настройки инфицированных в клиническом периоде:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountClin, int amountMaskClin, int amountSocDistClin, int amountGoodHumanClin,
            // Настройки выздоровевших:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountVzd, int amountMaskVzd, int amountSocDistVzd, int amountGoodHumanVzd,
            // Настройки бессимптомных:
            // кол-во (здоровых, носящих маски, соблюдающих соц дистанцию, носящих и соблюдающих)
            int amountAsympt, int amountMaskAsympt, int amountSocDistAsympt, int amountGoodHumanAsympt,
            
            bool statusCollision)
        {
            QuadTree.RADIUS = Math.Max(
                Math.Max(Human.Config.RadiusHuman, Human.Config.RadiusSocDist),
                Math.Max(Human.Config.RadiusContact, Human.Config.RadiusAirborne));
            _root = new QuadTree(new NodeRegion(0, 0, sizeX, sizeY), null);
            //===== Параметры карты
            SizeWidth = sizeX;
            SizeHeight = sizeY;

            //===== Количество людей в состояниях
            AmountZd = amountZd + amountMaskZd + amountSocDistZd + amountGoodHumanZd;
            AmountInc = amountInc + amountMaskInc + amountSocDistInc + amountGoodHumanInc;
            AmountProdorm = amountProd + amountMaskProd + amountSocDistProd + amountGoodHumanProd;
            AmountClin = amountClin + amountMaskClin + amountSocDistClin + amountGoodHumanClin;
            AmountVzd = amountVzd + amountMaskVzd + amountSocDistVzd + amountGoodHumanVzd;
            AmountAsympt = amountAsympt + amountMaskAsympt + amountSocDistAsympt + amountGoodHumanAsympt;
            AmountDied = 0;
            int[,] amounts = new int[6, 4];
            // Здоровые
            amounts[0, 0] = amountZd; amounts[0, 1] = amountMaskZd; amounts[0, 2] = amountSocDistZd;
            amounts[0, 3] = amountGoodHumanZd;
            // Инфицированные в инкубационном и латентном периоде
            amounts[1, 0] = amountInc; amounts[1, 1] = amountMaskInc; amounts[1, 2] = amountSocDistInc;
            amounts[1, 3] = amountGoodHumanInc;
            // Инфицированные в инкубационном периоде
            amounts[2, 0] = amountProd; amounts[2, 1] = amountMaskProd; amounts[2, 2] = amountSocDistProd;
            amounts[2, 3] = amountGoodHumanProd;
            // Инфицированные в клиническом периоде
            amounts[3, 0] = amountClin; amounts[3, 1] = amountMaskClin; amounts[3, 2] = amountSocDistClin;
            amounts[3, 3] = amountGoodHumanClin;
            // Выздоровевшие
            amounts[4, 0] = amountVzd; amounts[4, 1] = amountMaskVzd; amounts[4, 2] = amountSocDistVzd;
            amounts[4, 3] = amountGoodHumanVzd;
            // Бессимптомные
            amounts[5, 0] = amountAsympt; amounts[5, 1] = amountMaskAsympt; amounts[5, 2] = amountSocDistAsympt;
            amounts[5, 3] = amountGoodHumanAsympt;


            Random random = new Random(DateTime.Now.Second * DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.Day);
            int maxTryes = 10;

            for (int cond = 0; cond < 6; cond++)
            {
                for (int attr = 0; attr < 4; attr++)
                {
                    for (int i = 0; i < amounts[cond, attr]; i++)
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
                            rx = random.NextDouble() * (SizeWidth - Human.Config.RadiusHuman - 1) + Human.Config.RadiusHuman + 1;
                            ry = random.NextDouble() * (SizeHeight - Human.Config.RadiusHuman - 1) + Human.Config.RadiusHuman + 1;
                            numTry++;

                            if (CheckBarrier(rx, ry))
                            {
                                human.Position = (rx, ry);
                                double distance = statusCollision ? GetNearDistance(human) : -1;
                                if (distance != -1)
                                {
                                    f = distance < Human.Config.RadiusHumanOptim;
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

                            if (numTry > maxTryes)
                            {
                                f = false;
                                switch (cond)
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
                                        AmountAsympt--;
                                        break;
                                }
                            }

                        } while (f);


                        if (numTry <= maxTryes)
                        {
                            Root.Insert(human);
                        }
                    }
                }
            }

            StContacts = 0; StContactsInf = 0;
            StHandShakes = 0; StHandshakesInf = 0;
            StTouchesTheFaceWithInfect = 0;
            StChecks = 0; StR0 = 0;
            Iter = 1; IterFinal = 0;
        }

        // Выполнение итерации симуляции
        public void Iterate()
        {
            StatusChanges = false;
            if (AmountPeople > 0)
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
                CalculateR0();
                DensityPeople = AmountPeople / (SizeWidth * SizeHeight);
            }
        }

        public void OnOffCollision(bool status)
        {
            foreach(Human human in People)
            {
                human.OnOffCollision(status);
            }
        }

        // Произвести общение/контакт/беседу/встречу
        public void MakeMeet(Human human)
        {
            LinkedList<Human> tempList = GetRegionPeople(human);    
            foreach (Human tempHuman in tempList)
            {
                if (Math.Pow(human.Position.X - tempHuman.Position.X, 2) + Math.Pow(human.Position.Y - tempHuman.Position.Y, 2) < Human.Config.RadiusAirborneOptim)
                {
                    if ((human.Condition == HumanConditions.ProdromalInfected || 
                        human.Condition == HumanConditions.ClinicallyInfected || 
                        human.Condition == HumanConditions.AsymptomaticInfected) && 
                        tempHuman.Condition == HumanConditions.Healthy)
                    {
                        StContacts++;
                        if (Human.Config.GetPermissionInfect(human.Mask, tempHuman.Mask))
                        {
                            human.AmountInfects++;
                            tempHuman.Condition = HumanConditions.IncubatedInfected;
                            SetAmountCond(0, 1);
                            StContactsInf++;
                        }
                    }
                    else if ((tempHuman.Condition == HumanConditions.ProdromalInfected || 
                              tempHuman.Condition == HumanConditions.ClinicallyInfected || 
                              human.Condition == HumanConditions.AsymptomaticInfected) && 
                              human.Condition == HumanConditions.Healthy)
                    {
                        StContacts++;
                        if (Human.Config.GetPermissionInfect(tempHuman.Mask, human.Mask))
                        {
                            tempHuman.AmountInfects++;
                            human.Condition = HumanConditions.IncubatedInfected;
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
                if (Math.Pow(human.Position.X - tempHuman.Position.X, 2) + Math.Pow(human.Position.Y - tempHuman.Position.Y,2) < Human.Config.RadiusContactOptim)
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
                resDistance = Math.Pow(human.Position.X - tempList.First.Value.Position.X, 2) + Math.Pow(human.Position.Y - tempList.First.Value.Position.Y, 2);
                tempList.RemoveFirst();
                StChecks++;
                foreach (Human tempHuman in tempList){
                    double distance = Math.Pow(human.Position.X - tempHuman.Position.X, 2) + Math.Pow(human.Position.Y - tempHuman.Position.Y, 2);
                    if (distance < resDistance)
                        resDistance = distance;
                }
                StChecks++;
            }
            return resDistance;
        }

        public bool CheckBarrier(double x, double y)
        {
            return ((0 <= x - Human.Config.RadiusHuman) && (x + Human.Config.RadiusHuman <= SizeWidth) && 
                    (0 <= y - Human.Config.RadiusHuman) && (y + Human.Config.RadiusHuman <= SizeHeight));
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
                        AmountAsympt--;
                        break;
                    case 6:
                        AmountDied--;
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
                        AmountAsympt++;
                        break;
                    case 6:
                        AmountDied++;
                        break;
                }

                StatusChanges = true;
            }
        }

    }
}
