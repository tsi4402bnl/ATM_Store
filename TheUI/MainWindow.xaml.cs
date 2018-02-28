using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public sealed partial class MainWindow : Window, IDisposable
    {
        // public fields / properties
        public FireBase fbClient;

        // private fields / properties
        private LogDataBase logDatabase;

        public MainWindow()
        {

            InitializeComponent();

            new FbMessageProcessor(this);

            logDatabase = new LogDataBase(Dispatcher);
            DataContext = logDatabase.LogEntries;

            RegisterUiControlEvents();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fbClient = new FireBase(Dispatcher);
        }

        // Log functions
        private void BtnClearLog_Log(object sender, RoutedEventArgs e) { logDatabase.Clear_Log(); }
        public void Log(string msg) { logDatabase.Log(msg); }

        public void Dispose()
        {
            fbClient.Dispose();
        }

        private void RegisterUiControlEvents()
        {
            UcLog.BtnClearLog.Click += BtnClearLog_Log;
        }

        /*********************************************************** Fetch/Fill DB ********************************************************/
        // Fetch
        public bool IsFbMessagePending() { return fbClient == null ? false : fbClient.IsFbMessagePending(); }
        public FbEventData FetchNextFbMessage() { return fbClient == null ? new FbEventData() : fbClient.FetchNextFbMessage(); }

        
    }



}
