using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            Data = new ObservableCollection<FbImage>
            {
                new FbImage("")
            };
            HashSet<string> uniqueImages = new HashSet<string>();
            foreach (var entry in itemDatabase.Data)
            {
                if (entry.Image.FbData.Length > 0 && uniqueImages.Add(entry.Image.FbData)) Data.Add(entry.Image);
            }
            this.item = item;
            
            ImageListView.ItemsSource = Data;

            // position this window in the middle of main window
            Point positionFromScreen = mainWindow.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(mainWindow);
            Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(positionFromScreen);
            Top = targetPoints.Y + (mainWindow.ActualHeight - Height) / 2;
            Left = targetPoints.X + (mainWindow.ActualWidth - Width) / 2;
            ShowInTaskbar = false;
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".png",
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    ImageSource source = new BitmapImage(new Uri(dlg.FileName));

                    // resize image
                    int width = 100;
                    int height = 100;
                    var rect = new Rect(0, 0, width, height);
                    var group = new DrawingGroup();
                    RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
                    group.Children.Add(new ImageDrawing(source, rect));
                    var drawingVisual = new DrawingVisual();
                    using (var drawingContext = drawingVisual.RenderOpen()) drawingContext.DrawDrawing(group);
                    var resizedImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
                    resizedImage.Render(drawingVisual);

                    // convert ImageSource -> byte array -> hex string
                    byte[] data;
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(resizedImage));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        data = ms.ToArray();
                    }
                    string fbData = String.Concat(Array.ConvertAll(data, x => x.ToString("X2")));

                    Data.Insert(1, new FbImage(fbData));
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("There was an error opening the bitmap. Please check the path.");
                }
                string filename = dlg.FileName;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (ImageListView.SelectedItem != null)
            {
                item.Image = (FbImage)ImageListView.SelectedItem;
            }
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private ItemPropEntry item;
    }
}
