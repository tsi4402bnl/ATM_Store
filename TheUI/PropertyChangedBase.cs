using System;
using System.ComponentModel;
using System.Windows;

namespace TheUI
{
    // To automatically update XAML when value has changed
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }

    public class ObservableString : PropertyChangedBase
    {
        string val;

        public string Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged("Value"); }
        }
    }

    public class ObservableInt : PropertyChangedBase
    {
        int val;

        public int Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged("Value"); }
        }
    }

    public class ObservableDouble : PropertyChangedBase
    {
        double val;

        public double Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged("Value"); }
        }
    }
}
