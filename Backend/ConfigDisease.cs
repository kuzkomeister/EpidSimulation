using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.Backend
{
    public class ConfigDisease
    {
        private Random random;

        //===== Основные характеристики болезни
        private int _timeLatent_a, _timeLatent_b;       // Латентный период
        private int _timeIncub_a, _timeIncub_b;         // Инкубационный период
        private int _timeRecovery_a, _timeRecovery_b;   // Клинический период
        private double _probabilityDie;                // Летальность болезни

        //===== Уровень взаимодействия
        public readonly double RadiusMeet;          // Радиус Заражения
        public readonly double RadiusMeetOptim;     // Радиус Заражения (в квадрате)
        public readonly double RadiusMan;           // Радиус размера человека
        public readonly double RadiusManOptim;      // Радиус размера человека (в квадрате)
        public readonly double RadiusSoc;           // Радиус социальной дистанции
        public readonly double RadiusSocOptim;      // Радиус социальной дистанции (в квадрате)
        public readonly double RadiusHandshake;     // Радиус возможного рукопожатия
        public readonly double RadiusHandshakeOptim;// Радиус возможного рукопожатия (в квадрате)
        private double _probabilitySelfIsolation;   // Вероятность, что человек уйдет на самоизоляцию

        //===== Уровень распространения
        private double _probabilityInfHand;  // Заразиться от контакта рук с лицом
        private double _probabilityInfMeet;  // Заразиться на встрече
        //private double _maskProtectionFor;   // Эффективность защиты маски в сторону заразить кого-то
        //private double _maskProtectionFrom;  // Эффективность защиты маски в сторону заразиться от кого-то
        private double _probabilityInfFor;
        private double _probabilityInfFrom;

        //===== Уровень заболеваемости
        private int _timeMeet_a, _timeMeet_b;                           // Время между встречами 
        private int _timeHandToFaceContact_a, _timeHandToFaceContact_b; // Время между контактом рук с лицом
        private int _timeWash_a, _timeWash_b;                           // Время между мытьем рук
        private int _timeHandshake_a, _timeHandshake_b;                 // Время между рукопожатиями
        private int _timeInfHand_a, _timeInfHand_b;                     // Время между загрязнением рук


        // Для передвижения
        private int _timeChangeDirect_a, _timeChangeDirect_b;           // Время смены направления движения
        public readonly double MaxDist; // Максимальная дистанция шага человека
        public readonly int MaxTryes;   // Максимальное количество попыток перемещения в итерацию

        //====================================

        public ConfigDisease(
            // Основные характеристики болезни
            int timeLatent_a, int timeLatent_b,
            int timeIncub_a, int timeIncub_b,
            int timeRecovery_a, int timeRecovery_b,
            double probabilityDied,
            // Уровень взаимодействия
            double radiusMan, 
            double radiusSoc, 
            double radiusMeet, 
            double radiusHandshake,
            // Уровень распространения
            double probabilityInfHand,
            double probabilityInfMeet,
            double maskProtectionFor, 
            double maskProtectionFrom,
            // Уровень заболеваемости
            int timeMeet_a, int timeMeet_b,
            int timeHandToFaceContact_a, int timeHandToFaceContact_b,
            int timeWash_a, int timeWash_b,
            int timeHandshake_a, int timeHandshake_b,
            int timeInfHand_a, int timeInfHand_b
            )
        {
            // Настройки времени
            _timeMeet_a = timeMeet_a; _timeMeet_b = timeMeet_b;
            _timeHandToFaceContact_a = timeHandToFaceContact_a; _timeHandToFaceContact_b = timeHandToFaceContact_b;
            _timeWash_a = timeWash_a; _timeWash_b = timeWash_b;
            _timeLatent_a = timeLatent_a; _timeLatent_b = timeLatent_b;
            _timeIncub_a = timeIncub_a; _timeIncub_b = timeIncub_b;
            _timeRecovery_a = timeRecovery_a; _timeRecovery_b = timeRecovery_b;
            _timeHandshake_a = timeHandshake_a; _timeHandshake_b = timeHandshake_b;
            _timeInfHand_a = timeInfHand_a; _timeInfHand_b = timeInfHand_b;
            // Вероятности
            this._probabilityDie = probabilityDied;
            this._probabilityInfMeet = probabilityInfMeet;
            this._probabilityInfHand = probabilityInfHand;
            //this._maskProtectionFor = maskProtectionFor;
            this._probabilityInfFor = 1 - maskProtectionFor;
            //this._maskProtectionFrom = maskProtectionFrom;
            this._probabilityInfFrom = 1 - maskProtectionFrom;
            // Радиусы
            this.RadiusMan = radiusMan;
            RadiusManOptim = (double)(4.2 * Math.Pow(this.RadiusMan, 2));
            this.RadiusSoc = radiusSoc + radiusMan;
            RadiusSocOptim = (double)(4.1 * Math.Pow(this.RadiusSoc, 2));
            this.RadiusMeet = radiusMeet + radiusMan;
            RadiusMeetOptim = (double)Math.Pow(this.RadiusMeet, 2);
            this.RadiusHandshake = radiusHandshake + radiusMan;
            RadiusHandshakeOptim = (double)Math.Pow(this.RadiusHandshake, 2);

            this.MaxDist = 0.3;
            this.MaxTryes = 3;
            _timeChangeDirect_a = 50; _timeChangeDirect_b = 100;

            random = new Random(333);
        }

        //===== Вероятность
        public bool GetPermissionInfHand()
        {
            return random.NextDouble() <= _probabilityInfHand;
        }

        public bool GetPermissionDie()
        {
            return random.NextDouble() <= _probabilityDie;
        }

        public bool GetPermissionInfect(bool maskFrom, bool maskFor)
        {
            if (maskFor)
            {
                if (maskFrom)
                    return random.NextDouble() < _probabilityInfMeet * _probabilityInfFor * _probabilityInfFrom;
                else
                    return random.NextDouble() < _probabilityInfMeet * _probabilityInfFrom;
                
            }
            else
            {
                if (maskFrom)
                    return random.NextDouble() < _probabilityInfMeet * _probabilityInfFor;
                else
                    return random.NextDouble() < _probabilityInfMeet;                
            }
        }

        //===== Получить случайное время для таймера
        public int GetTimeMeet()
        {
            return random.Next(_timeMeet_a, _timeMeet_b);
        }

        public int GetTimeHandToFaceContact()
        {
            return random.Next(_timeHandToFaceContact_a, _timeHandToFaceContact_b);
        }

        public int GetTimeWash()
        {
            return random.Next(_timeWash_a, _timeWash_b);
        }

        public int GetTimeChangeDirect()
        {
            return random.Next(_timeChangeDirect_a, _timeChangeDirect_b);
        }

        public int GetTimeHandshake()
        {
            return random.Next(_timeHandshake_a, _timeHandshake_b);
        }

        public int GetTimeInfHand()
        {
            return random.Next(_timeInfHand_a, _timeInfHand_b);
        }

        public int GetTimeLatent()
        {
            return random.Next(_timeLatent_a, _timeLatent_b);
        }

        public int GetTimeIncub()
        {
            return random.Next(_timeIncub_a, _timeIncub_b);
        }

        public int GetTimeRecovery()
        {
            return random.Next(_timeRecovery_a, _timeRecovery_b);
        }

        public double GetDirection()
        {
            return random.NextDouble() * (2 * MaxDist) - MaxDist;
        }

    }
}
