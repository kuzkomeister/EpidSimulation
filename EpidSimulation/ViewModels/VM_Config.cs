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
            _model = config; 
        }

        private Config _model;

        #region [ Свойства VM ]

        public string V_Name 
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeIncub_A 
        {
            get => _model.TimeIncub_A;
            set
            {
                _model.TimeIncub_A = value;
                OnPropertyChanged();
            } 
        }

        public int V_TimeIncub_B
        {
            get => _model.TimeIncub_B;
            set
            {
                _model.TimeIncub_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeProdorm_A 
        {
            get => _model.TimeProdorm_A;
            set
            {
                _model.TimeProdorm_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeProdorm_B 
        {
            get => _model.TimeProdorm_B;
            set
            {
                _model.TimeProdorm_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeRecovery_A
        {
            get => _model.TimeRecovery_A;
            set
            {
                _model.TimeRecovery_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeRecovery_B
        {
            get => _model.TimeRecovery_B;
            set
            {
                _model.TimeRecovery_B = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityDie 
        {
            get => _model.ProbabilityDie;
            set
            {
                _model.ProbabilityDie = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityAsymptomatic 
        {
            get => _model.ProbabilityAsymptomatic;
            set
            {
                _model.ProbabilityAsymptomatic = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusHuman 
        {
            get => _model.RadiusHuman;
            set
            {
                _model.RadiusHuman = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusAirborne 
        {
            get => _model.RadiusAirborne;
            set
            {
                _model.RadiusAirborne = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusContact 
        {
            get => _model.RadiusContact;
            set
            {
                _model.RadiusContact = value;
                OnPropertyChanged();
            }
        }

        public double V_RadiusSocDist 
        {
            get => _model.RadiusSocDist;
            set
            {
                _model.RadiusSocDist = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityInfContact 
        {
            get => _model.ProbabilityInfContact;
            set
            {
                _model.ProbabilityInfContact = value;
                OnPropertyChanged();
            }
        }

        public double V_ProbabilityInfAirborne 
        {
            get => _model.ProbabilityInfAirborne;
            set
            {
                _model.ProbabilityInfAirborne = value;
                OnPropertyChanged();
            }
        }

        public double V_MaskProtectionFor 
        {
            get => _model.MaskProtectionFor;
            set
            {
                _model.MaskProtectionFor = value;
                OnPropertyChanged();
            }
        }

        public double V_MaskProtectionFrom 
        {
            get => _model.MaskProtectionFrom;
            set
            {
                _model.MaskProtectionFrom = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeAirborne_A 
        {
            get => _model.TimeAirborne_A;
            set 
            { 
                _model.TimeAirborne_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeAirborne_B 
        {
            get => _model.TimeAirborne_B;
            set
            {
                _model.TimeAirborne_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeHandToFaceContact_A 
        {
            get => _model.TimeHandToFaceContact_A;
            set
            {
                _model.TimeHandToFaceContact_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeHandToFaceContact_B 
        {
            get => _model.TimeHandToFaceContact_B;
            set
            {
                _model.TimeHandToFaceContact_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeWash_A 
        {
            get => _model.TimeWash_A;
            set
            {
                _model.TimeWash_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeWash_B 
        {
            get => _model.TimeWash_B;
            set
            {
                _model.TimeWash_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeContact_A 
        {
            get => _model.TimeContact_A;
            set
            {
                _model.TimeContact_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeContact_B 
        {
            get => _model.TimeContact_B;
            set
            {
                _model.TimeContact_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeInfHand_A 
        {
            get => _model.TimeInfHand_A;
            set
            {
                _model.TimeInfHand_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeInfHand_B 
        {
            get => _model.TimeInfHand_B;
            set
            {
                _model.TimeInfHand_B = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeChangeDirect_A 
        {
            get => _model.TimeChangeDirect_A;
            set
            {
                _model.TimeChangeDirect_A = value;
                OnPropertyChanged();
            }
        }

        public int V_TimeChangeDirect_B 
        {
            get => _model.TimeChangeDirect_B;
            set
            {
                _model.TimeChangeDirect_B = value;
                OnPropertyChanged();
            }
        }

        public double V_MaxDist 
        {
            get => _model.MaxDist;
            set
            {
                _model.MaxDist = value;
                OnPropertyChanged();
            }
        }

        public int V_MaxTryes 
        {
            get => _model.MaxTryes;
            set
            {
                _model.MaxTryes = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
