﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpidSimulation.ViewModels.Configs
{
    public class VMUC_AreasInteraction : VM_BASIC
    {
        public delegate void ChangedHandler();
        public event ChangedHandler Changed;
    }
}
