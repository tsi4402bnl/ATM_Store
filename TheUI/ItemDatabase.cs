using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TheUI
{
    public class ItemPropEntry : PropertyChangedBase
    {
        public ItemPropEntry()
        {
            Init(id: "", item: new ItemPropEntryFb(name: "", category: 0, description: "", price: 1.0, 
                qtyPerBox: 1, units: "pcs", supplierId: ""));
        }
        public ItemPropEntry(string id, ItemPropEntryFb item)
        {
            Init(id, item);
        }
        public ItemPropEntryFb GetItemPropEntryFb()
        {
            return new ItemPropEntryFb(Name.Value, Category.Value, Description.Value, 
                Price.Value, QtyPerBox.Value, Units.Value, SupplierId.Value);
        }

        public ObservableString Id          { get; set; }
        public ObservableString Name        { get; set; }
        public ObservableInt    Category    { get; set; }
        public ObservableString Description { get; set; }
        public ObservableDouble Price       { get; set; }
        public ObservableInt    QtyPerBox   { get; set; }
        public ObservableString Units       { get; set; }
        public ObservableString SupplierId  { get; set; }

        private void Init(string id, ItemPropEntryFb item)
        {
            Id          = new ObservableString() { Value = id               };
            Name        = new ObservableString() { Value = item.Name        };
            Category    = new ObservableInt()    { Value = item.Category    };
            Description = new ObservableString() { Value = item.Description };
            Price       = new ObservableDouble() { Value = item.Price       };
            QtyPerBox   = new ObservableInt()    { Value = item.QtyPerBox   };
            Units       = new ObservableString() { Value = item.Units       };
            SupplierId  = new ObservableString() { Value = item.SupplierId  };
        }
    }

    public class ItemPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public ItemPropEntryFb(string name, int category, string description, double price, 
            int qtyPerBox, string units, string supplierId)
        { Name = name; Category = category; Description = description; Price = price;
            QtyPerBox = qtyPerBox; Units = units; SupplierId = supplierId; }
        public string Name        { get; set; }
        public int    Category    { get; set; }
        public string Description { get; set; }
        public double Price       { get; set; }
        public int    QtyPerBox   { get; set; }
        public string Units       { get; set; }
        public string SupplierId  { get; set; }
    }

    public class ItemDatabase
    {
        public ItemDatabase(Dispatcher d)
        {
            dispatcher = d;;
            Data = new ObservableCollection<ItemPropEntry>(); ;
            uniqueIds = new HashSet<string>();
        }

        public void AddItemProperties(string id, ItemPropEntryFb item)
        {
            dispatcher.Invoke(delegate
            {
                if (uniqueIds.Add(id))
                {
                    Data.Add(new ItemPropEntry(id, item));
                }
                else
                {
                    for (int i = 0; i < Data.Count; i++)
                    {
                        if (Data[i].Id.Value == id)
                        {
                            if (item.Name.Length != 0) Data[i].Name.Value = item.Name;
                            if (item.Category >= 0) Data[i].Category.Value = item.Category; // TODO: lookup for actual category name
                            if (item.Description.Length != 0) Data[i].Description.Value = item.Description;
                            if (item.Price >= 0) Data[i].Price.Value = item.Price;
                            if (item.QtyPerBox >= 0) Data[i].QtyPerBox.Value = item.QtyPerBox;
                            if (item.Units.Length != 0) Data[i].Units.Value = item.Units;
                            if (item.SupplierId.Length != 0) Data[i].SupplierId.Value = item.SupplierId; // TODO: lookup for actual supplier name
                        }
                    }
                }
            });
        }

        public void RemoveItemProperties(string id)
        {
            dispatcher.Invoke(delegate
            {
                if (uniqueIds.Remove(id))
                {
                    for (int i = 0; i < Data.Count; i++)
                    {
                        if (Data[i].Id.Value == id)
                        {
                            Data.RemoveAt(i);
                            break;
                        }
                    }
                }
            });
        }

        public ObservableCollection<ItemPropEntry> Data { get; set; }

        private HashSet<string> uniqueIds; // fb item table entry ids, 
        private Dispatcher dispatcher;
    }
}
