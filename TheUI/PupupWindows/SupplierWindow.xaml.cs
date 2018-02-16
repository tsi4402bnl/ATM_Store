using System.Windows;

namespace TheUI
{
    public partial class SupplierWindow : Window
    {
        public SupplierWindow(SupplierPropEntry s, MainWindow mainWindow)
        {
            supplier = new SupplierPropEntry(s);
            InitializeComponent();
            gSupplier.DataContext = supplier;
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
            string tableName = "suppliers";
            if (supplier.Id.Value.Length > 0)
                fbClient.ModifyInFb(tableName, supplier.Id.Value, supplier.GetPropEntryFb());
            else
                fbClient.InsertInFb(tableName, supplier.GetPropEntryFb());
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private SupplierPropEntry supplier { get; set; }
        private FireBase fbClient;
    }
}
