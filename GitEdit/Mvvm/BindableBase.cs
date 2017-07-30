using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GitEdit.Mvvm
{
    public class BindableBase
        : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(propertyName != null);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<X>(ref X field, X value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<X>.Default.Equals(field, value)) return;

            field = value;
            NotifyPropertyChanged(propertyName);
        }
    }
}
