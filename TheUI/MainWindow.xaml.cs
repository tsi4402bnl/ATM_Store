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
        private ItemDatabase itemDatabase;
        private CategoryDatabase categoryDatabase;
        private SupplierDatabase supplierDatabase;

        public MainWindow()
        {

            InitializeComponent();

            new FbMessageProcessor(this);

            logDatabase = new LogDataBase(Dispatcher);
            DataContext = logDatabase.LogEntries;

            categoryDatabase = new CategoryDatabase(Dispatcher);
            supplierDatabase = new SupplierDatabase(Dispatcher);
            itemDatabase = new ItemDatabase(Dispatcher, categoryDatabase, supplierDatabase);

            UcOrder.lbItems.ItemsSource = itemDatabase.DataView;
            UcSuppliers.lbSuppliers.ItemsSource = supplierDatabase.DataView;

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
            UcOrder.BtnNewItem.Click += BtnNewItem_Click;
            UcOrder.BtnEditItem.Click += BtnEditItem_Click;
            UcOrder.BtnDeleteItem.Click += BtnDeleteItem_Click;

            UcSuppliers.BtnNewSupplier.Click += BtnNewSupplier_Click;
            UcSuppliers.BtnEditSupplier.Click += BtnEditSupplier_Click;
            UcSuppliers.lbSuppliers.PreviewMouseDoubleClick += BtnEditSupplier_Click;
            UcSuppliers.BtnDeleteSupplier.Click += BtnDeleteSupplier_Click;

            UcLog.BtnClearLog.Click += BtnClearLog_Log;
        }

        /********************************************************* Order Tab Events *******************************************************/
        private void BtnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UcOrder.NewItem(this, categoryDatabase, supplierDatabase, itemDatabase);
        }
        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        {
            UcOrder.EditItem(this, categoryDatabase, supplierDatabase, itemDatabase);
        }
        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            UcOrder.DeleteItem(this, fbClient);
        }

        /******************************************************* Supplier Tab Events ******************************************************/
        private void BtnNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            UcSuppliers.CreateSupplier(this);
        }
        private void BtnEditSupplier_Click(object sender, RoutedEventArgs e)
        {
            UcSuppliers.EditSupplier(this);
        }
        private void BtnDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            UcSuppliers.DeleteSupplier(this, itemDatabase, fbClient);
        }


        /*********************************************************** Fetch/Fill DB ********************************************************/
        // Fetch
        public bool IsFbMessagePending() { return fbClient == null ? false : fbClient.IsFbMessagePending(); }
        public FbEventData FetchNextFbMessage() { return fbClient == null ? new FbEventData() : fbClient.FetchNextFbMessage(); }

        // Add
        public void AddProperties(string id,        IItemPropEntryFb item) {        itemDatabase.AddProperties(id, item); }
        public void AddProperties(string id,    ICategoryPropEntryFb item) {    categoryDatabase.AddProperties(id, item); }
        public void AddProperties(string id,    ISupplierPropEntryFb item) {    supplierDatabase.AddProperties(id, item); }

        // Remove
        public void RemoveProperties(string id,        IItemPropEntryFb item) {        itemDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id,    ICategoryPropEntryFb item) {    categoryDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id,    ISupplierPropEntryFb item) {    supplierDatabase.RemoveProperties(id); }

        
    }



}
