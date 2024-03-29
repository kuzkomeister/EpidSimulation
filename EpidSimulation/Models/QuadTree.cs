﻿using System.Collections.Generic;
using System.Linq;

namespace EpidSimulation.Models
{
    public class NodeRegion
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Width;
        public readonly double Height;

        public NodeRegion(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    public class QuadTree
    {
        // Родительский узел
        private QuadTree _parent;               
        public QuadTree Parent { get => _parent; }

        // Дочерние узлы
        private QuadTree[] _childs;             
        public QuadTree Child(int ind) { return _childs[ind]; }
        /* Индексы дочерних квадрантов
            +---+---+
            | 1 | 0 |
            +---+---+
            | 2 | 3 |
            +---+---+                   */

        private LinkedList<Human> _people;      // Список объектов
        public LinkedList<Human> People { get => _people; }
        private int _count;                     // Количество объектов находящихся в данной области
        private NodeRegion _region;              // Область
        public (double, double, double, double) ParRegion { get => (_region.X, _region.Y, _region.Width, _region.Height); }
        public (double, double) LVPoint { get => (_region.X, _region.Y); }


        static int MIN_SIZE = 8;        // Минимальный размер области 
        public static double RADIUS;    // Радиус для распределения по областям
        static int MAX_OBJECTS = 50;    // Максимальное количество объектов на узле до деления

        // Количество созданных узлов
        private static int _amountNodes = -1;
        public static int AmountNodes { get => _amountNodes; }

        //==========
        public QuadTree(NodeRegion region, QuadTree parent)
        {
            _region = region;
            _people = new LinkedList<Human>();
            _childs = new QuadTree[4];
            _parent = parent;
            _count = 0;
            _amountNodes++;
        }

        // Удаление узла
        public void Clear()
        {
            _people.Clear();
            _people = null;
            _parent = null;
            _amountNodes--;
            for (int i = 0; i < _childs.Length; ++i)
            {
                if (_childs[i] != null)
                {
                    _childs[i].Clear();
                    _childs[i] = null;
                }
            }
        }

        // Деление узла
        private void Split()
        {
            double subWidth = _region.Width / 2;
            double subHeight = _region.Height / 2;
            double x = _region.X;
            double y = _region.Y;

            _childs[0] = new QuadTree(new NodeRegion(x + subWidth, y, subWidth, subHeight), this);
            _childs[1] = new QuadTree(new NodeRegion(x, y, subWidth, subHeight), this);
            _childs[2] = new QuadTree(new NodeRegion(x, y + subHeight, subWidth, subHeight), this);
            _childs[3] = new QuadTree(new NodeRegion(x + subWidth, y + subHeight, subWidth, subHeight), this);
        }

        // Получение индекса области, в которой находится объект
        // Индекс = -1 означает, что он находится на пересечении дочерних узлов
        private int GetIndex(Human human)
        {
            int index;
            double verticalMidPoint = _region.X + _region.Width / 2;
            double horizontalMidPoint = _region.Y + _region.Height / 2;

            if ((human.Position.X - RADIUS < verticalMidPoint && verticalMidPoint < human.Position.X + RADIUS) ||
                (human.Position.Y - RADIUS < horizontalMidPoint && horizontalMidPoint < human.Position.Y + RADIUS))
            { 
                index = -1;
            }
            else
            {
                if (human.Position.X > verticalMidPoint)
                {
                    if (human.Position.Y > horizontalMidPoint)
                        index = 3;
                    else
                        index = 0;
                }
                else
                {
                    if (human.Position.Y > horizontalMidPoint)
                        index = 2;
                    else
                        index = 1;
                }
            }
            return index;
        }

        // Вставка объекта в узел
        // Перемещение объекта по дереву вниз
        public void Insert(Human human)
        {
            _count++;
            if (_childs[0] != null)
            {
                int index = GetIndex(human);
                if (index != -1)
                {
                    _childs[index].Insert(human);
                    return;
                }
            }

            _people.AddLast(human);
            if (_people.Count > MAX_OBJECTS && (_region.Width > MIN_SIZE && _region.Height > MIN_SIZE))
            {
                if (_childs[0] == null)
                    Split();

                int i = 0;
                while (i < _people.Count)
                {
                    int index = GetIndex(_people.ElementAt(i));
                    if (index != -1)
                    {
                        _childs[index].Insert(_people.ElementAt(i));
                        _people.Remove(_people.ElementAt(i));
                    }
                    else
                    {
                        i++;
                    }
                }

            }
        }

        // Получение списка объектов, которые могут пересекаться с заданным объектом
        public LinkedList<Human> Retrieve(LinkedList<Human> returnPeople, Human human)
        {
            int index = GetIndex(human);
            if (_childs[0] != null && index != -1)
            {
                _childs[index].Retrieve(returnPeople, human);
            }
            foreach (Human tempHuman in _people)
                returnPeople.AddFirst(tempHuman);

            return returnPeople;
        }

        // Объединение неполных узлов
        public void Join()
        {
            if (_childs[0] != null)
            {
                for (int i = 0; i < _childs.Length; ++i)
                {
                    _childs[i].Join();
                }

                if (_count < MAX_OBJECTS)
                {
                    for (int i = 0; i < _childs.Length; ++i)
                    {
                        foreach(Human human in _childs[i]._people)
                        {
                            _people.AddFirst(human);
                        }
                        _childs[i].Clear();
                        _childs[i] = null;
                    }
                }
            }
        }

        // Обновление объекта в узле
        public void Update(Human human)
        {
            _people.Remove(human);
            _count--;
            if (human.Condition != HumanConditions.Dead)
            {
                _count++;
                Relocate(human);
            }
        }

        // Перемещение объекта по дереву вверх
        private void Relocate(Human human)
        {
            _count--;
            if (_region.X < human.Position.X - RADIUS && human.Position.X + RADIUS < _region.Width &&
                _region.Y < human.Position.Y - RADIUS && human.Position.Y + RADIUS < _region.Height)
            {
                Insert(human);
            }
            else
            {
                if (_parent != null)
                    _parent.Relocate(human);
                else
                    Insert(human);
            }

        }

        // Получение списка узлов и списков их объектов
        public LinkedList<(QuadTree, int, LinkedList<Human>)> GetNodeWithObjects(LinkedList<(QuadTree, int, LinkedList<Human>)> returnList)
        {
            returnList.AddFirst((this, _people.Count, _people));
            if (_childs[0] != null)
            {
                for (int i = 0; i < _childs.Length; ++i)
                {
                    _childs[i].GetNodeWithObjects(returnList);
                }
            }
            return returnList;
        }

        // Получение списка списков объектов
        public LinkedList<LinkedList<Human>> GetListObjects(LinkedList<LinkedList<Human>> returnList)
        {
            returnList.AddFirst(_people);
            if (_childs[0] != null)
            {
                for (int i = 0; i < _childs.Length; ++i)
                    _childs[i].GetListObjects(returnList);
            }
            return returnList;
        }

    }

    
}
