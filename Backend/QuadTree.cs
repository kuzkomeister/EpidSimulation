using System.Collections.Generic;
using System.Linq;

namespace EpidSimulation.Backend
{
    class Rectangle
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Width;
        public readonly double Height;

        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    class QuadTree
    {
        private QuadTree _parent;
        public QuadTree Parent { get => _parent; }

        private QuadTree[] _childs;
        public QuadTree Child(int ind) { return _childs[ind]; }
        /* Индексы дочерних квадрантов
            +---+---+
            | 1 | 0 |
            +---+---+
            | 2 | 3 |
            +---+---+                   */

        private LinkedList<Human> _people;
        private int _count;
        private Rectangle _region;


        static int MIN_SIZE = 8;
        public static double RADIUS;
        static int MAX_OBJECTS = 50;

        private static int _amountNodes = -1;
        public static int AmountNodes { get => _amountNodes; }

        public QuadTree(Rectangle region, QuadTree parent)
        {
            _region = region;
            _people = new LinkedList<Human>();
            _childs = new QuadTree[4];
            _parent = parent;
            _count = 0;
            _amountNodes++;
        }

        public string GetInfo(int t)
        {
            string s = "";
            if (_childs[0] != null)
            {
                s += _childs[0].GetInfo(t + 1);
                s += _childs[1].GetInfo(t + 1);
                s += _childs[2].GetInfo(t + 1);
                s += _childs[3].GetInfo(t + 1);
            }
            for (int i = 0; i < t; ++i) s += "\t";
            s += "ЛП: (" +_region.X + ", " + _region.Y + ") ШВ: (" + _region.Width + ", " + _region.Height + ")\n";
            for (int i = 0; i < t; ++i) s += "\t";
            s += "Челики:\n";
            foreach (Human human in _people)
            {
                for (int i = 0; i < t; ++i) s += "\t";
                s += "(" + human.X + ", " + human.Y + ")\n";
            }

            return s;
        }

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

        private void Split()
        {
            double subWidth = _region.Width / 2;
            double subHeight = _region.Height / 2;
            double x = _region.X;
            double y = _region.Y;

            _childs[0] = new QuadTree(new Rectangle(x + subWidth, y, subWidth, subHeight), this);
            _childs[1] = new QuadTree(new Rectangle(x, y, subWidth, subHeight), this);
            _childs[2] = new QuadTree(new Rectangle(x, y + subHeight, subWidth, subHeight), this);
            _childs[3] = new QuadTree(new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight), this);
        }

        private int GetIndex(Human human)
        {
            int index;
            double verticalMidPoint = _region.X + _region.Width / 2;
            double horizontalMidPoint = _region.Y + _region.Height / 2;

            if ((human.X - RADIUS < verticalMidPoint && verticalMidPoint < human.X + RADIUS) ||
                (human.Y - RADIUS < horizontalMidPoint && horizontalMidPoint < human.Y + RADIUS))
            {
                index = -1;
            }
            else
            {
                if (human.X > verticalMidPoint)
                {
                    if (human.Y > horizontalMidPoint)
                        index = 3;
                    else
                        index = 0;
                }
                else
                {
                    if (human.Y > horizontalMidPoint)
                        index = 2;
                    else
                        index = 1;
                }
            }
            return index;
        }

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

        public void Update(Human human)
        {
            _people.Remove(human);
            if (human.Condition != 5)
                Relocate(human);
        }

        private void Relocate(Human human)
        {
            _count--;
            if (_region.X < human.X - RADIUS && human.X + RADIUS < _region.Width &&
                _region.Y < human.Y - RADIUS && human.Y + RADIUS < _region.Height)
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

        public LinkedList<(QuadTree, int, LinkedList<Human>)> GetAllPeople(LinkedList<(QuadTree, int, LinkedList<Human>)> returnList)
        {
            returnList.AddFirst((this, _people.Count, _people));
            if (_childs[0] != null)
            {
                for (int i = 0; i < _childs.Length; ++i)
                {
                    _childs[i].GetAllPeople(returnList);
                }
            }
            
            return returnList;
        }

        

    }

    
}
