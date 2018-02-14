using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Page
    {
        // public fields / properties
        public FireBase fbClient;

        // private fields / properties
        private LogDataBase logDatabase;
        private ItemDatabase itemDatabase;
        private CategoryDatabase categoryDatabase;

        public MainWindow(int w, int h)
        {
            if (null == Application.Current)
            {
                new Application();
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }

            InitializeComponent();
            Width = w; // app size is defined in main.cpp
            Height = h;

            logDatabase = new LogDataBase(Dispatcher);
            DataContext = logDatabase.LogEntries;

            categoryDatabase = new CategoryDatabase(Dispatcher);

            itemDatabase = new ItemDatabase(Dispatcher, categoryDatabase);
            dgItems.ItemsSource = itemDatabase.Data;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e) { fbClient = new FireBase(Dispatcher); }

        // Log functions
        private void Clear_Log(object sender, RoutedEventArgs e) { logDatabase.Clear_Log(); }
        public void Log(string msg) { logDatabase.Log(msg); }


        /********************************************************* Item Tab Events ********************************************************/
        private void BtnNewItem_Click(object sender, RoutedEventArgs e)
        {
            CreateItemPopupWindow(new ItemPropEntry());
        }    
        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem != null) CreateItemPopupWindow((ItemPropEntry)dgItems.SelectedItem);
        }
        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem != null) fbClient.DeleteFromFb("items", ((ItemPropEntry)dgItems.SelectedItem).Id.Value);
        }
        private void CreateItemPopupWindow(ItemPropEntry item)
        {
            new wintouch(item, this).ShowDialog();
        }


        /************************************************** Fetch/Fill DB, called from C++ ************************************************/
        // Fetch
        public bool IsFbMessagePending() { return fbClient == null ? false : fbClient.IsFbMessagePending(); }
        public FbEventData FetchNextFbMessage() { return fbClient == null ? new FbEventData() : fbClient.FetchNextFbMessage(); }

        // Add
        public void AddItemProperties(string id, ItemPropEntryFb item) {
            itemDatabase.AddProperties(id, item); }

        // Remove
        public void RemoveItemProperties(string id) { itemDatabase.RemoveProperties(id); }
    }



}
