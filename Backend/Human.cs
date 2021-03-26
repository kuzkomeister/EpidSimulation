using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EpidSimulation.Backend
{
    class Human : INotifyPropertyChanged
    {

        static public ConfigDisease Config;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private int _condition;  // Состояние человека
        public int Condition { get => _condition; set { _condition = value; OnPropertyChanged("Condition"); } }
        /*
         0 - здоровый, 1 - инфицированный в инкубационном периоде
         2 - инфицированный в клиническом периоде, 3 - выздоровевший
         4 - инфицированный без проявления симптомов, 5 - мертвый
         */

        //===== Параметры перемещения
        private double _x;   // Координата х
        private double _y;   // Координата у
        public double X { get => _x; set { _x = value; OnPropertyChanged("X"); } }
        public double Y { get => _y; set { _y = value; OnPropertyChanged("Y"); } }

        private double[] _vectorDirection;   // Вектор направления и скорости человека

        //===== Параметры влияющие на эпид ситуацию в случае конкретного человека
        public readonly bool Mask;      // Наличие маски на лице
        public readonly bool SocDist;   // Соблюдает социальную дистанцию
        private bool _infectHand;        // Наличие вируса на руках
        public bool InfectHand { get => _infectHand; set { _infectHand = value; OnPropertyChanged("InfectHand"); } }

        //===== Параметры для действия таймеров
        private int _timeChangeDirect;       // Время до смены направления
        private int _timeInfInc;             // Время до окончания инкубационного периода
        private int _timeRecovery;           // Время до выздоровления
        private int _timeInfHand;            // Время до "загрязнения" своих рук
        private int _timeMeet;               // Время до инициирования встречи/беседы
        private int _timeHandshake;          // Время до инициирования пожатия рук
        private int _timeHandToFaceContact;  // Время до контакта рук с лицом
        private int _timeWash;               // Время до мытья/дезинфицирования рук


        public Human(int condition, bool mask, bool socDist, double x, double y)
        {
            Condition = condition;
            _x = x; _y = y;
            Mask = mask; SocDist = socDist;
            _infectHand = false;

            _vectorDirection = new double[2];
            _vectorDirection[0] = Config.GetDirection();
            _vectorDirection[1] = Config.GetDirection();

            // Устанавливаем таймеры
            _timeInfInc = Config.GetTimeInfInc();
            _timeRecovery = Config.GetTimeRecovery();
            _timeMeet = Config.GetTimeMeet();
            _timeHandToFaceContact = Config.GetTimeHandToFaceContact();
            _timeWash = Config.GetTimeWash();
            _timeChangeDirect = Config.GetTimeChangeDirect();
            _timeHandshake = Config.GetTimeHandshake();
            _timeInfHand = Config.GetTimeInfHand();
        }

        public void DoDela(Simulation simulation)
        {
            if (Condition != 5)
            {
                int oldCond = Condition;
                CourseDisease();
                WashHands();
                Handshake(simulation);
                if (Condition == 2 || Condition == 4 || Condition == 0)
                    Meet(simulation);
                if (Condition == 0)
                    TouchTheFace(simulation);
                Rotate();
                Move(simulation);

                simulation.SetAmountCond(oldCond, Condition);
            }
        }

        private void Move(Simulation simulation)
        {
            double oldDist = 100;
            if (SocDist)
                oldDist = simulation.GetNearDistance(this);
            bool res = false;
            for (int i = 0; i < Config.MaxTryes; ++i)
            {
                res = true;
                if (simulation.CheckBarrier(X + _vectorDirection[0], Y + _vectorDirection[1]))
                {
                    double oldX = X;
                    double oldY = Y;

                    X += _vectorDirection[0];
                    Y += _vectorDirection[1];

                    double distance = simulation.GetNearDistance(this);

                    if (distance != -1)
                    {
                        if (SocDist)
                        {
                            if (distance < Config.RadiusSocOptim && distance < oldDist)
                            {
                                X = oldX;
                                Y = oldY;
                                res = false;
                            }
                        }
                        else
                        {
                            if (distance < Config.RadiusManOptim)
                            {
                                X = oldX;
                                Y = oldY;
                                res = false;
                            }
                        }
                    }
                }
                else
                {
                    res = false;
                }

                if (!res)
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
                int oldcond = Condition;
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
                _timeHandshake = Config.GetTimeHandshake();
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
                    if (Config.GetPermissionInfHand())
                    {
                        Condition = 1;
                        simulation.IncStHandshakeInf();
                    }
                }
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
            if (Condition == 1)
            {
                if (_timeInfInc == 0)
                {
                    if (Config.GetPermissionNotSymp())
                    {
                        Condition = 4;
                    }
                    else
                    {
                        Condition = 2;
                    }
                }
                else
                {
                    _timeInfInc--;
                }
            }
            else if (Condition == 2 || Condition == 4)
            {
                SetHandsDirty();
                if (_timeRecovery == 0)
                {
                    if (Config.GetPermissionDied() && Condition != 4)
                    {
                        Condition = 5;
                        _infectHand = false;
                    }
                    else
                    {
                        Condition = 3;
                    }
                }
                else
                {
                    _timeRecovery--;
                }
            }
        }





    }
}
