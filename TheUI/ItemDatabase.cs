using System.Windows.Threading;

namespace TheUI
{
    public class ItemPropEntry : PropEntry<ItemPropEntryFb>
    {
        public ItemPropEntry() : base("")
        {
            Init(entry: new ItemPropEntryFb(name: "", categoryId: "", description: "", price: 1.0, 
                qtyPerBox: 1, units: "pcs", supplierId: ""));
        }
        public ItemPropEntry(string id, ItemPropEntryFb entry) : base(id)
        {
            Init(entry);
        }
        public ItemPropEntry(ItemPropEntry entry) : base(entry.Id.Value)
        {
            Init(entry.GetPropEntryFb());
            Category.Name.Value = entry.Category.Name.Value;
        }
        public ItemPropEntryFb GetPropEntryFb()
        {
            return new ItemPropEntryFb(Name.Value, Category.Id.Value, Description.Value, 
                Price.Value, QtyPerBox.Value, Units.Value, SupplierId.Value);
        }

        public ObservableString  Name        { get; set; }
        public CategoryPropEntry Category    { get { return _category; } set { _category = value; OnPropertyChanged("Category"); } }
        public ObservableString  Description { get; set; }
        public ObservableDouble  Price       { get; set; }
        public ObservableInt     QtyPerBox   { get; set; }
        public ObservableString  Units       { get; set; }
        public ObservableString  SupplierId  { get; set; }

        protected override void Init(ItemPropEntryFb entry)
        {
            Name        = new ObservableString() { Value = entry.Name        };
            Category    = new CategoryPropEntry()
            {
                Id   = new ObservableString() { Value = entry.CategoryId },
                Name = new ObservableString()
            };
            Description = new ObservableString() { Value = entry.Description };
            Price       = new ObservableDouble() { Value = entry.Price       };
            QtyPerBox   = new ObservableInt()    { Value = entry.QtyPerBox   };
            Units       = new ObservableString() { Value = entry.Units       };
            SupplierId  = new ObservableString() { Value = entry.SupplierId  };
        }

        private CategoryPropEntry _category;
    }

    public class ItemPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public ItemPropEntryFb(string name, string categoryId, string description, double price, 
            int qtyPerBox, string units, string supplierId)
        { Name = name; CategoryId = categoryId; Description = description; Price = price;
            QtyPerBox = qtyPerBox; Units = units; SupplierId = supplierId; }
        public string Name        { get; set; }
        public string CategoryId  { get; set; }
        public string Description { get; set; }
        public double Price       { get; set; }
        public int    QtyPerBox   { get; set; }
        public string Units       { get; set; }
        public string SupplierId  { get; set; }
    }

    public class ItemDatabase : Database<ItemPropEntry, ItemPropEntryFb>
    {
        public ItemDatabase(Dispatcher d, CategoryDatabase categoryDb) : base(d)
        {
            this.categoryDb = categoryDb;
        }

        public override void AddProperties(string id, ItemPropEntryFb entry)
        {
            dispatcher.Invoke(delegate
            {
                if (uniqueIds.Add(id))
                {
                    Data.Add(new ItemPropEntry(id, entry) { Category = categoryDb.GetEntryById(entry.CategoryId) });
                }
                else
                {
                    for (int i = 0; i < Data.Count; i++)
                    {
                        if (Data[i].Id.Value == id)
                        {
                            if (entry.Name.Length != 0) Data[i].Name.Value = entry.Name;
                            if (entry.CategoryId.Length != 0)
                            {
                                Data[i].Category = categoryDb.GetEntryById(entry.CategoryId); // TODO: lookup for actual category name
                            }
                            if (entry.Description.Length != 0)
                                Data[i].Description.Value = entry.Description;
                            if (entry.Price >= 0) Data[i].Price.Value = entry.Price;
                            if (entry.QtyPerBox >= 0) Data[i].QtyPerBox.Value = entry.QtyPerBox;
                            if (entry.Units.Length != 0) Data[i].Units.Value = entry.Units;
                            if (entry.SupplierId.Length != 0) Data[i].SupplierId.Value = entry.SupplierId; // TODO: lookup for actual supplier name
                        }
                    }
                }
            });
        }

        private CategoryDatabase categoryDb;
    }
}
