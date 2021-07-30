using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EpidSimulation.Models
{
    public class Human : INotifyPropertyChanged
    {
        static public Config Config;

        #region [ Поля и свойства ]
        private HumanConditions _condition;  // Состояние человека
        public HumanConditions Condition 
        { 
            get => _condition; 
            set 
            { 
                _condition = value; 
                OnPropertyChanged(); 
            } 
        }

        //===== Параметры перемещения
        private (double, double) _position;     
        /// <summary>
        /// Местоположение человека
        /// </summary>
        public (double X, double Y) Position    
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Y));
            }
        } 
        public double X
        {
            get => Position.X;
        }
        public double Y
        {
            get => Position.Y;
        }

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

        //===== Статистика
        private int _amountInfects = 0;     // Количество заражений 
        public int AmountInfects 
        {
            set => _amountInfects = value;
            get => _amountInfects;
        }

        #endregion

        

        public void DoDela(Simulation simulation)
        {
            if (Condition != HumanConditions.Dead)
            {
                HumanConditions oldCond = Condition;
                CourseDisease();
                Meet(simulation);
                ContactTM(simulation);
                Rotate();
                Move?.Invoke(simulation);

                simulation.SetAmountCond((int)oldCond, (int)Condition);
            }
        }

        public void OnOffCollision(bool status)
        {
            if (status)
                Move = SocDist ? MoveWithSocDist : MoveWithoutSocDist;
            else
                Move = SocDist ? MoveWithSocDist : MoveWithoutCollision;
            
        }

        #region MoveMethods

        private void MoveWithSocDist(Simulation simulation)
        {
            bool needChangeDir = true;
            double oldDistance = simulation.GetNearDistance(this);

            for (int i = 0; i < Config.MaxTryes && needChangeDir; ++i)
            {
                if (simulation.CheckBarrier(Position.X + _vectorDirection[0], Position.Y + _vectorDirection[1]))
                {
                    double oldX = Position.X;
                    double oldY = Position.Y;

                    Position = (Position.X + _vectorDirection[0], Position.Y + _vectorDirection[1]);
                    needChangeDir = false;

                    double distance = simulation.GetNearDistance(this);

                    if (distance < Config.RadiusSocDistOptim && distance < oldDistance)
                    {
                        Position = (oldX, oldY);
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
                if (simulation.CheckBarrier(Position.X + _vectorDirection[0], Position.Y + _vectorDirection[1]))
                {
                    double oldX = Position.X;
                    double oldY = Position.Y;

                    Position = (Position.X + _vectorDirection[0], Position.Y + _vectorDirection[1]);
                    needChangeDir = false;

                    double distance = simulation.GetNearDistance(this);

                    if (distance != -1 && distance < Config.RadiusHumanOptim)
                    {
                        Position = (oldX, oldY);
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
                if (simulation.CheckBarrier(Position.X + _vectorDirection[0], Position.Y + _vectorDirection[1]))
                {
                    double oldX = Position.X;
                    double oldY = Position.Y;


                    Position = (Position.X + _vectorDirection[0], Position.Y + _vectorDirection[1]);
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

        #endregion

        // TM - transmission mechanism (механизм передачи)

        // Произвести воздушно-капельный механизм передачи с ближайшими людьми
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

        
        private void ContactTM(Simulation simulation)
        {
            WashHands();
            Handshake(simulation);
            if (Condition == 0)
                TouchTheFace(simulation);
            if (Condition == HumanConditions.ProdromalInfected || 
                Condition == HumanConditions.ClinicallyInfected || 
                Condition == HumanConditions.AsymptomaticInfected)
                SetHandsDirty();
        }

        #region ForContactTM

        // Произвести контактный механизм мередачи с ближайшими людьми
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

        // Потрогать свое лицо руками
        private void TouchTheFace(Simulation simulation)
        {
            if (_timeHandToFaceContact == 0)
            {
                if (_infectHand)
                {
                    simulation.StTouchesTheFaceWithInfect++;
                    if (Config.GetPermissionInfContact())
                    {
                        Condition = HumanConditions.IncubatedInfected;
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

        // Испачкать руки инфекцией
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

        // Помыть руки
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

        #endregion

        // Течение болезни у человека
        private void CourseDisease()
        {
            switch (Condition)
            {
                case HumanConditions.IncubatedInfected:
                    if (_timeIncub == 0)
                    {
                        if (Config.GetPermissionAsymptomatic())
                            Condition = HumanConditions.AsymptomaticInfected;
                        else
                            Condition = HumanConditions.ProdromalInfected;
                    }
                    else
                        _timeIncub--;
                    break;

                case HumanConditions.ProdromalInfected:
                    if (_timeProdorm == 0)
                        Condition = HumanConditions.ClinicallyInfected;
                    else
                        _timeProdorm--;
                    break;

                case HumanConditions.ClinicallyInfected:
                case HumanConditions.AsymptomaticInfected:
                    if (_timeRecovery == 0)
                    {
                        if (Config.GetPermissionDie())
                            Condition = HumanConditions.Dead;
                        else
                            Condition = HumanConditions.Recovered;
                    }
                    else
                        _timeRecovery--;
                    break;
            }
        }


        #region [ Конструктор ]
        public Human(int condition, bool mask, bool socDist, double x, double y)
        {
            Condition = (HumanConditions)condition;
            Position = (x, y);
            
            Mask = mask; 
            SocDist = socDist;
            InfectHand = false;
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
        #endregion

        /// <summary>
        /// Объявление свойства из INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
