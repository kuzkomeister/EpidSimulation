using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_ChancesInfection : VMUC_ConfigBasic
    {
        public VMUC_ChancesInfection(VM_Config model) : base(model) { }

        #region [ Свойства VM ]

        public double V_ProbabilityInfContact
        {
            get => _model.V_ProbabilityInfContact;
            set
            {
                _model.V_ProbabilityInfContact = value;
                OnChanged();
            }
        }

        public double V_ProbabilityInfAirborne
        {
            get => _model.V_ProbabilityInfAirborne;
            set
            {
                _model.V_ProbabilityInfAirborne = value;
                OnChanged();
            }
        }

        #endregion

    }
}
