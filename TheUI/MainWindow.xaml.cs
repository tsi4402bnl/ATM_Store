using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
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
    public partial class MainWindow : Page
    {
        private static FireBase fbClient;

        ItemDatabase itemDatabase;
        LogDataBase logDatabase;

        public MainWindow(int w, int h)
        {
            if (null == Application.Current)
            {
                new Application();
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }

            InitializeComponent();
            Width = w; // app size is defined in main.cpp
            Height = h;
            Init();
        }

        private void Init()                  
        {
            itemDatabase = new ItemDatabase(Dispatcher);
            dgItems.ItemsSource = itemDatabase.Data;
            logDatabase = new LogDataBase(Dispatcher);
            DataContext = logDatabase.LogEntries;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fbClient = new FireBase(Dispatcher);
        }

        public bool IsFbMessagePending()
        {
            if (fbClient == null) return false;
            return fbClient.IsFbMessagePending(); // Called from C++ to know if it needs to parse fb message
        }

        public FbEventData FetchNextFbMessage()
        {
            if (fbClient == null) return new FbEventData();
            return fbClient.FetchNextFbMessage(); // Called from C++, will parse next fb message
        }

        private void Clear_Log(object sender, RoutedEventArgs e) { logDatabase.Clear_Log(); }
        public void Log(string msg) { logDatabase.Log(msg); }

        /************************************************************ Item Tab Events *********************************************************/
        private void BtnNewItem_Click(object sender, RoutedEventArgs e)
        {
            CreateItemPopupWindow(new ItemPropEntry());
        }

        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem == null) return;
            CreateItemPopupWindow((ItemPropEntry)dgItems.SelectedItem);
        }
        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem == null) return;
            fbClient.DeleteFromFb("items", ((ItemPropEntry)dgItems.SelectedItem).Id.Value);
        }

        private void CreateItemPopupWindow(ItemPropEntry item)
        {
            wintouch cw = new wintouch(item, new _delUpdateFbItem(UpdateFbItem));

            Point positionFromScreen = PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(this);
            Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(positionFromScreen);
            cw.Top = targetPoints.Y + (ActualHeight - cw.Height) / 2;
            cw.Left = targetPoints.X + (ActualWidth - cw.Width) / 2;
            cw.ShowInTaskbar = false;

            cw.ShowDialog();
        }

        delegate void _delUpdateFbItem(ItemPropEntry item);
        private void UpdateFbItem(ItemPropEntry item)
        {
            string tableName = "items";
            if (item.Id.Value.Length > 0)
                fbClient.ModifyInFb(tableName, item.Id.Value, item.GetItemPropEntryFb());
            else
                fbClient.InsertInFb(tableName, item.GetItemPropEntryFb());
        }

        // will be called automatically from C++
        public void AddItemProperties(string id, ItemPropEntryFb item)
        {
            itemDatabase.AddItemProperties(id, item);
        }
        public void RemoveItemProperties(string id)
        {
            itemDatabase.RemoveItemProperties(id);
        }

    }
}
