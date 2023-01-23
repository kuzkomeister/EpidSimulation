using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_ConfigBasic : VM_BASIC
    {
        public VMUC_ConfigBasic(VM_Config model)
        {
            _model = model;
        }
        protected VM_Config _model;

        public delegate void ChangedHandler();
        public event ChangedHandler Changed;
        protected void OnChanged([CallerMemberName] string propertyName = "")
        {
            Changed?.Invoke();
            OnPropertyChanged(propertyName);
        }
    }
}
