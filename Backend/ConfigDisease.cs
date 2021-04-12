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

        //===== Интервалы времени
        private int _timeMeet_a, _timeMeet_b;
        private int _timeHandToFaceContact_a, _timeHandToFaceContact_b;
        private int _timeWash_a, _timeWash_b;
        private int _timeChangeDirect_a, _timeChangeDirect_b;
        private int _timeHandshake_a, _timeHandshake_b;
        private int _timeInfHand_a, _timeInfHand_b;
        private int _timeInfInc_a, _timeInfInc_b;
        private int _timeRecovery_a, _timeRecovery_b;
        
        //===== Вероятности
        private double _probabilityNotSymp;  // Стать бессимптомным
        private double _probabilityDied;     // Умереть
        private double _probabilityInfHand;  // Занести вирус от контакта рук с лицом
        // Эффективность маски
        private double _maskProtectionFor;   // Защиты в сторону заразить кого-то
        private double _maskProtectionFrom;  // Защиты в сторону заразиться от кого-то

        //===== Радиусы
        public readonly double RadiusMeet;           // Радиус Заражения
        public readonly double RadiusMeetOptim;      // Радиус Заражения (в квадрате)
        public readonly double RadiusMan;           // Радиус размера человека
        public readonly double RadiusManOptim;      // Радиус размера человека (в квадрате)
        public readonly double RadiusSoc;           // Радиус социальной дистанции
        public readonly double RadiusSocOptim;      // Радиус социальной дистанции (в квадрате)
        public readonly double RadiusHandshake;     // Радиус возможного рукопожатия
        public readonly double RadiusHandshakeOptim;// Радиус возможного рукопожатия (в квадрате)

        public readonly double MaxDist; // Максимальная дистанция шага человека
        public readonly int MaxTryes;   // Максимальное количество попыток перемещения в итерацию

        //====================================

        public ConfigDisease(
            // Интервалы времени
            int timeMeet_a, int timeMeet_b,
            int timeHandToFaceContact_a, int timeHandToFaceContact_b,
            int timeWash_a, int timeWash_b,
            int timeChangeDirect_a, int timeChangeDirect_b,
            int timeHandshake_a, int timeHandshake_b,
            int timeInfHand_a, int timeInfHand_b,
            int timeInfInc_a, int timeInfInc_b,
            int timeRecovery_a, int timeRecovery_b,
            // Вероятность
            float probabilityNotSymp, float probabilityDied,
            float probabilityInfHand,
            // Эффективность маски
            float maskProtectionFor, float maskProtectionFrom,
            // Радиусы
            float radiusSoc, float radiusMan, float radiusMeet, float radiusHandshake,
            
            float maxDist
            )
        {
            // Настройки времени
            _timeMeet_a = timeMeet_a; _timeMeet_b = timeMeet_b;
            _timeHandToFaceContact_a = timeHandToFaceContact_a; _timeHandToFaceContact_b = timeHandToFaceContact_b;
            _timeWash_a = timeWash_a; _timeWash_b = timeWash_b;
            _timeInfInc_a = timeInfInc_a; _timeInfInc_b = timeInfInc_b;
            _timeChangeDirect_a = timeChangeDirect_a; _timeChangeDirect_b = timeChangeDirect_b;
            _timeRecovery_a = timeRecovery_a; _timeRecovery_b = timeRecovery_b;
            _timeHandshake_a = timeHandshake_a; _timeHandshake_b = timeHandshake_b;
            _timeInfHand_a = timeInfHand_a; _timeInfHand_b = timeInfHand_b;
            // Вероятности
            this._probabilityDied = probabilityDied;
            this._probabilityNotSymp = probabilityNotSymp;
            this._probabilityInfHand = probabilityInfHand;
            this._maskProtectionFor = maskProtectionFor;
            this._maskProtectionFrom = maskProtectionFrom;
            // Радиусы
            this.RadiusMan = radiusMan;
            RadiusManOptim = (double)(4.2 * Math.Pow(this.RadiusMan, 2));
            this.RadiusSoc = radiusSoc + radiusMan;
            RadiusSocOptim = (double)(4.1 * Math.Pow(this.RadiusSoc, 2));
            this.RadiusMeet = radiusMeet + radiusMan;
            RadiusMeetOptim = (double)Math.Pow(this.RadiusMeet, 2);
            this.RadiusHandshake = radiusHandshake + radiusMan;
            RadiusHandshakeOptim = (double)Math.Pow(this.RadiusHandshake, 2);

            this.MaxDist = maxDist;
            this.MaxTryes = 3;

            random = new Random(333);
        }

        //===== Вероятность
        public bool GetPermissionInfHand()
        {
            return random.NextDouble() <= _probabilityInfHand;
        }

        public bool GetPermissionNotSymp()
        {
            return random.NextDouble() <= _probabilityNotSymp;
        }

        public bool GetPermissionDied()
        {
            return random.NextDouble() <= _probabilityDied;
        }

        public bool GetPermissionInfect(bool maskFrom, bool maskFor)
        {
            if (maskFor)
            {
                if (maskFrom)
                    return random.NextDouble() > _maskProtectionFor * _maskProtectionFrom;
                else
                    return random.NextDouble() > _maskProtectionFor;
                
            }
            else
            {
                if (maskFrom)
                    return random.NextDouble() > _maskProtectionFrom;
                else
                    return true;                
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

        public int GetTimeInfInc()
        {
            return random.Next(_timeInfInc_a, _timeInfInc_b);
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
