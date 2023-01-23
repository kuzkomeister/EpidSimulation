using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_AreasInteraction : VMUC_ConfigBasic
    {
        public VMUC_AreasInteraction(VM_Config model) : base(model) { }

        #region [ Свойства VM ]

        public double V_RadiusHuman
        {
            get => _model.V_RadiusHuman;
            set
            {
                _model.V_RadiusHuman = value;
                _UpdateRadiuses();
                _UpdateZIndexes();
                OnChanged();
            }
        }

        public double V_RadiusAirborne
        {
            get => _model.V_RadiusAirborne;
            set
            {
                _model.V_RadiusAirborne = value;
                _UpdateRadiuses();
                _UpdateZIndexes();
                OnChanged();
            }
        }

        public double V_RadiusContact
        {
            get => _model.V_RadiusContact;
            set
            {
                _model.V_RadiusContact = value;
                _UpdateRadiuses();
                _UpdateZIndexes();
                OnChanged();
            }
        }

        public double V_RadiusSocDist
        {
            get => _model.V_RadiusSocDist;
            set
            {
                _model.V_RadiusSocDist = value;
                _UpdateRadiuses();
                _UpdateZIndexes();
                OnChanged();
            }
        }

        public int V_ZIndexAirBorne { get => _GetZIndex(V_RadiusAirborne); }
        public int V_ZIndexContact { get => _GetZIndex(V_RadiusContact); }
        public int V_ZIndexSocDist { get => _GetZIndex(V_RadiusSocDist); }

        private void _UpdateZIndexes()
        {
            OnPropertyChanged(nameof(V_ZIndexAirBorne));
            OnPropertyChanged(nameof(V_ZIndexContact));
            OnPropertyChanged(nameof(V_ZIndexSocDist));
        }

        private void _UpdateRadiuses()
        {
            OnPropertyChanged(nameof(V_RadiusHuman));
            OnPropertyChanged(nameof(V_RadiusContact));
            OnPropertyChanged(nameof(V_RadiusAirborne));
            OnPropertyChanged(nameof(V_RadiusSocDist));
        }

        private int _GetZIndex(double value)
        {
            int res = 0;
            if(value < V_RadiusAirborne) 
            {
                res++;
            }
            if(value < V_RadiusContact)
            {
                res++;
            }
            if(value < V_RadiusSocDist)
            {
                res++;
            }
            return res;
        }

        #endregion
    }
}
