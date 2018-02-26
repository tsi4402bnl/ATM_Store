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

            lbItems.ItemsSource = itemDatabase.DataView;
            CbxSearchSupplier.ItemsSource = supplierDatabase.Data;
            //CbxSearchSupplier.
            lbSuppliers.ItemsSource = supplierDatabase.DataView;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fbClient = new FireBase(Dispatcher);
            CbxSearchSupplier.SelectedIndex = 0;
        }

        // Log functions
        private void Clear_Log(object sender, RoutedEventArgs e) { logDatabase.Clear_Log(); }
        public void Log(string msg) { logDatabase.Log(msg); }

        public void Dispose()
        {
            fbClient.Dispose();
        }

        /********************************************************* Item Tab Events ********************************************************/
        private void BtnSell_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            CbxSearchSupplier.SelectedIndex = 0;
            TbxSearchName.Clear();
            //itemDatabase.SetSearchCriteria("", "");
        }
        private void CbxSearchSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSearchCriteria();
        }
        private void TbxSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSearchCriteria();
        }
        private void SetSearchCriteria()
        {
            string searchName = TbxSearchName.Text;
            string searchSupplierId = "";
            if (CbxSearchSupplier.SelectedIndex != -1 && CbxSearchSupplier.SelectedValue != null)
                searchSupplierId = CbxSearchSupplier.SelectedValue.ToString();
            itemDatabase.SetSearchCriteria(searchName, searchSupplierId);
        }
        //private void BtnNewItem_Click(object sender, RoutedEventArgs e)
        //{
        //    CreateItemPopupWindow(new ItemPropEntry());
        //}    
        //private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (lbItems.SelectedItem != null) CreateItemPopupWindow((ItemPropEntry)lbItems.SelectedItem);
        //}
        //private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (lbItems.SelectedItem != null) fbClient.DeleteFromFb("items", ((ItemPropEntry)lbItems.SelectedItem).Id.Value);
        //}
        //private void CreateItemPopupWindow(ItemPropEntry item)
        //{
        //    new ItemWindow(item, this, categoryDatabase, supplierDatabase, itemDatabase).ShowDialog();
        //}


        /******************************************************* Supplier Tab Events ******************************************************/
        private void BtnNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            CreateSupplierPopupWindow(new SupplierPropEntry());
        }
        private void BtnEditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (lbSuppliers.SelectedItem != null) CreateSupplierPopupWindow((SupplierPropEntry)lbSuppliers.SelectedItem);
        }
        private void BtnDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (lbSuppliers.SelectedItem != null)
            {
                if (!itemDatabase.Contain((SupplierPropEntry)lbSuppliers.SelectedItem))
                {
                    fbClient.DeleteFromFb("suppliers", ((SupplierPropEntry)lbSuppliers.SelectedItem).Id.Value);
                }
                else
                {
                    MessageBox.Show("Supplier in use!");
                }
            }
        }
        private void CreateSupplierPopupWindow(SupplierPropEntry supplier)
        {
            new SupplierWindow(supplier, this).ShowDialog();
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
        public void RemoveProperties(string id,        IItemPropEntryFb item) {        itemDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id,    ICategoryPropEntryFb item) {    categoryDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id,    ISupplierPropEntryFb item) {    supplierDatabase.RemoveProperties(id); }
        public void RemoveProperties(string id, ITransactionPropEntryFb item) { transactionDatabase.RemoveProperties(id); }

        
    }



}
