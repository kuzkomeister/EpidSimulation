using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels
{
    public class VM_Disease
    {
        public string V_Name { get; set; }
        public bool V_IsContact { get; set; }
        public bool V_IsAirborne { get; set; }
        public double V_ChanceDie { get; set; }
        public double V_ChanceAsymp { get; set; }
    }
}
