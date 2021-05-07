using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EpidSimulation.Backend
{
    public class Human : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        static public ConfigDisease Config;

        private int _condition;  // Состояние человека
        public int Condition { get => _condition; set { _condition = value; OnPropertyChanged("Condition"); } }
        /*
            0 - здоровый 
            1 - инфицированный в инкубационном периоде
            2 - инфицированный в продормальном периоде
            3 - инфицированный в клиническом периоде
            4 - выздоровевший
            5 - бессимптомный
            6 - мертвый
         */

        //===== Параметры перемещения
        private double _x;   // Координата х
        private double _y;   // Координата у
        public double X { get => _x; set { _x = value; OnPropertyChanged("X"); } }
        public double Y { get => _y; set { _y = value; OnPropertyChanged("Y"); } }

        private double[] _vectorDirection;   // Вектор направления и скорости человека

        private delegate void MethodMove(Simulation simulation);
        private MethodMove Move;            // Метод перемещения 

        //===== Параметры влияющие на эпид ситуацию в случае конкретного человека
        public readonly bool Mask;      // Наличие маски на лице
        public readonly bool SocDist;   // Соблюдает социальную дистанцию

        private bool _infectHand;        // Наличие вируса на руках
        public bool InfectHand { get => _infectHand; set { _infectHand = value; OnPropertyChanged("InfectHand"); } }

        //===== Параметры для действия таймеров
        private int _timeChangeDirect;       // Время до смены направления
        private int _timeIncub;              // Время до окончания инкубационного периода
        private int _timeProdorm;            // Время до окончания продормального периода
        private int _timeRecovery;           // Время до выздоровления
        private int _timeInfHand;            // Время до "загрязнения" своих рук
        private int _timeMeet;               // Время до инициирования встречи/беседы
        private int _timeHandshake;          // Время до инициирования пожатия рук
        private int _timeHandToFaceContact;  // Время до контакта рук с лицом
        private int _timeWash;               // Время до мытья/дезинфицирования рук


        public Human(int condition, bool mask, bool socDist, double x, double y)
        {
            Condition = condition;
            _x = x; 
            _y = y;
            
            Mask = mask; 
            SocDist = socDist;
            _infectHand = false;
            Move = SocDist ? MoveWithSocDist : MoveWithoutCollision;

            // Направление движения
            _vectorDirection = new double[2];
            _vectorDirection[0] = Config.GetDirection();
            _vectorDirection[1] = Config.GetDirection();

            // Устанавливаем таймеры
            _timeIncub = Config.GetTimeIncub();
            _timeProdorm = Config.GetTimeProdorm();
            _timeRecovery = Config.GetTimeRecovery();
            _timeMeet = Config.GetTimeMeet();
            _timeHandToFaceContact = Config.GetTimeHandToFaceContact();
            _timeWash = Config.GetTimeWash();
            _timeChangeDirect = Config.GetTimeChangeDirect();
            _timeHandshake = Config.GetTimeContact();
            _timeInfHand = Config.GetTimeInfHand();
        }

        public void DoDela(Simulation simulation)
        {
            if (Condition != 6)
            {
                int oldCond = Condition;
                CourseDisease();
                WashHands();
                Handshake(simulation);
                Meet(simulation);
                if (Condition == 0)
                    TouchTheFace(simulation);
                Rotate();
                Move?.Invoke(simulation);

                simulation.SetAmountCond(oldCond, Condition);
            }
        }

        private void MoveWithSocDist(Simulation simulation)
        {
            bool needChangeDir = true;
            double oldDistance = simulation.GetNearDistance(this);

            for (int i = 0; i < Config.MaxTryes && needChangeDir; ++i)
            {
                if (simulation.CheckBarrier(X + _vectorDirection[0], Y + _vectorDirection[1]))
                {
                    double oldX = X;
                    double oldY = Y;

                    X += _vectorDirection[0];
                    Y += _vectorDirection[1];
                    needChangeDir = false;

                    double distance = simulation.GetNearDistance(this);

                    if (distance < Config.RadiusSocDistOptim && distance < oldDistance)
                    {
                        X = oldX;
                        Y = oldY;
                        needChangeDir = true;
                    }
                }
                else
                {
                    needChangeDir = true;
                }

                if (needChangeDir)
                {
                    _vectorDirection[0] = Config.GetDirection();
                    _vectorDirection[1] = Config.GetDirection();
                    _timeChangeDirect = Config.GetTimeChangeDirect();
                }
            }
        }

        private void MoveWithoutSocDist(Simulation simulation)
        {
            bool needChangeDir = true;
            for (int i = 0; i < Config.MaxTryes && needChangeDir; ++i)
            {
                if (simulation.CheckBarrier(X + _vectorDirection[0], Y + _vectorDirection[1]))
                {
                    double oldX = X;
                    double oldY = Y;

                    X += _vectorDirection[0];
                    Y += _vectorDirection[1];
                    needChangeDir = false;

                    double distance = simulation.GetNearDistance(this);

                    if (distance != -1 && distance < Config.RadiusHumanOptim)
                    {
                        X = oldX;
                        Y = oldY;
                        needChangeDir = true;
                    }
                }
                else
                {
                    needChangeDir = true;
                }

                if (needChangeDir)
                {
                    _vectorDirection[0] = Config.GetDirection();
                    _vectorDirection[1] = Config.GetDirection();
                    _timeChangeDirect = Config.GetTimeChangeDirect();
                }
            }
        }

        private void MoveWithoutCollision(Simulation simulation)
        {
            bool needChangeDir = true;
            for (int i = 0; i < Config.MaxTryes && needChangeDir; ++i)
            {
                if (simulation.CheckBarrier(X + _vectorDirection[0], Y + _vectorDirection[1]))
                {
                    double oldX = X;
                    double oldY = Y;

                    X += _vectorDirection[0];
                    Y += _vectorDirection[1];
                    needChangeDir = false;
                }
                else
                {
                    needChangeDir = true;
                }

                if (needChangeDir)
                {
                    _vectorDirection[0] = Config.GetDirection();
                    _vectorDirection[1] = Config.GetDirection();
                    _timeChangeDirect = Config.GetTimeChangeDirect();
                }
            }
        }

        private void Rotate()
        {
            if (_timeChangeDirect == 0)
            {
                _vectorDirection[0] = Config.GetDirection();
                _vectorDirection[1] = Config.GetDirection();
                _timeChangeDirect = Config.GetTimeChangeDirect();
            }
            else
            {
                _timeChangeDirect--;
            }
        }

        private void Meet(Simulation simulation)
        {
            if (_timeMeet == 0)
            {
                simulation.MakeMeet(this);
                _timeMeet = Config.GetTimeMeet();
            }
            else
            {
                _timeMeet--;
            }
        }

        private void Handshake(Simulation simulation)
        {
            if (_timeHandshake == 0)
            {
                simulation.MakeHandshake(this);
                _timeHandshake = Config.GetTimeContact();
            }
            else
            {
                _timeHandshake--;
            }
        }

        private void TouchTheFace(Simulation simulation)
        {
            if (_timeHandToFaceContact == 0)
            {
                if (_infectHand)
                {
                    if (Config.GetPermissionInfContact())
                    {
                        Condition = 1;
                        simulation.StHandshakesInf++;
                    }
                }
                _timeHandToFaceContact = Config.GetTimeHandToFaceContact();
            }
            else
            {
                _timeHandToFaceContact--;
            }
        }

        private void SetHandsDirty()
        {
            if (_timeInfHand == 0)
            {
                InfectHand = true;
                _timeInfHand = Config.GetTimeInfHand();
            }
            else
            {
                _timeInfHand--;
            }
        }

        private void WashHands()
        {
            if (_timeWash == 0)
            {
                InfectHand = false;
                _timeWash = Config.GetTimeWash();
            }
            else
            {
                _timeWash--;
            }
        }

        private void CourseDisease()
        {
            switch (Condition)
            {
                case 1:
                    if (_timeIncub == 0)
                    {
                        if (Config.GetPermissionAsymptomatic())
                            Condition = 5;
                        else
                            Condition = 2;
                    }
                    else
                        _timeIncub--;
                    break;

                case 2:
                    SetHandsDirty();
                    if (_timeProdorm == 0)
                        Condition = 3;
                    else
                        _timeProdorm--;
                    break;

                case 3:
                case 5:
                    SetHandsDirty();
                    if (_timeRecovery == 0)
                    {
                        if (Config.GetPermissionDie())
                            Condition = 5;
                        else
                            Condition = 4;
                    }
                    else
                        _timeRecovery--;
                    break;
            }
            /*
            if (Condition == 1)
            {
                // Инкубационный период
                if (_timeIncub == 0)
                { 
                    if (Config.GetPermissionAsymptomatic())
                        Condition = 6;
                    else
                        Condition = 2;      
                }
                else
                {
                    _timeIncub--;
                }
            }
            else if (Condition == 2)
            {
                // Продормальный период
                SetHandsDirty();
                if (_timeProdorm == 0)
                {
                    Condition = 3;                    
                }
                else
                {
                    _timeProdorm--;
                }
            }
            else if (Condition == 3 || Condition == 6)
            {
                // Клинический период (носительство)
                SetHandsDirty();
                if (_timeRecovery == 0)
                {
                    if (Config.GetPermissionDie())
                    {
                        Condition = 5;
                        _infectHand = false;
                    }
                    else
                    {
                        Condition = 4;
                    }
                }
                else
                {
                    _timeRecovery--;
                }
            }
            */
        }





    }
}
