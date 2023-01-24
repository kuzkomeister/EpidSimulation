using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EpidSimulation.ViewModels
{
    public abstract class VM_BASIC : INotifyPropertyChanged
    {
        /// <summary>
        /// Объявление свойства из INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
