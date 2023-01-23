using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_SocialActs : VMUC_ConfigBasic
    {
        public VMUC_SocialActs(VM_Config model) : base(model) { }

        #region [ Свойства VM ]

        public int V_TimeAirborne_A
        {
            get => _model.V_TimeAirborne_A;
            set
            {
                _model.V_TimeAirborne_A = value;
                OnChanged();
            }
        }

        public int V_TimeAirborne_B
        {
            get => _model.V_TimeAirborne_B;
            set
            {
                _model.V_TimeAirborne_B = value;
                OnChanged();
            }
        }

        public int V_TimeContact_A
        {
            get => _model.V_TimeContact_A;
            set
            {
                _model.V_TimeContact_A = value;
                OnChanged();
            }
        }

        public int V_TimeContact_B
        {
            get => _model.V_TimeContact_B;
            set
            {
                _model.V_TimeContact_B = value;
                OnChanged();
            }
        }

        public int V_TimeWash_A
        {
            get => _model.V_TimeWash_A;
            set
            {
                _model.V_TimeWash_A = value;
                OnChanged();
            }
        }

        public int V_TimeWash_B
        {
            get => _model.V_TimeWash_B;
            set
            {
                _model.V_TimeWash_B = value;
                OnChanged();
            }
        }

        public int V_TimeInfHand_A
        {
            get => _model.V_TimeInfHand_A;
            set
            {
                _model.V_TimeInfHand_A = value;
                OnChanged();
            }
        }

        public int V_TimeInfHand_B
        {
            get => _model.V_TimeInfHand_B;
            set
            {
                _model.V_TimeInfHand_B = value;
                OnChanged();
            }
        }

        public int V_TimeHandToFaceContact_A
        {
            get => _model.V_TimeHandToFaceContact_A;
            set
            {
                _model.V_TimeHandToFaceContact_A = value;
                OnChanged();
            }
        }

        public int V_TimeHandToFaceContact_B
        {
            get => _model.V_TimeHandToFaceContact_B;
            set
            {
                _model.V_TimeHandToFaceContact_B = value;
                OnChanged();
            }
        }

        #endregion

    }
}
