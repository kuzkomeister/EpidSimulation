using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_Diseases : VMUC_ConfigBasic
    {
        public VMUC_Diseases(VM_Config model) : base(model) { }

        #region [ Свойства VM ]

        public string V_Name 
        {
            get => _model.V_Name;
            set
            {
                _model.V_Name = value;
                OnChanged();
            }
        }

        public double V_ProbabilityDie 
        {
            get => _model.V_ProbabilityDie;
            set
            {
                _model.V_ProbabilityDie = value;
                OnChanged();
            }
        }

        public double V_ProbabilityAsymptomatic
        {
            get => _model.V_ProbabilityAsymptomatic;
            set
            {
                _model.V_ProbabilityAsymptomatic = value;
                OnChanged();
            }
        }

        public int V_TimeIncub_A
        {
            get => _model.V_TimeIncub_A;
            set
            {
                _model.V_TimeIncub_A = value;
                OnChanged();
            }
        }

        public int V_TimeIncub_B
        {
            get => _model.V_TimeIncub_B;
            set
            {
                _model.V_TimeIncub_B = value;
                OnChanged();
            }
        }

        public int V_TimeProdorm_A
        {
            get => _model.V_TimeProdorm_A;
            set
            {
                _model.V_TimeProdorm_A = value;
                OnChanged();
            }
        }

        public int V_TimeProdorm_B
        {
            get => _model.V_TimeProdorm_B;
            set
            {
                _model.V_TimeProdorm_B = value;
                OnChanged();
            }
        }

        public int V_TimeRecovery_A
        {
            get => _model.V_TimeRecovery_A;
            set
            {
                _model.V_TimeRecovery_A = value;
                OnChanged();
            }
        }

        public int V_TimeRecovery_B
        {
            get => _model.V_TimeRecovery_B;
            set
            {
                _model.V_TimeRecovery_B = value;
                OnChanged();
            }
        }

        #endregion
    }
}
