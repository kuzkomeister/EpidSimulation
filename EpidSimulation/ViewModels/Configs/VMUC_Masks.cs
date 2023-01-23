using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_Masks : VMUC_ConfigBasic
    {
        public VMUC_Masks(VM_Config model) : base(model) { }

        #region [ Свойства VM ]

        /// <summary>
        /// Заразить
        /// </summary>
        public double V_MaskProtectionFor
        {
            get => _model.V_MaskProtectionFor;
            set
            {
                _model.V_MaskProtectionFor = value;
                OnChanged();
                _UpdateChances();
            }
        }

        /// <summary>
        /// Заразиться
        /// </summary>
        public double V_MaskProtectionFrom
        {
            get => _model.V_MaskProtectionFrom;
            set
            {
                _model.V_MaskProtectionFrom = value;
                OnChanged();
                _UpdateChances();
            }
        }

        public double V_InfInf { get => _model.V_ProbabilityInfAirborne; }

        public double V_InfMask { get => _model.V_ProbabilityInfAirborne * (1 - V_MaskProtectionFrom); }

        public double V_MaskInf { get => _model.V_ProbabilityInfAirborne * (1 - V_MaskProtectionFor); }

        public double V_MaskMask { get => _model.V_ProbabilityInfAirborne * (1 - V_MaskProtectionFor) * (1 - V_MaskProtectionFrom); }

        #endregion

        private void _UpdateChances()
        {
            OnPropertyChanged(nameof(V_InfInf));
            OnPropertyChanged(nameof(V_InfMask));
            OnPropertyChanged(nameof(V_MaskInf));
            OnPropertyChanged(nameof(V_MaskMask));
        }
    }
}
