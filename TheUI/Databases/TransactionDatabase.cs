using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace TheUI
{
    public class TransactionPropEntry : PropEntry<ITransactionPropEntryFb>
    {
        public TransactionPropEntry() : this("") { }
        public TransactionPropEntry(string id) : base(id)
        {
            Init(new TransactionPropEntryFb(itemId: "", itemName: "", singleItemPrice: 1.0, qty: 1, epochTime: GetTimeSpan(DateTime.UtcNow)));
        }
        public TransactionPropEntry(string id, ITransactionPropEntryFb entry) : base(id)
        {
            Init(entry);
        }
        public TransactionPropEntryFb GetPropEntryFb()
        {
            return new TransactionPropEntryFb(Item.Id.Value, ItemName.Value, SingleItemPrice.Value, Qty.Value, GetTimeSpan(TransactionTime.Value));
        }

        public ItemPropEntry Item { get { return _item; } set { _item = value; OnPropertyChanged("Item"); } }
        public ObservableString ItemName { get; set; }
        public ObservableDouble SingleItemPrice
        {
            get { if (_singleItemPrice.Value < 0.01) _singleItemPrice.Value = 0.01; return _singleItemPrice; }
            set { _singleItemPrice = value; if (_singleItemPrice.Value < 0.01) _singleItemPrice.Value = 0.01; }
        }
        public ObservableInt Qty
        {
            get { if (_qty.Value < 1) _qty.Value = 1; return _qty; }
            set { _qty = value; if (_qty.Value < 1) _qty.Value = 1; }
        }
        public ObservableDateTime TransactionTime { get; set; }

        protected override void Init(ITransactionPropEntryFb entry)
        {
            Item = new ItemPropEntry(entry.ItemId);
            ItemName = new ObservableString(entry.ItemName);
            SingleItemPrice = new ObservableDouble(entry.SingleItemPrice);
            Qty = new ObservableInt(entry.Qty);
            SetTransactionTime(entry.EpochTime);
        }
        public void SetTransactionTime(double msSinceEpoch)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddMilliseconds(msSinceEpoch).ToLocalTime();
            TransactionTime = new ObservableDateTime(epoch);
        }
        private double GetTimeSpan(DateTime till)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return (till - epoch).TotalMilliseconds;
        }

        private ItemPropEntry _item;
        private ObservableDouble _singleItemPrice;
        private ObservableInt _qty;
    }

    public interface ITransactionPropEntryFb
    {
        string ItemId          { get; }
        string ItemName        { get; } // the name of item when it was bought
        double SingleItemPrice { get; } // The price of item when it was bought
        int    Qty             { get; }
        double EpochTime       { get; } // Retrieved server time from firebase
        Dictionary<string, string> Timestamp { get; } // constant command for firebase to store server time
    }

    public class TransactionPropEntryFb : ITransactionPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public TransactionPropEntryFb(string itemId, string itemName, double singleItemPrice, int qty, double epochTime = -1.0)
        {
            ItemId = itemId; ItemName = itemName; SingleItemPrice = singleItemPrice; Qty = qty; EpochTime = epochTime;
            Timestamp = new Dictionary<string, string> { { ".sv", "timestamp" } };
        }
        public string ItemId          { get; }
        public string ItemName        { get; }
        public double SingleItemPrice { get; }
        public int    Qty             { get; }
        public double EpochTime       { get; }
        public Dictionary<string, string> Timestamp { get; }
    }

    public class TransactionDatabase : Database<TransactionPropEntry, ITransactionPropEntryFb>
    {
        public TransactionDatabase(Dispatcher d, ItemDatabase itemDb) : base(d)
        {
            this.itemDb = itemDb;
        }

        public override void AddProperties(string id, ITransactionPropEntryFb entry)
        {
            dispatcher.Invoke(delegate
            {
                if (uniqueIds.Add(id))
                {
                    Data.Add(new TransactionPropEntry(id));
                }

                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].Id.Value == id)
                    {
                        if (entry.ItemId.Length != 0)
                        {
                            Data[i].Item = itemDb.GetEntryById(entry.ItemId.Substring(1));
                        }
                        if (entry.ItemName.Length != 0) Data[i].ItemName.Value = entry.ItemName.Substring(1);
                        if (entry.SingleItemPrice >= 0) Data[i].SingleItemPrice.Value = entry.SingleItemPrice;
                        if (entry.Qty >= 0) Data[i].Qty.Value = entry.Qty;
                        if (entry.EpochTime >= 0) Data[i].SetTransactionTime(entry.EpochTime);
                    }
                }
            });
        }

        public bool Contain(ItemPropEntry selectedItem)
        {
            foreach (var item in Data)
            {
                if (item.Item.Equals(selectedItem)) return true;
            }
            return false;
        }

        private ItemDatabase itemDb;
    }
}
