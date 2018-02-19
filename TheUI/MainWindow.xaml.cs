using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
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
            lbItems.ItemsSource = itemDatabase.Data;
            lbSuppliers.ItemsSource = supplierDatabase.DataView;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e) { fbClient = new FireBase(Dispatcher); }

        // Log functions
        private void Clear_Log(object sender, RoutedEventArgs e) { logDatabase.Clear_Log(); }
        public void Log(string msg) { logDatabase.Log(msg); }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    ImageSource source = new BitmapImage(new Uri(dlg.FileName));
                    int width = 100;
                    int height = 100;

                    var rect = new Rect(0, 0, width, height);
                    var group = new DrawingGroup();
                    RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
                    group.Children.Add(new ImageDrawing(source, rect));
                    var drawingVisual = new DrawingVisual();
                    using (var drawingContext = drawingVisual.RenderOpen()) drawingContext.DrawDrawing(group);
                    var resizedImage = new RenderTargetBitmap( width, height, 96, 96, PixelFormats.Default);
                    resizedImage.Render(drawingVisual);
                    
                    byte[] data;
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(resizedImage));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        data = ms.ToArray();
                    }
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = new MemoryStream(data);
                    img.EndInit();
                    imgPhoto.Source = img;

                }
                catch (System.IO.FileNotFoundException)
                {
                    MessageBox.Show("There was an error opening the bitmap." +
                        "Please check the path.");
                }
                string filename = dlg.FileName;
            }
        }


        /********************************************************* Item Tab Events ********************************************************/
        private void BtnNewItem_Click(object sender, RoutedEventArgs e)
        {
            CreateItemPopupWindow(new ItemPropEntry());
        }    
        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        {
            if (lbItems.SelectedItem != null) CreateItemPopupWindow((ItemPropEntry)lbItems.SelectedItem);
        }
        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (lbItems.SelectedItem != null) fbClient.DeleteFromFb("items", ((ItemPropEntry)lbItems.SelectedItem).Id.Value);
        }
        private void CreateItemPopupWindow(ItemPropEntry item)
        {
            new ItemWindow(item, this, categoryDatabase, supplierDatabase).ShowDialog();
        }


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
        public void AddProperties(string id,     IItemPropEntryFb item) {     itemDatabase.AddProperties(id, item); }
        public void AddProperties(string id, ICategoryPropEntryFb item) { categoryDatabase.AddProperties(id, item); }
        public void AddProperties(string id, ISupplierPropEntryFb item) { supplierDatabase.AddProperties(id, item); }

        // Remove
        public void     RemoveItemProperties(string id) {     itemDatabase.RemoveProperties(id); }
        public void RemoveCategoryProperties(string id) { categoryDatabase.RemoveProperties(id); }
        public void RemoveSupplierProperties(string id) { supplierDatabase.RemoveProperties(id); }

    }



}
