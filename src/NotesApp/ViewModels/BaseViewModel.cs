using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (_propertyValues.TryGetValue(propertyName, out var existingValue) &&
                EqualityComparer<T>.Default.Equals((T)existingValue, value))
            {
                return false;
            }

            _propertyValues[propertyName] = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (_propertyValues.TryGetValue(propertyName, out var value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
