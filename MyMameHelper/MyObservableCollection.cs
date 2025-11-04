using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMameHelper.ContTable;

namespace MyMameHelper
{
    public class MyObservableCollection<T> : ObservableCollection<T>
    {
        public List<T> ChangeContent
        {
            set
            {
                this.Clear();
                foreach (T element in value)
                    this.Add(element);
            }
        }

        /*
        public void AddRange(ObservableCollection<T> other)
        {
            foreach (T element in other)
                this.Add(element);
        }
        public void AddRange(List<T> other)
        {
            foreach (T element in other)
                this.Add(element);
        }*/
        public void AddSilent(T element)
        {
            Items.Add(element);
        }

        public void RemoveSilent(T element)
        {
            Items.Remove(element);
        }

        public void SignalChange()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddSilentRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (T element in collection)
                Items.Add(element);


            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }


        internal void RemoveSilentRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");


            foreach (T element in collection)
                Items.Remove(element);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }


        /// <summary>
        /// Reorganise les items de la collection
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="direction"></param>
        /// <remarks>Ne génère pas d'autre collection</remarks>
        internal void Sort<TKey>(Func<T, TKey> keySelector, ListSortDirection direction)
        {
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    {
                        ApplySort(Items.OrderBy(keySelector));
                        break;
                    }
                case ListSortDirection.Descending:
                    {
                        ApplySort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
            /*
            ObservableCollection<T> tempo = new ObservableCollection<T>(this);
            tempo = this.OrderBy()


            throw new NotImplementedException();*/
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            ApplySort(Items.OrderBy(keySelector, comparer));
        }

        private void ApplySort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }

        // Convertisseur implicite
       /* public static implicit operator MyObservableCollection<T>(List<T> v)
        {
            MyObservableCollection<T> collection = new MyObservableCollection<T>();
            foreach (var c in v)
            {
                collection.Add(c);
            }

            return collection;
        }*/
    }
}
