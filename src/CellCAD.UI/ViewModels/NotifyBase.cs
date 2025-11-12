using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace CellCAD.viewmodels
{
    /// <summary>
    /// Minimal INotifyPropertyChanged base to avoid external deps.
    /// </summary>
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;
            storage = value;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Raise([CallerMemberName] string propertyName = null)
            => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
