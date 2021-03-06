﻿using System.Windows.Threading;

namespace TheUI
{

    public class CategoryPropEntry : PropEntry<ICategoryPropEntryFb>
    {
        public CategoryPropEntry() : this("") { }
        public CategoryPropEntry(string id) : base(id)
        {
            Init(new CategoryPropEntryFb(name: "other"));
        }
        public CategoryPropEntry(string id, ICategoryPropEntryFb entry) : base(id)
        {
            Init(entry);
        }
        public ICategoryPropEntryFb GetPropEntryFb()
        {
            return new CategoryPropEntryFb(Name.Value);
        }

        public ObservableString Name { get; set; }

        protected override void Init(ICategoryPropEntryFb entry)
        {
            Name = new ObservableString(entry.Name);
        }
    }

    public interface ICategoryPropEntryFb
    {
        string Name { get; }
    }

    public class CategoryPropEntryFb : ICategoryPropEntryFb // for Fb class is not allowed to contain subClasses, keep it clean
    {
        public CategoryPropEntryFb(string name) { Name = name; }
        public string Name { get; }
    }

    public class CategoryDatabase : Database<CategoryPropEntry, ICategoryPropEntryFb>
    {
        public CategoryDatabase(Dispatcher d) : base(d) { }

        public override void AddProperties(string id, ICategoryPropEntryFb entry)
        {
            dispatcher.Invoke(delegate
            {
                if (uniqueIds.Add(id))
                {
                    Data.Add(new CategoryPropEntry(id));
                }

                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].Id.Value == id)
                    {
                        if (entry.Name.Length != 0) Data[i].Name.Value = entry.Name.Substring(1);
                    }
                }
            });
        }
    }
}
