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
}
