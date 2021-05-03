using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.Backend
{
    public class ConfigDisease
    {
        private Random random = new Random(322);

        //===== Основные характеристики болезни

        // Инкубационный период
        private int _timeIncub_a, _timeIncub_b;       
        public int TimeIncub_A {
            set => _timeIncub_a = value <= TimeIncub_B ? value: _timeIncub_a;
            get => _timeIncub_a; 
        }
        public int TimeIncub_B
        {
            set => _timeIncub_b = value >= TimeIncub_A ? value : _timeIncub_b;
            get => _timeIncub_b;
        }

        // Продормальный период
        private int _timeProdorm_a, _timeProdorm_b;   
        public int TimeProdorm_A
        {
            set => _timeProdorm_a = value <= TimeProdorm_B ? value : _timeProdorm_a;
            get => _timeProdorm_a;
        }
        public int TimeProdorm_B
        {
            set => _timeProdorm_b = value >= TimeProdorm_A ? value : _timeProdorm_b;
            get => _timeProdorm_b;
        }

        // Клинический период
        private int _timeRecovery_a, _timeRecovery_b; 
        public int TimeRecovery_A
        {
            set => _timeRecovery_a = value <= TimeRecovery_B ? value : _timeRecovery_a;
            get => _timeRecovery_a;
        }
        public int TimeRecovery_B
        {
            set => _timeRecovery_b = value >= TimeRecovery_A ? value : _timeRecovery_b;
            get => _timeRecovery_b;
        }

        // Летальность болезни
        private double _probabilityDie;               
        public double ProbabilityDie
        {
            set => _probabilityDie = 0 <= value && value <= 1 ? value : _probabilityDie; 
            get => _probabilityDie;
        }

        // Процент бессимптомности болезни
        private double _probabilityAsymptomatic;
        public double ProbabilityAsymptomatic
        {
            set => _probabilityAsymptomatic = 0 <= value && value <= 1 ? value : _probabilityAsymptomatic;
            get => _probabilityAsymptomatic;
        }

        //===== Уровень взаимодействия

        //=== Радиус размера человека
        private double _radiusHuman;           
        public double RadiusHuman
        {
            set
            {
                if (value > 0)
                {
                    _radiusHuman = value;
                    _radiusHumanOptim = (double)(4.1 * Math.Pow(RadiusHuman, 2));
                }
            }
            get => _radiusHuman;
        }
        // Радиус размера человека (в квадрате)
        private double _radiusHumanOptim;      
        public double RadiusHumanOptim { get => _radiusHumanOptim; }

        //=== Радиус воздушно-капельного механизма передачи
        private double _radiusAirborne;
        public double RadiusAirborne
        {
            set
            {
                if (value >= 0)
                {
                    _radiusAirborne = value + RadiusHuman;
                    _radiusAirborneOptim = (double)(4.1 * Math.Pow(RadiusAirborne, 2));
                }
            }

            get => _radiusAirborne;     
        }
        // Радиус воздушно-капельного механизма передачи (в квадрате)
        private double _radiusAirborneOptim;
        public double RadiusAirborneOptim { get => _radiusAirborneOptim; }

        //=== Радиус контактного механизма передачи
        private double _radiusContact;
        public double RadiusContact
        {
            set
            {
                if (value >= 0)
                {
                    _radiusContact = value + RadiusHuman;
                    _radiusContactOptim = (double)(4.1 * Math.Pow(RadiusContact, 2));
                }
            }
            get => _radiusContact;
        }
        // Радиус контактного механизма передачи (в квадрате)
        private double _radiusContactOptim;
        public double RadiusContactOptim { get => _radiusContactOptim; }

        //=== Радиус социальной дистанции
        private double _radiusSocDist;
        public double RadiusSocDist
        {
            set
            {
                if (value >= 0)
                {
                    _radiusSocDist = value + RadiusHuman;
                    _radiusSocDistOptim = (double)(4.1 * Math.Pow(RadiusSocDist, 2));
                }
            }
            get => _radiusSocDist;
        }
        // Радиус социальной дистанции (в квадрате)
        private double _radiusSocDistOptim;      
        public double RadiusSocDistOptim { get => _radiusSocDistOptim; }



        //===== Уровень распространения

        // Шанс заразиться от контактного механизма передачи 
        private double _probabilityInfContact;  
        public double ProbabilityInfContact
        {
            set => _probabilityInfContact = 0 <= value && value <= 1 ? value : _probabilityInfContact;
            get => _probabilityInfContact;
        }

        // Шанс заразиться от воздушно-капельного механизма передачи
        private double _probabilityInfAirborne;  
        public double ProbabilityInfAirborne
        {
            set => _probabilityInfAirborne = 0 <= value && value <= 1 ? value : _probabilityInfAirborne;
            get => _probabilityInfAirborne;
        }
        //private double _maskProtectionFor;   // Эффективность защиты маски в сторону заразить кого-то
        //private double _maskProtectionFrom;  // Эффективность защиты маски в сторону заразиться от кого-то
        
        // Шанс заразить восприимчивый организм от источника инфекции (зависит от степени защиты индивидуальной защиты)
        private double _probabilityInfFor;
        public double ProbabilityInfFor
        {
            set => _probabilityInfFor = 0 <= value && value <= 1 ? value : _probabilityInfFor;
            get => _probabilityInfFor;
        }
        
        // Шанс заразиться восприимчивому организму от источника инфекции (зависит от степени защиты индивидуальной защиты)
        private double _probabilityInfFrom;
        public double ProbabilityInfFrom
        {
            set => _probabilityInfFrom = 0 <= value && value <= 1 ? value : _probabilityInfFrom;
            get => _probabilityInfFrom;
        }

        //===== Уровень заболеваемости

        // Время между контактами воздушно-капельного механизма передачи
        private int _timeAirborne_a, _timeAirborne_b;                           
        public int TimeAirborne_A
        { 
            set => _timeAirborne_a = value <= TimeAirborne_B ? value : _timeAirborne_a;
            get => _timeAirborne_a;
        }
        public int TimeAirborne_B
        {
            set => _timeAirborne_b = value >= TimeAirborne_A ? value : _timeAirborne_b;
            get => _timeAirborne_b;
        }
        
        // Время между контактом рук с лицом
        private int _timeHandToFaceContact_a, _timeHandToFaceContact_b; 
        public int TimeHandToFaceContact_A
        {  
            set => _timeHandToFaceContact_a = value <= TimeHandToFaceContact_B? value : _timeHandToFaceContact_a;
            get => _timeHandToFaceContact_a;
        }
        public int TimeHandToFaceContact_B
        {
            set => _timeHandToFaceContact_b = value >= TimeHandToFaceContact_A ? value : _timeHandToFaceContact_b;
            get => _timeHandToFaceContact_b;
        }
    
        // Время между мытьем рук
        private int _timeWash_a, _timeWash_b;                           
        public int TimeWash_A
        {
            set => _timeWash_a = value <= TimeWash_B ? value : _timeWash_a;
            get => _timeWash_a;
        }
        public int TimeWash_B
        {
            set => _timeWash_b = value >= TimeWash_A ? value : _timeWash_b;
            get => _timeWash_b;
        }
        
        // Время между контактами контактного механизма передачи
        private int _timeContact_a, _timeContact_b;                 
        public int TimeContact_A
        {
            set => _timeContact_a = value <= TimeContact_B ? value : _timeContact_a;
            get => _timeContact_a;
        }
        public int TimeContact_B
        {
            set => _timeContact_b = value >= TimeContact_A ? value : _timeContact_b;
            get => _timeContact_b;
        }

        // Время между загрязнением рук
        private int _timeInfHand_a, _timeInfHand_b;                     
        public int TimeInfHand_A
        {   
            set => _timeInfHand_a = value <= TimeInfHand_B ? value : _timeInfHand_a;
            get => _timeInfHand_a;
        }
        public int TimeInfHand_B
        {
            set => _timeInfHand_b = value >= TimeInfHand_A ? value : _timeInfHand_b;
            get => _timeInfHand_b;
        }


        //====== Для передвижения

        // Время смены направления движения
        private int _timeChangeDirect_a, _timeChangeDirect_b;           
        public int TimeChangeDirect_A
        {   
            set => _timeChangeDirect_a = value <= TimeChangeDirect_B ? value : _timeChangeDirect_a;
            get => _timeChangeDirect_a;
        }
        public int TimeChangeDirect_B
        {
            set => _timeChangeDirect_b = value >= TimeChangeDirect_A ? value : _timeChangeDirect_b;
            get => _timeChangeDirect_b;
        }

        // Максимальная дистанция шага человека
        private double _maxDist; 
        public double MaxDist
        {
            set => _maxDist = value > 0 ? value : _maxDist;
            get => _maxDist;
        }

        // Максимальное количество попыток перемещения в итерацию
        private int _maxTryes;   
        public int MaxTryes
        {
            set => _maxTryes = value > 1 ? value : _maxTryes;
            get => _maxTryes;
        }

        //====================================

        public ConfigDisease()
        {
            TimeIncub_B = 500; TimeIncub_A = 400;
            TimeProdorm_B = 200; TimeProdorm_A = 100;
            TimeRecovery_B = 900; TimeRecovery_A = 800;
            ProbabilityDie = 0.1; ProbabilityAsymptomatic = 0.2;

            RadiusHuman = 1.0; RadiusSocDist = 2.0;
            RadiusAirborne = 1.5; RadiusContact = 1.0;

            ProbabilityInfContact = 0.5; ProbabilityInfAirborne = 0.8;
            ProbabilityInfFor = 0.1; ProbabilityInfFrom = 0.9;

            TimeAirborne_B = 120; TimeAirborne_A = 100;
            TimeContact_B = 100; TimeContact_A = 50;
            TimeHandToFaceContact_B = 160; TimeHandToFaceContact_A = 140;
            TimeWash_B = 300; TimeWash_A = 200;
            TimeInfHand_B = 100; TimeInfHand_A = 50;

            MaxDist = 0.3;  MaxTryes = 3;
            TimeChangeDirect_B = 100; TimeChangeDirect_A = 50;
        }

        //===== Вероятность
        public bool GetPermissionInfContact()
        {
            return random.NextDouble() <= ProbabilityInfContact;
        }

        public bool GetPermissionDie()
        {
            return random.NextDouble() <= ProbabilityDie;
        }

        public bool GetPermissionInfect(bool maskFrom, bool maskFor)
        {
            if (maskFor)
            {
                if (maskFrom)
                    return random.NextDouble() < ProbabilityInfAirborne * ProbabilityInfFor * ProbabilityInfFrom;
                else
                    return random.NextDouble() < ProbabilityInfAirborne * ProbabilityInfFrom;
                
            }
            else
            {
                if (maskFrom)
                    return random.NextDouble() < ProbabilityInfAirborne * _probabilityInfFor;
                else
                    return random.NextDouble() < ProbabilityInfAirborne;                
            }
        }

        public bool GetPermissionAsymptomatic()
        {
            return random.NextDouble() <= ProbabilityAsymptomatic;
        }

        //===== Получить случайное время для таймера
        public int GetTimeMeet()
        {
            return random.Next(TimeAirborne_A, TimeAirborne_B);
        }

        public int GetTimeHandToFaceContact()
        {
            return random.Next(TimeHandToFaceContact_A, TimeHandToFaceContact_B);
        }

        public int GetTimeWash()
        {
            return random.Next(TimeWash_A, TimeWash_B);
        }

        public int GetTimeChangeDirect()
        {
            return random.Next(TimeChangeDirect_A, TimeChangeDirect_B);
        }

        public int GetTimeContact()
        {
            return random.Next(TimeContact_A, TimeContact_B);
        }

        public int GetTimeInfHand()
        {
            return random.Next(TimeInfHand_A, TimeInfHand_B);
        }

        public int GetTimeIncub()
        {
            return random.Next(TimeIncub_A, TimeIncub_B);
        }

        public int GetTimeProdorm()
        {
            return random.Next(TimeProdorm_A, TimeProdorm_B);
        }

        public int GetTimeRecovery()
        {
            return random.Next(TimeRecovery_A, TimeRecovery_B);
        }

        //==== Поменять направление движения

        public double GetDirection()
        {
            return random.NextDouble() * (2 * MaxDist) - MaxDist;
        }

    }
}
