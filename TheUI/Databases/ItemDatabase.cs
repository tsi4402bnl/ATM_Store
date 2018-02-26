using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TheUI
{
    public class ItemPropEntry : PropEntry<IItemPropEntryFb>
    {
        public ItemPropEntry() : this("") { }
        public ItemPropEntry(string id) : base(id)
        {
            Init(new ItemPropEntryFb(name: "", categoryId: "", description: "", price: 1.0,
                qty: 1, units: "pcs", supplierId: "", image: ""));
        }
        public ItemPropEntry(string id, IItemPropEntryFb entry) : base(id)
        {
            Init(entry);
        }
        public ItemPropEntry(ItemPropEntry entry) : base(entry.Id.Value)
        {
            Init(entry.GetPropEntryFb());
            Category.Name.Value  = entry.Category.Name.Value;
            Supplier.Name.Value  = entry.Supplier.Name.Value;
            Supplier.Email.Value = entry.Supplier.Email.Value;
        }
        public ItemPropEntryFb GetPropEntryFb()
        {
            return new ItemPropEntryFb(Name.Value, Category.Id.Value, Description.Value, 
                Price.Value, Qty.Value, Units.Value, Supplier.Id.Value, Image.FbData);
        }

        public ObservableString  Name        { get; set; }
        public CategoryPropEntry Category    { get { return _category; } set { _category = value; OnPropertyChanged("Category"); } }
        public ObservableString  Description { get; set; }
        public ObservableDouble  Price
        {
            get { if (_price.Value < 0.01) _price.Value = 0.01; return _price; }
            set { _price = value; if (_price.Value < 0.01) _price.Value = 0.01; }
        }
        public ObservableInt     Qty
        {
            get { if (_qty.Value < 0) _qty.Value = 0; return _qty; }
            set { _qty = value; if (_qty.Value < 0) _qty.Value = 0; }
        }
        public ObservableString  Units       { get; set; }
        public SupplierPropEntry Supplier    { get { return _supplier; } set { _supplier = value; OnPropertyChanged("Supplier"); } }
        public FbImage           Image       { get { return _image;    } set { _image    = value; OnPropertyChanged("Image"   ); } }
        public ObservableBool    ShowInShop  { get; set; }

        protected override void Init(IItemPropEntryFb entry)
        {
            Name        = new ObservableString(entry.Name);
            Category    = new CategoryPropEntry(entry.CategoryId);
            Description = new ObservableString(entry.Description);
            Price       = new ObservableDouble(entry.Price);
            Qty         = new ObservableInt(entry.Qty);
            Units       = new ObservableString(entry.Units);
            Supplier    = new SupplierPropEntry(entry.SupplierId);
            Image       = new FbImage(entry.Image);
            ShowInShop  = new ObservableBool(true);
        }

        private ObservableDouble _price;
        private ObservableInt _qty;
        private CategoryPropEntry _category;
        private SupplierPropEntry _supplier;
        private FbImage _image;
    }

    public class FbImage
    {
        public FbImage(string fbData)
        {
            FbData = fbData;

            int numberChars = fbData.Length;
            if (numberChars > 0 && numberChars % 2 == 0)
            {
                byte[] data = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                    data[i / 2] = Convert.ToByte(fbData.Substring(i, 2), 16);

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = new MemoryStream(data);
                img.EndInit();

                Image = img;
            }
            else
            {
                Image = new BitmapImage();
            }
        }
        public ImageSource Image { get; set; }
        public string FbData    { get; set; }
    }

    public interface IItemPropEntryFb
    {
        string CategoryId  { get; }
        string Description { get; }
        string Name        { get; }
        double Price       { get; }
        int    Qty         { get; }
        string SupplierId  { get; }
        string Units       { get; }
        string Image       { get; }
    }

    public class ItemPropEntryFb : IItemPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public ItemPropEntryFb(string name, string categoryId, string description, double price, 
            int qty, string units, string supplierId, string image)
        { Name = name; CategoryId = categoryId; Description = description; Price = price;
            Qty = qty; Units = units; SupplierId = supplierId; Image = image; }
        public string Name        { get; }
        public string CategoryId  { get; }
        public string Description { get; }
        public double Price       { get; }
        public int    Qty         { get; }
        public string Units       { get; }
        public string SupplierId  { get; }
        public string Image       { get; }
    }

    public class ItemDatabase : Database<ItemPropEntry, IItemPropEntryFb>
    {
        public ItemDatabase(Dispatcher d, CategoryDatabase categoryDb, SupplierDatabase supplierDb) : base(d)
        {
            this.categoryDb = categoryDb;
            this.supplierDb = supplierDb;
            DataView.Filter = new Predicate<object>(FilterData);
            searchName = "";
            searchSupplier = "";
        }

        public override void AddProperties(string id, IItemPropEntryFb entry)
        {
            dispatcher.Invoke(delegate
            {
            if (uniqueIds.Add(id))
            {
                Data.Add(new ItemPropEntry(id));
            }

            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Id.Value == id)
                {
                    if (entry.Name.Length != 0)
                    {
                        Data[i].Name.Value = entry.Name.Substring(1);
                        if (searchName.Length > 0) DataView.Refresh();
                    }
                    if (entry.CategoryId.Length != 0)
                    {
                        Data[i].Category = categoryDb.GetEntryById(entry.CategoryId.Substring(1));
                    }
                    if (entry.Description.Length != 0)
                        Data[i].Description.Value = entry.Description.Substring(1);
                    if (entry.Price >= 0) Data[i].Price.Value = entry.Price;
                    if (entry.Qty >= 0)
                    {
                        Data[i].Qty.Value = entry.Qty;
                        Data[i].ShowInShop.Value = entry.Qty > 0;
                    }
                    if (entry.Units.Length != 0) Data[i].Units.Value = entry.Units.Substring(1);
                    if (entry.SupplierId.Length != 0)
                    {
                        Data[i].Supplier = supplierDb.GetEntryById(entry.SupplierId.Substring(1));
                        if (searchSupplier.Length > 0) DataView.Refresh();
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

        public bool FilterData(object obj)
        {
            if (obj == null) return false;

            ItemPropEntry item = (ItemPropEntry)obj;
            if (searchName.Length > 0 && !item.Name.Value.ToLower().Contains(searchName.ToLower())) return false;
            if (searchSupplier.Length > 0 && !item.Supplier.Id.Value.Contains(searchSupplier)) return false;

            return true;
        }

        public void SetSearchCriteria(string name, string supplierId)
        {
            searchName = name;
            searchSupplier = supplierId == "all" ? "" : supplierId; // to 
            DataView.Refresh();
        }

        private CategoryDatabase categoryDb;
        private SupplierDatabase supplierDb;
        private string searchName;
        private string searchSupplier;
    }
}
