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
        public ObservableCollection<VM_Disease> V_List { get; set; } = new ObservableCollection<VM_Disease>()
        {
            new VM_Disease()
            {
                V_Name = "Covid-19",
                V_ChanceAsymp = 0.4,
                V_ChanceDie = 0.25,
                V_IsAirborne = true,
                V_IsContact = true,
            },
        };
    }
}
