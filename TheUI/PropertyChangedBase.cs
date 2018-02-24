using System;
using System.ComponentModel;
using System.Windows;

namespace TheUI
{
    // To automatically update XAML when value has changed
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }

    public class ObservableString : ObservableType<string>
    {
        public ObservableString() : base() { }
        public ObservableString(string value) : base(value) { }
    }

    public class ObservableInt : ObservableType<int>
    {
        public ObservableInt() : base() { }
        public ObservableInt(int value) : base(value) { }
    }

    public class ObservableDouble : ObservableType<double>
    {
        public ObservableDouble() : base() { }
        public ObservableDouble(double value) : base(value) { }
    }

    public class ObservableDateTime : ObservableType<DateTime>
    {
        public ObservableDateTime() : base() { }
        public ObservableDateTime(DateTime value) : base(value) { }
    }

    public class ObservableType<T> : PropertyChangedBase
    {
        public ObservableType() { }
        public ObservableType(T value) { Value = value; }

        T val;
        public T Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged("Value"); }
        }
    }
}
