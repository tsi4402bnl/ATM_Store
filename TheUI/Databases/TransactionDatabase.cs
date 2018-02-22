using System.Windows.Threading;

namespace TheUI
{
    /*
     *             // timestamp = new Dictionary<string, string>{ {".sv", "timestamp"} }
            //"1519309008111"
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddMilliseconds(1519309008111).ToLocalTime();
            Log(epoch.ToLongDateString() + " & " + epoch.ToLongTimeString());
            //
    public class TransactionPropEntry : PropEntry<ITransactionPropEntryFb>
    {
        public TransactionPropEntry() : this("") { }
        public TransactionPropEntry(string id) : base(id)
        {
            Init(new TransactionPropEntryFb(itemId: "", itemName: "", singleItemPrice: 1.0, qty: 1));
        }
        public TransactionPropEntry(string id, ITransactionPropEntryFb entry) : base(id)
        {
            Init(entry);
        }
        public TransactionPropEntryFb GetPropEntryFb()
        {
            return new TransactionPropEntryFb(Item.id.Value, Category.Id.Value, Description.Value,
                Price.Value, Qty.Value, Units.Value, Supplier.Id.Value, Image.FbData);
        }

        public ObservableString Name { get; set; }
        public CategoryPropEntry Category { get { return _category; } set { _category = value; OnPropertyChanged("Category"); } }
        public ObservableString Description { get; set; }
        public ObservableDouble Price
        {
            get { if (_price.Value < 0.01) _price.Value = 0.01; return _price; }
            set { _price = value; if (_price.Value < 0.01) _price.Value = 0.01; }
        }
        public ObservableInt Qty
        {
            get { if (_qty.Value < 1) _qty.Value = 1; return _qty; }
            set { _qty = value; if (_qty.Value < 1) _qty.Value = 1; }
        }
        public ObservableString Units { get; set; }
        public SupplierPropEntry Supplier { get { return _supplier; } set { _supplier = value; OnPropertyChanged("Supplier"); } }
        public FbImage Image { get { return _image; } set { _image = value; OnPropertyChanged("Image"); } }

        protected override void Init(ITransactionPropEntryFb entry)
        {
            Name = new ObservableString(entry.Name);
            Category = new CategoryPropEntry(entry.CategoryId);
            Description = new ObservableString(entry.Description);
            Price = new ObservableDouble(entry.Price);
            Qty = new ObservableInt(entry.Qty);
            Units = new ObservableString(entry.Units);
            Supplier = new SupplierPropEntry(entry.SupplierId);
            Image = new FbImage(entry.Image);
        }

        private ObservableDouble _price;
        private ObservableInt _qty;
        private CategoryPropEntry _category;
        private SupplierPropEntry _supplier;
        private FbImage _image;
    }

    public interface ITransactionPropEntryFb
    {
        string ItemId { get; set; }
        string ItemName { get; set; } // the name of item when it was bought
        double SingleItemPrice { get; set; } // The price of item when it was bought
        int Qty { get; set; }
    }

    public class TransactionPropEntryFb : ITransactionPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public TransactionPropEntryFb(string itemId, string itemName, double singleItemPrice, int qty)
        {
            ItemId = itemId; ItemName = itemName; SingleItemPrice = singleItemPrice; Qty = qty;
        }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public double SingleItemPrice { get; set; }
        public int Qty { get; set; }
    }

    public class TransactionDatabase : Database<TransactionPropEntry, ITransactionPropEntryFb>
    {
        public TransactionDatabase(Dispatcher d, CategoryDatabase categoryDb, SupplierDatabase supplierDb) : base(d)
        {
            this.categoryDb = categoryDb;
            this.supplierDb = supplierDb;
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
                        if (entry.Name.Length != 0) Data[i].Name.Value = entry.Name.Substring(1);
                        if (entry.CategoryId.Length != 0)
                        {
                            Data[i].Category = categoryDb.GetEntryById(entry.CategoryId.Substring(1));
                        }
                        if (entry.Description.Length != 0)
                            Data[i].Description.Value = entry.Description.Substring(1);
                        if (entry.Price >= 0) Data[i].Price.Value = entry.Price;
                        if (entry.Qty >= 0) Data[i].Qty.Value = entry.Qty;
                        if (entry.Units.Length != 0) Data[i].Units.Value = entry.Units.Substring(1);
                        if (entry.SupplierId.Length != 0)
                        {
                            Data[i].Supplier = supplierDb.GetEntryById(entry.SupplierId.Substring(1));
                        }
                        if (entry.Image.Length != 0)
                            Data[i].Image = new FbImage(entry.Image.Substring(1));
                    }
                }
            });
        }

        internal bool Contain(SupplierPropEntry selectedItem)
        {
            foreach (var item in Data)
            {
                if (item.Supplier.Equals(selectedItem)) return true;
            }
            return false;
        }

        private CategoryDatabase categoryDb;
        private SupplierDatabase supplierDb;
    }//*/
}
