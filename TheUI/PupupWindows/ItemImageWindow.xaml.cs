using System.Collections.ObjectModel;
using System.Windows;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for ItemImageWindow.xaml
    /// </summary>
    public partial class ItemImageWindow : Window
    {
        public ObservableCollection<FbImage> Data { get; set; }

        public ItemImageWindow(ItemPropEntry item, Window mainWindow, ItemDatabase itemDatabase)
        {
            InitializeComponent();
            Data = new ObservableCollection<FbImage>();

            foreach (var entry in itemDatabase.Data)
            {
                Data.Add(entry.Image);
                Data.Add(entry.Image);
                Data.Add(entry.Image);
                Data.Add(entry.Image);
                Data.Add(entry.Image);
                Data.Add(entry.Image);
            }

            ImageListView.ItemsSource = Data;

            //cmbxCategory.ItemsSource = categoryDatabase.Data;
            //cmbxSupplier.ItemsSource = supplierDatabase.Data;


            // position this window in the middle of main window
            Point positionFromScreen = mainWindow.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(mainWindow);
            Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(positionFromScreen);
            Top = targetPoints.Y + (mainWindow.ActualHeight - Height) / 2;
            Left = targetPoints.X + (mainWindow.ActualWidth - Width) / 2;
            ShowInTaskbar = false;
        }
    }
}
