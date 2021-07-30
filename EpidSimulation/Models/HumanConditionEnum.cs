namespace EpidSimulation.Models
{
   public enum HumanConditions
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
