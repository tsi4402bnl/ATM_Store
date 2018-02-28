using System.Windows;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public sealed partial class MainWindow : Window
    {
        // private fields / properties
        private LogDataBase logDatabase;

        public MainWindow()
        {

            InitializeComponent();

            logDatabase = new LogDataBase(Dispatcher);
            DataContext = logDatabase.LogEntries;

            RegisterUiControlEvents();
        }

        // Log functions
        private void BtnClearLog_Log(object sender, RoutedEventArgs e) { logDatabase.Clear_Log(); }
        public void Log(string msg) { logDatabase.Log(msg); }

        private void RegisterUiControlEvents()
        {
            UcLog.BtnClearLog.Click += BtnClearLog_Log;
        }

        
    }



}
