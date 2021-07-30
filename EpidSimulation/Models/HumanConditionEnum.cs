using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.Models
{
   public enum HumanCondition
    {
        Healthy,                // Здоровый
        IncubatedInfected,      // Инфицированный в инкубационном периоде
        ProdromalInfected,      // Инфицированный в продромальном периоде
        ClinicallyInfected,     // Инфицированный в клиническом   периоде
        Recovered,              // Выздоровевший
        AsymptomaticInfected,   // Бессимптомный
        Dead                    // Мертвый
    }
}
