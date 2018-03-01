using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
        private TransactionDatabase transactionDatabase;

        public MainWindow()
        {

            InitializeComponent();

            new FbMessageProcessor(this);

            logDatabase = new LogDataBase(Dispatcher);
            DataContext = logDatabase.LogEntries;

            categoryDatabase = new CategoryDatabase(Dispatcher);
            supplierDatabase = new SupplierDatabase(Dispatcher);
            itemDatabase = new ItemDatabase(Dispatcher, categoryDatabase, supplierDatabase);
            transactionDatabase = new TransactionDatabase(Dispatcher, itemDatabase);

            UcShop.lbItems.ItemsSource = itemDatabase.DataView;
            UcShop.CbxSearchSupplier.ItemsSource = supplierDatabase.Data;
            UcOrder.lbItems.ItemsSource = itemDatabase.DataView;
            UcOrder.CbxSearchSupplier.ItemsSource = supplierDatabase.Data;
            UcSuppliers.lbSuppliers.ItemsSource = supplierDatabase.DataView;

            RegisterUiControlEvents();
            header.Content = DateTime.Today.ToString("dd.MM.yyyy");
            header.FontWeight = FontWeights.Bold;
            header.Margin = new Thickness(20, 10, 0, 0);
            nameD.Content = NameDays.getNamedayNames();
            nameD.Margin = new Thickness(5, 10, 0, 0);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fbClient = new FireBase(Dispatcher);
            UcShop.CbxSearchSupplier.SelectedIndex = 0;
            UcOrder.CbxSearchSupplier.SelectedIndex = 0;
            Log("window loaded");
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
            UcShop.BtnSell.Click += BtnSell_Click;
            UcShop.lbItems.PreviewMouseDoubleClick += BtnSell_Click;
            UcShop.BtnClearFilter.Click += BtnClearFilter_Click;
            UcShop.CbxSearchSupplier.SelectionChanged += CbxSearchSupplier_SelectionChanged;
            UcShop.TbxSearchName.TextChanged += TbxSearchName_TextChanged;

            UcOrder.BtnBuy.Click += BtnBuy_Click;
            UcOrder.BtnNewItem.Click += BtnNewItem_Click;
            UcOrder.BtnEditItem.Click += BtnEditItem_Click;
            UcOrder.lbItems.PreviewMouseDoubleClick += BtnBuy_Click;
            UcOrder.BtnDeleteItem.Click += BtnDeleteItem_Click;
            UcOrder.TbxSearchName.TextChanged += TbxSearchName_TextChanged;
            UcOrder.BtnClearFilter.Click += BtnClearFilter_Click;
            UcOrder.CbxSearchSupplier.SelectionChanged += CbxSearchSupplier_SelectionChanged;
            UcOrder.TbxSearchName.TextChanged += TbxSearchName_TextChanged;

            UcSuppliers.BtnNewSupplier.Click += BtnNewSupplier_Click;
            UcSuppliers.BtnEditSupplier.Click += BtnEditSupplier_Click;
            UcSuppliers.lbSuppliers.PreviewMouseDoubleClick += BtnEditSupplier_Click;
            UcSuppliers.BtnDeleteSupplier.Click += BtnDeleteSupplier_Click;

            UcLog.BtnClearLog.Click += BtnClearLog_Log;
        }

        /******************************************************* Filter items Events ******************************************************/
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShopTab.IsSelected) UcShop.SetSearchCriteria(itemDatabase);
            else if (OrderTab.IsSelected) UcOrder.SetSearchCriteria(itemDatabase);
        }
        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            if (ShopTab.IsSelected) UcShop.ClearFilter();
            else if (OrderTab.IsSelected) UcOrder.ClearFilter();
        }
        private void CbxSearchSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShopTab.IsSelected) UcShop.SetSearchCriteria(itemDatabase);
            else if (OrderTab.IsSelected) UcOrder.SetSearchCriteria(itemDatabase);
        }
        private void TbxSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ShopTab.IsSelected) UcShop.SetSearchCriteria(itemDatabase);
            else if (OrderTab.IsSelected) UcOrder.SetSearchCriteria(itemDatabase);
        }


        /********************************************************* Shop Tab Events ********************************************************/
        private void BtnSell_Click(object sender, RoutedEventArgs e)
        {
            UcShop.Sell(this, fbClient);
        }


        /********************************************************* Order Tab Events *******************************************************/
        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            UcOrder.Buy(this, fbClient);
        }
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
            UcOrder.DeleteItem(this, fbClient, transactionDatabase);
        }
        private void CreateItemPopupWindow(ItemPropEntry item)
        {
            new ItemWindow(item, this, categoryDatabase, supplierDatabase, itemDatabase).ShowDialog();
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
        public void AddProperties(string id, ITransactionPropEntryFb item) { transactionDatabase.AddProperties(id, item); }

        // Remove
        public void RemoveProperties(string id, IItemPropEntryFb item) { itemDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id, ICategoryPropEntryFb item) { categoryDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id, ISupplierPropEntryFb item) { supplierDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id, ITransactionPropEntryFb item) { transactionDatabase.RemoveProperties(id); }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            Reports reports = new Reports();
            reports.GenerateAllProductsReport(itemDatabase);            
        }
    }



}
