using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_Diseases : VM_BASIC
    {
        public VMUC_Diseases(VM_Config model)
        {
            _model = model;
        }

        private VM_Config _model;

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

        #endregion


        public delegate void ChangedHandler();
        public event ChangedHandler Changed;
        private void OnChanged()
        {
            Changed?.Invoke();
        }
    }
}
