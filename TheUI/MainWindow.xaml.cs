using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private int logIndex;
        private static FirebaseClient fbClient;

        public static ObservableCollection<ItemPropEntry> itemProperties { get; set; }
        public static HashSet<string> uniqueItemIds; // fb item table entry ids, 

        public static ObservableString btnTestTabItem1Text { get; set; } // only for test purposes
        public static ObservableCollection<LogEntry> LogEntries { get; set; } // holds all log data
        public static Queue<FbEventData> fbReceivedMessages; // holds fb mesaages which needs to be parsed by C++

        public MainWindow()
        {
            if (null == Application.Current)
            {
                new Application();
            }

            InitializeComponent();
            Init();
        }

        public MainWindow(int w, int h)
        {
            if (null == Application.Current)
            {
                new Application();
            }

            InitializeComponent();
            Width = w; // app size is defined in main.cpp
            Height = h;
            Init();
        }

        private void Init()                  
        {
            btnTestTabItem1Text = new ObservableString() { Value = "Click Me!" }; // only for test purposes

            itemProperties = new ObservableCollection<ItemPropEntry>();
            dgItems.ItemsSource = itemProperties;

            uniqueItemIds = new HashSet<string>();
            fbReceivedMessages = new Queue<FbEventData>();
            DataContext = LogEntries = new ObservableCollection<LogEntry>();
            InitFireBase();
        }

        private void btnTestTabItem1_Click(object sender, RoutedEventArgs e)
        {
            //InsertInFb("items", new ItemPropEntryFb("apple", 123));
            //InsertInFb("items", new ItemPropEntryFb("tomato", 345));
            if (itemProperties.Count > 0)
                ModifyInFb("items", itemProperties[0].Id.Value, new ItemPropEntryFb("apple", itemProperties[0].Price.Value + 1));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedIndex < itemProperties.Count)
            {
                int i = dgItems.SelectedIndex;
                string id    = itemProperties[i].Id.Value;
                string name  = itemProperties[i].Name.Value;
                double    price = itemProperties[i].Price.Value;
                //ModifyInFb("items", id, new ItemPropEntryFb(name, price));
                Log(id + ", " + name + ", " + price.ToString());
            }
        }

        /************************************************************ Firebase ************************************************************/
        private void InitFireBase()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "YL6Bb5ejs59R4f8xFRLzcVcrjXu8klIPPKeAxHbv",
                BasePath = "https://atm-store.firebaseio.com/"
            };
            fbClient = new FirebaseClient(config);
        }

        private async void InsertInFb<T>(string tableName, T data)
        {
            await fbClient.PushAsync(tableName, data);
        }

        private async void ModifyInFb<T>(string tableName, string id, T data)
        {
            await fbClient.UpdateAsync(tableName + "/" + id, data);
        }

        private ValueAddedEventHandler GetFbItemAdded()
        {
            return (s, args, c) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    fbReceivedMessages.Enqueue(new FbEventData() { operation = Fb_Operations.fb_add, data = args.Data, path = args.Path });
                }, DispatcherPriority.Normal);
            };
        }

        private ValueChangedEventHandler GetFbItemChanged()
        {
            return (s, args, c) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    fbReceivedMessages.Enqueue(new FbEventData() { operation = Fb_Operations.fb_edit, data = args.Data, path = args.Path });
                }, DispatcherPriority.Normal);
            };
        }

        private ValueRemovedEventHandler GetFbItemRemoved()
        {
            return (s, args, c) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    fbReceivedMessages.Enqueue(new FbEventData() { operation = Fb_Operations.fb_delete, data = "", path = args.Path });
                }, DispatcherPriority.Normal);
            };
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ValueAddedEventHandler ItemAdded = GetFbItemAdded();
            ValueChangedEventHandler ItemChanged = GetFbItemChanged();
            ValueRemovedEventHandler ItemRemoved = GetFbItemRemoved();
            // fetches fb events
            await fbClient.OnAsync("", ItemAdded, ItemChanged, ItemRemoved);
        }

        public bool IsFbMessagePending()
        {
            return fbReceivedMessages.Count > 0; // Called from C++ to know if it needs to parse fb message
        }
        public FbEventData FetchNextFbMessage()
        {
            return fbReceivedMessages.Dequeue(); // Called from C++, will parse next fb message
        }


    /****************************************************** UI property setters ********************************************************/

        // will be called automatically from C++
        public void AddItemProperties(string id, string name, double price)
        {
            Dispatcher.Invoke(delegate
            {
                if (uniqueItemIds.Add(id))
                {
                    itemProperties.Add(new ItemPropEntry(id, name, price));
                }
                else
                {
                    for (int i = 0; i < itemProperties.Count; i++)
                    {
                        if (itemProperties[i].Id.Value == id)
                        {
                            if (name.Length != 0) itemProperties[i].Name.Value = name;
                            if (price >= 0) itemProperties[i].Price.Value = price;
                        }
                    }
                }
            });
        }

        public void RemoveItemProperties(string id)
        {
            Dispatcher.Invoke(delegate
            {
                if (uniqueItemIds.Remove(id))
                {
                    for (int i = 0; i < itemProperties.Count; i++)
                    {
                        if (itemProperties[i].Id.Value == id)
                        {
                            itemProperties.RemoveAt(i);
                            break;
                        }
                    }
                }
            });
        }

        /********************************************************* Control Getters *********************************************************/
        public Button GetTestTabBtn() // for testing purposes only
        {
            return btnTestTabItem1;
        }

    /********************************************************** LOG functions *********************************************************/
        private void Clear_Log(object sender, RoutedEventArgs e)
        {
            LogEntries.Clear();
        }

        public void Log(string msg)
        {
            DateTime time = DateTime.Now;
            string timeString = time.Hour.ToString() + ":";
            if (time.Minute <= 9) timeString += "0";
            timeString += time.Minute.ToString() + ":";
            if (time.Second <= 9) timeString += "0";
            timeString += time.Second.ToString() + "  ";

            Dispatcher.Invoke(delegate
            {
                LogEntries.Add(new LogEntry() { Index = logIndex++, DateTime = timeString, Message = msg });
            });
        }
    }

    /********************************************************* Helper classes *********************************************************/

    // To automatically update XAML when value has changed
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }

    public class ObservableString : PropertyChangedBase
    {
        string val;

        public string Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged("Value"); }
        }
    }

    public class ObservableInt : PropertyChangedBase
    {
        int val;

        public int Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged("Value"); }
        }
    }

    public class ObservableDouble : PropertyChangedBase
    {
        double val;

        public double Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged("Value"); }
        }
    }

    public class LogEntry : PropertyChangedBase
    {
        public string DateTime { get; set; }
        public int    Index    { get; set; }
        public string Message  { get; set; }
    }

    public class ItemPropEntry : PropertyChangedBase
    {
        public ItemPropEntry()
        {
            Init(id: "", name: "", price: -1.0);
        }
        public ItemPropEntry(string id, string name, double price)
        {
            Init(id: id, name: name, price: price);
        }
        public ItemPropEntry(string name, double price)
        {
            Init(id: "", name: name, price: price); ;
        }
        public ObservableString Id    { get; set; }
        public ObservableString Name  { get; set; }
        public ObservableDouble    Price { get; set; }

        private void Init(string id, string name, double price)
        {
            Id    = new ObservableString() { Value = id    };
            Name  = new ObservableString() { Value = name  };
            Price = new ObservableDouble() { Value = price };
        }
    }

    public class ItemPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public ItemPropEntryFb(string name, double price) { Name = name; Price = price; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public enum Fb_Operations
    {
          fb_add = 1
        , fb_edit
        , fb_delete
    }

    public struct FbEventData
    {
        public Fb_Operations operation;
        public string data;
        public string path;
    }

    // To automatically scroll to the bottom of Log
    public static class AutoScrollHelper
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollHelper), new PropertyMetadata(false, AutoScrollPropertyChanged));


        public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;
            if (scrollViewer == null) return;

            if ((bool)args.NewValue)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                scrollViewer.ScrollToEnd();
            }
            else
            {
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange > 0 || e.ExtentHeightChange < 0)
            {
                var scrollViewer = sender as ScrollViewer;
                scrollViewer?.ScrollToEnd();
            }
        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }
    }
}
