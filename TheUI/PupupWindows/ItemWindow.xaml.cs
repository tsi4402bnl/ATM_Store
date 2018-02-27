using System.Windows;

namespace TheUI
{
    public partial class ItemWindow : CenterWindow
    {
        public ItemWindow(ItemPropEntry i, MainWindow mainWindow, CategoryDatabase categoryDatabase, SupplierDatabase supplierDatabase, ItemDatabase itemDatabase) : base(mainWindow)
        {
            item = new ItemPropEntry(i);
            InitializeComponent();
            gItem.DataContext = item;
            fbClient = mainWindow.fbClient;

            cmbxCategory.ItemsSource = categoryDatabase.Data;
            cmbxSupplier.ItemsSource = supplierDatabase.Data;
            this.itemDatabase = itemDatabase;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string tableName = "items";
            if (cmbxCategory.SelectedValue != null)
                item.Category.Id.Value = cmbxCategory.SelectedValue.ToString();
            if (cmbxSupplier.SelectedValue != null)
                item.Supplier.Id.Value = cmbxSupplier.SelectedValue.ToString();

            if (item.Id.Value.Length > 0)
                fbClient.ModifyInFb(tableName, item.Id.Value, item.GetPropEntryFb());
            else
                fbClient.InsertInFb(tableName, item.GetPropEntryFb());
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private ItemPropEntry item { get; set; }
        private FireBase fbClient;
        private ItemDatabase itemDatabase;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (item != null && item.Id.Value.Length > 0)
            {
                cmbxCategory.SelectedValue = item.Category.Id.Value;
                cmbxSupplier.SelectedValue = item.Supplier.Id.Value;
            }
        }

        private void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            new ItemImageWindow(item, this, itemDatabase).ShowDialog();
        }
    }
}
