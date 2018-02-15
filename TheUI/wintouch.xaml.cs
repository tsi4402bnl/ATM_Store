using System.Windows;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for wintouch.xaml
    /// </summary>
    public partial class wintouch : Window
    {
        public wintouch(ItemPropEntry i, MainWindow mainWindow, CategoryDatabase categoryDatabase)
        {
            item = new ItemPropEntry(i);
            InitializeComponent();
            gItem.DataContext = item;
            fbClient = mainWindow.fbClient;

            cmbxCategory.ItemsSource = categoryDatabase.Data;

            // position this window in the middle of main window
            Point positionFromScreen = mainWindow.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(mainWindow);
            Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(positionFromScreen);
            Top = targetPoints.Y + (mainWindow.ActualHeight - Height) / 2;
            Left = targetPoints.X + (mainWindow.ActualWidth - Width) / 2;
            ShowInTaskbar = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string tableName = "items";
            if (cmbxCategory.SelectedValue != null)
                item.Category.Id.Value = cmbxCategory.SelectedValue.ToString();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (item != null && item.Id.Value.Length > 0)
            {
                cmbxCategory.SelectedValue = item.Category.Id.Value; ;
            }
        }
    }
}
