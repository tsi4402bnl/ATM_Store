using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TheUI
{
    public class LogDataBase
    {
        public LogDataBase(Dispatcher d)
        {
            dispatcher = d;
            LogEntries = new ObservableCollection<LogEntry>();
        }

        public void Clear_Log()
        {
            LogEntries.Clear();
        }

        public void Log(string msg)
        {
            DateTime time = DateTime.Now;
            string timeString = time.Hour.ToString() + ":";
            if (time.Minute <= 9) timeString += "0";
            timeString += time.Minute.ToString() + ":";
            if (time.Second <= 9) timeString += "0";
            timeString += time.Second.ToString() + "  ";

            dispatcher.Invoke(delegate
            {
                LogEntries.Add(new LogEntry() { Index = logIndex++, DateTime = timeString, Message = msg });
            });
        }

        public ObservableCollection<LogEntry> LogEntries { get; set; } // holds all log data

        private int logIndex;
        private Dispatcher dispatcher;
    }

    public class LogEntry : PropertyChangedBase
    {
        public string DateTime { get; set; }
        public int    Index    { get; set; }
        public string Message  { get; set; }
    }

    // To automatically scroll to the bottom of Log
    public static class AutoScrollHelper
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollHelper), new PropertyMetadata(false, AutoScrollPropertyChanged));


        public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;
            if (scrollViewer == null) return;

            if ((bool)args.NewValue)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                scrollViewer.ScrollToEnd();
            }
            else
            {
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange > 0 || e.ExtentHeightChange < 0)
            {
                var scrollViewer = sender as ScrollViewer;
                scrollViewer?.ScrollToEnd();
            }
        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }
    }
}
