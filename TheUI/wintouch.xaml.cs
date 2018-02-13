using System.Windows;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for wintouch.xaml
    /// </summary>
    public partial class wintouch : Window
    {
        public wintouch(ItemPropEntry i, MainWindow mainWindow)
        {
            item = new ItemPropEntry(i);
            InitializeComponent();
            gItem.DataContext = item;
            fbClient = mainWindow.fbClient;

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
    }
}
