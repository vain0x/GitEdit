using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GitEdit.ViewModel
{
    public class ViewModelBase
        : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(propertyName != null);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<X>(ref X r, X x, [CallerMemberName] string propertyName = null)
        {
            r = x;
            NotifyPropertyChanged(propertyName);
        }
    }
}
