using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace TheUI
{
    public abstract class PropEntry<EntryFbType> : PropertyChangedBase, System.IEquatable<PropEntry<EntryFbType>>
    {
        public PropEntry(string id) { Id = new ObservableString() { Value = id }; }
        public ObservableString Id { get; set; }

        public bool Equals(PropEntry<EntryFbType> other)
        {
            return Id.Value == other.Id.Value;
        }

        protected abstract void Init(EntryFbType entry);
    }

    public abstract class Database<EntryType, EntryFbType> where EntryType : PropEntry<EntryFbType>, new()
    {
        public Database(Dispatcher d)
        {
            dispatcher = d; ;
            Data = new ObservableCollection<EntryType>();
            uniqueIds = new HashSet<string>();
        }

        public abstract void AddProperties(string id, EntryFbType entry);

        public void RemoveProperties(string id)
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

        public EntryType GetEntryById(string id)
        {
            if (uniqueIds.Add(id))
            {
                Data.Add(new EntryType()
                {
                    Id = new ObservableString { Value = id },
                });
            }

            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Id.Value == id)
                {
                    return Data[i];
                }
            }

            return new EntryType(); // this should never execute
        }

        public ObservableCollection<EntryType> Data { get; set; }

        protected HashSet<string> uniqueIds; // fb item table entry ids, 
        protected Dispatcher dispatcher;
    }
}
