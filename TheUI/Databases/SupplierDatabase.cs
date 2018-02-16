using System;
using System.Windows.Threading;

namespace TheUI
{
    public class SupplierPropEntry : PropEntry<SupplierPropEntryFb>
    {
        public SupplierPropEntry() : this("") { }
        public SupplierPropEntry(string id) : base(id)
        {
            Init(new SupplierPropEntryFb(name: "other", email:""));
        }
        public SupplierPropEntry(string id, SupplierPropEntryFb entry) : base(id)
        {
            Init(entry);
        }
        public SupplierPropEntry(SupplierPropEntry entry) : base(entry.Id.Value)
        {
            Init(entry.GetPropEntryFb());
        }
        public SupplierPropEntryFb GetPropEntryFb()
        {
            return new SupplierPropEntryFb(Name.Value, Email.Value);
        }

        public ObservableString Name  { get; set; }
        public ObservableString Email { get; set; }

        protected override void Init(SupplierPropEntryFb entry)
        {
            Name  = new ObservableString(entry.Name);
            Email = new ObservableString(entry.Email);
        }
    }

    public class SupplierPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public SupplierPropEntryFb(string name, string email) { Name = name; Email = email; }
        public string Name  { get; set; }
        public string Email { get; set; }
    }

    public class SupplierDatabase : Database<SupplierPropEntry, SupplierPropEntryFb>
    {
        public SupplierDatabase(Dispatcher d) : base(d)
        {
            DataView.Filter = new Predicate<object>(FilterData);
        }

        public override void AddProperties(string id, SupplierPropEntryFb entry)
        {
            dispatcher.Invoke(delegate
            {
                if (uniqueIds.Add(id))
                {
                    Data.Add(new SupplierPropEntry(id));
                }

                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].Id.Value == id)
                    {
                        if (entry.Name.Length  != 0) Data[i].Name.Value = entry.Name.Substring(1);
                        if (entry.Email.Length != 0) Data[i].Email.Value = entry.Email.Substring(1);
                    }
                }
            });
        }

        public bool FilterData(object item)
        {
            if (item != null && ((SupplierPropEntry)item).Id.Value.Length > 0)
                return true;
            return false;
        }
    }

}
