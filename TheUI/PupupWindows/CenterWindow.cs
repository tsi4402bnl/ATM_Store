using System.Windows;

namespace TheUI
{
    public abstract class CenterWindow : Window
    {
        private Window parent;
        public CenterWindow(Window parent) : base()
        {
            this.parent = parent;
            Loaded += CenterWindow_Loaded;
            Initialized += CenterWindow_Initialized; ;
        }

        private void CenterWindow_Initialized(object sender, System.EventArgs e)
        {
            //position this window in the middle of main window
            Point positionFromScreen = parent.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(parent);
            Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(positionFromScreen);
            Top = targetPoints.Y + (parent.ActualHeight - Height) / 2;
            Left = targetPoints.X + (parent.ActualWidth - Width) / 2;
            ShowInTaskbar = false;
        }

        private void CenterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Height != ActualHeight || Width != ActualWidth) Center(ActualHeight, ActualWidth);
            ShowInTaskbar = false;
        }

        private void Center(double h, double w)
        {
            Point positionFromScreen = parent.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(parent);
            Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(positionFromScreen);
            Top = targetPoints.Y + (parent.ActualHeight - h) / 2;
            Left = targetPoints.X + (parent.ActualWidth - w) / 2;
        }
    }
}
