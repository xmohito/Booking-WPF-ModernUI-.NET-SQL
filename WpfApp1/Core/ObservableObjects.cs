using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp_1.Core
{
    class ObservableObjects : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
