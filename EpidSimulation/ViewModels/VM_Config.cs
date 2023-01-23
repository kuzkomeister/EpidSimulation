using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using EpidSimulation.Models;

namespace EpidSimulation.ViewModels
{
    public class VM_Config : VM_BASIC
    {
        public VM_Config(Config config)
        {
            Model = config; 
        }

        public Config Model { private set; get; }

        #region [ Свойства VM ]

        public string V_Name 
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeIncub_A 
        {
            get => Model.TimeIncub_A;
            set
            {
                Model.TimeIncub_A = value;
                OnPropertyChanged();
            } 
        }

        public int V_TimeIncub_B
        {
            get => Model.TimeIncub_B;
            set
            {
                Model.TimeIncub_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeProdorm_A 
        {
            get => Model.TimeProdorm_A;
            set
            {
                Model.TimeProdorm_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeProdorm_B 
        {
            get => Model.TimeProdorm_B;
            set
            {
                Model.TimeProdorm_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeRecovery_A
        {
            get => Model.TimeRecovery_A;
            set
            {
                Model.TimeRecovery_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeRecovery_B
        {
            get => Model.TimeRecovery_B;
            set
            {
                Model.TimeRecovery_B = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityDie 
        {
            get => Model.ProbabilityDie;
            set
            {
                Model.ProbabilityDie = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityAsymptomatic 
        {
            get => Model.ProbabilityAsymptomatic;
            set
            {
                Model.ProbabilityAsymptomatic = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusHuman 
        {
            get => Model.RadiusHuman;
            set
            {
                Model.RadiusHuman = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusAirborne 
        {
            get => Model.RadiusAirborne;
            set
            {
                Model.RadiusAirborne = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusContact 
        {
            get => Model.RadiusContact;
            set
            {
                Model.RadiusContact = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusSocDist 
        {
            get => Model.RadiusSocDist;
            set
            {
                Model.RadiusSocDist = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityInfContact 
        {
            get => Model.ProbabilityInfContact;
            set
            {
                Model.ProbabilityInfContact = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityInfAirborne 
        {
            get => Model.ProbabilityInfAirborne;
            set
            {
                Model.ProbabilityInfAirborne = value;
                OnPropertyChanged();
            }
        }

        public double V_MaskProtectionFor 
        {
            get => Model.MaskProtectionFor;
            set
            {
                Model.MaskProtectionFor = value;
                OnPropertyChanged();
            }
        }

        public double V_MaskProtectionFrom 
        {
            get => Model.MaskProtectionFrom;
            set
            {
                Model.MaskProtectionFrom = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeAirborne_A 
        {
            get => Model.TimeAirborne_A;
            set 
            { 
                Model.TimeAirborne_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeAirborne_B 
        {
            get => Model.TimeAirborne_B;
            set
            {
                Model.TimeAirborne_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeHandToFaceContact_A 
        {
            get => Model.TimeHandToFaceContact_A;
            set
            {
                Model.TimeHandToFaceContact_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeHandToFaceContact_B 
        {
            get => Model.TimeHandToFaceContact_B;
            set
            {
                Model.TimeHandToFaceContact_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeWash_A 
        {
            get => Model.TimeWash_A;
            set
            {
                Model.TimeWash_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeWash_B 
        {
            get => Model.TimeWash_B;
            set
            {
                Model.TimeWash_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeContact_A 
        {
            get => Model.TimeContact_A;
            set
            {
                Model.TimeContact_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeContact_B 
        {
            get => Model.TimeContact_B;
            set
            {
                Model.TimeContact_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeInfHand_A 
        {
            get => Model.TimeInfHand_A;
            set
            {
                Model.TimeInfHand_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeInfHand_B 
        {
            get => Model.TimeInfHand_B;
            set
            {
                Model.TimeInfHand_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeChangeDirect_A 
        {
            get => Model.TimeChangeDirect_A;
            set
            {
                Model.TimeChangeDirect_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeChangeDirect_B 
        {
            get => Model.TimeChangeDirect_B;
            set
            {
                Model.TimeChangeDirect_B = value;
                OnPropertyChanged();
            }
        }

        public double V_MaxDist 
        {
            get => Model.MaxDist;
            set
            {
                Model.MaxDist = value;
                OnPropertyChanged();
            }
        }

        public int V_MaxTryes 
        {
            get => Model.MaxTryes;
            set
            {
                Model.MaxTryes = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
