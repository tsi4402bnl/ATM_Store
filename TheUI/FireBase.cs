using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TheUI
{
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

    public class FireBase
    {
        public FireBase(Dispatcher d)
        {
            dispatcher = d;
            fbReceivedMessages = new Queue<FbEventData>();

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "YL6Bb5ejs59R4f8xFRLzcVcrjXu8klIPPKeAxHbv",
                BasePath = "https://atm-store.firebaseio.com/"
            };
            fbClient = new FirebaseClient(config);
            Init();
        }

        public async void InsertInFb<T>(string tableName, T data)
        {
            await fbClient.PushAsync(tableName, data);
        }

        public async void ModifyInFb<T>(string tableName, string id, T data)
        {
            await fbClient.UpdateAsync(tableName + "/" + id, data);
        }

        public async void DeleteFromFb(string tableName, string id)
        {
            await fbClient.DeleteAsync(tableName + "/" + id);
        }

        public bool IsFbMessagePending()
        {
            return fbReceivedMessages.Count > 0;
        }
        public FbEventData FetchNextFbMessage()
        {
            return fbReceivedMessages.Dequeue();
        }

        public Queue<FbEventData> fbReceivedMessages; // holds fb mesaages which needs to be parsed by C++

        private async void Init()
        {
            ValueAddedEventHandler ItemAdded = GetFbItemAdded();
            ValueChangedEventHandler ItemChanged = GetFbItemChanged();
            ValueRemovedEventHandler ItemRemoved = GetFbItemRemoved();
            // fetches fb events
            await fbClient.OnAsync("", ItemAdded, ItemChanged, ItemRemoved);
        }

        private ValueAddedEventHandler GetFbItemAdded()
        {
            return (s, args, c) =>
            {
                dispatcher.InvokeAsync(() =>
                {
                    fbReceivedMessages.Enqueue(new FbEventData() { operation = Fb_Operations.fb_add, data = args.Data, path = args.Path } );
                }, DispatcherPriority.Normal);
            };
        }

        private ValueChangedEventHandler GetFbItemChanged()
        {
            return (s, args, c) =>
            {
                dispatcher.InvokeAsync(() =>
                {
                    fbReceivedMessages.Enqueue(new FbEventData() { operation = Fb_Operations.fb_edit, data = args.Data, path = args.Path });
                }, DispatcherPriority.Normal);
            };
        }

        private ValueRemovedEventHandler GetFbItemRemoved()
        {
            return (s, args, c) =>
            {
                dispatcher.InvokeAsync(() =>
                {
                    fbReceivedMessages.Enqueue(new FbEventData() { operation = Fb_Operations.fb_delete, data = "", path = args.Path });
                }, DispatcherPriority.Normal);
            };
        }

        private FirebaseClient fbClient;
        private Dispatcher dispatcher;
    }
}
