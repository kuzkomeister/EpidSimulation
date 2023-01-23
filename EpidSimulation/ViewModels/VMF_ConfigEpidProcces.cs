using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using EpidSimulation.Models;
using EpidSimulation.Utils;
using EpidSimulation.ViewModels.Configs;

namespace EpidSimulation.ViewModels
{
    public class VMF_ConfigEpidProcces : VM_BASIC
    {
        public VMF_ConfigEpidProcces(Config model)
        {
            Config = new VM_Config(model);

            V_Diseases = new VMUC_Diseases(Config);
            V_SocialActs = new VMUC_SocialActs(Config);
            V_ChancesInfection = new VMUC_ChancesInfection(Config);
            V_AreasInteraction = new VMUC_AreasInteraction(Config);
            V_Masks = new VMUC_Masks(Config);

            V_AreasInteraction.Changed += ChangedHandler;
            V_ChancesInfection.Changed += ChangedHandler;
            V_Diseases.Changed += ChangedHandler;
            V_Masks.Changed += ChangedHandler;
            V_SocialActs.Changed += ChangedHandler;
        }

        private void ChangedHandler()
        {
            V_CanSave = true;
        }

        #region [ Свойства VM ]

        public VMUC_AreasInteraction V_AreasInteraction { get; set; }
        public VMUC_ChancesInfection V_ChancesInfection { get; set; }
        public VMUC_Diseases V_Diseases { get; set; }
        public VMUC_Masks V_Masks { get; set; }
        public VMUC_SocialActs V_SocialActs { get; set; }

        public VM_Config Config { get; set; }

        public bool V_CanSave
        {
            get => _canSave;
            set 
            { 
                _canSave = value; 
                OnPropertyChanged();
            }
        }
        private bool _canSave;

        #endregion

        #region [ Команды ]

        public RelayCommand<Window> CmdSave { get => new RelayCommand<Window>(_DoSave); }
        private void _DoSave(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }

        #endregion

    }
}
