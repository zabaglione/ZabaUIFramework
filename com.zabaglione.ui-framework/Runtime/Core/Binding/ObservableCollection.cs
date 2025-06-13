using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace jp.zabaglione.ui.core.binding
{
    /// <summary>
    /// A collection that provides notifications when items are added, removed, or the entire list is refreshed
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection</typeparam>
    public class ObservableCollection<T> : IList<T>, INotifyCollectionChanged
    {
        private readonly List<T> _items;

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the number of elements in the collection
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the element at the specified index
        /// </summary>
        public T this[int index]
        {
            get => _items[index];
            set
            {
                if (index < 0 || index >= _items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                T oldItem = _items[index];
                _items[index] = value;

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class
        /// </summary>
        public ObservableCollection()
        {
            _items = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class that contains elements copied from the specified collection
        /// </summary>
        public ObservableCollection(IEnumerable<T> collection)
        {
            _items = new List<T>(collection ?? throw new ArgumentNullException(nameof(collection)));
        }

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        public void Add(T item)
        {
            int index = _items.Count;
            _items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Adds multiple items to the collection
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var itemsList = new List<T>(items);
            if (itemsList.Count == 0)
                return;

            int startIndex = _items.Count;
            _items.AddRange(itemsList);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, itemsList, startIndex));
        }

        /// <summary>
        /// Removes all items from the collection
        /// </summary>
        public void Clear()
        {
            if (_items.Count == 0)
                return;

            _items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Determines whether the collection contains a specific value
        /// </summary>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection
        /// </summary>
        public bool Remove(T item)
        {
            int index = _items.IndexOf(item);
            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index
        /// </summary>
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item at the specified index
        /// </summary>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > _items.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _items.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Removes the item at the specified index
        /// </summary>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _items.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            T removedItem = _items[index];
            _items.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        /// <summary>
        /// Moves an item from one index to another
        /// </summary>
        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _items.Count)
                throw new ArgumentOutOfRangeException(nameof(oldIndex));
            if (newIndex < 0 || newIndex >= _items.Count)
                throw new ArgumentOutOfRangeException(nameof(newIndex));

            if (oldIndex == newIndex)
                return;

            T item = _items[oldIndex];
            _items.RemoveAt(oldIndex);
            _items.Insert(newIndex, item);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        /// <summary>
        /// Replaces all items in the collection with the specified collection
        /// </summary>
        public void Reset(IEnumerable<T> newItems)
        {
            _items.Clear();
            if (newItems != null)
            {
                _items.AddRange(newItems);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Sorts the collection using the default comparer
        /// </summary>
        public void Sort()
        {
            _items.Sort();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Sorts the collection using the specified comparison
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            _items.Sort(comparison);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Sorts the collection using the specified comparer
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            _items.Sort(comparer);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Returns a read-only wrapper for the current collection
        /// </summary>
        public IReadOnlyList<T> AsReadOnly()
        {
            return _items.AsReadOnly();
        }

        /// <summary>
        /// Raises the CollectionChanged event
        /// </summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Notifies listeners of dynamic changes to a collection
    /// </summary>
    public interface INotifyCollectionChanged
    {
        /// <summary>
        /// Occurs when the collection changes
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    /// <summary>
    /// Represents the method that handles the CollectionChanged event
    /// </summary>
    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

    /// <summary>
    /// Provides data for the CollectionChanged event
    /// </summary>
    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the action that caused the event
        /// </summary>
        public NotifyCollectionChangedAction Action { get; }

        /// <summary>
        /// Gets the list of new items involved in the change
        /// </summary>
        public IList NewItems { get; }

        /// <summary>
        /// Gets the list of items affected by a Replace, Remove, or Move action
        /// </summary>
        public IList OldItems { get; }

        /// <summary>
        /// Gets the index at which the change occurred
        /// </summary>
        public int NewStartingIndex { get; }

        /// <summary>
        /// Gets the index at which a Move, Remove, or Replace action occurred
        /// </summary>
        public int OldStartingIndex { get; }

        /// <summary>
        /// Initializes a new instance for a Reset action
        /// </summary>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            if (action != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("This constructor can only be used for Reset actions");

            Action = action;
            NewStartingIndex = -1;
            OldStartingIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance for single item actions
        /// </summary>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            Action = action;
            
            if (action == NotifyCollectionChangedAction.Add)
            {
                NewItems = new[] { changedItem };
                NewStartingIndex = index;
                OldStartingIndex = -1;
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                OldItems = new[] { changedItem };
                OldStartingIndex = index;
                NewStartingIndex = -1;
            }
            else
            {
                throw new ArgumentException("This constructor can only be used for Add or Remove actions");
            }
        }

        /// <summary>
        /// Initializes a new instance for multi-item actions
        /// </summary>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            Action = action;
            
            if (action == NotifyCollectionChangedAction.Add)
            {
                NewItems = changedItems;
                NewStartingIndex = startingIndex;
                OldStartingIndex = -1;
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                OldItems = changedItems;
                OldStartingIndex = startingIndex;
                NewStartingIndex = -1;
            }
            else
            {
                throw new ArgumentException("This constructor can only be used for Add or Remove actions");
            }
        }

        /// <summary>
        /// Initializes a new instance for Replace actions
        /// </summary>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            if (action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException("This constructor can only be used for Replace actions");

            Action = action;
            NewItems = new[] { newItem };
            OldItems = new[] { oldItem };
            NewStartingIndex = index;
            OldStartingIndex = index;
        }

        /// <summary>
        /// Initializes a new instance for Move actions
        /// </summary>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int newIndex, int oldIndex)
        {
            if (action != NotifyCollectionChangedAction.Move)
                throw new ArgumentException("This constructor can only be used for Move actions");

            Action = action;
            NewItems = new[] { changedItem };
            OldItems = new[] { changedItem };
            NewStartingIndex = newIndex;
            OldStartingIndex = oldIndex;
        }
    }

    /// <summary>
    /// Describes the action that caused a CollectionChanged event
    /// </summary>
    public enum NotifyCollectionChangedAction
    {
        /// <summary>
        /// One or more items were added to the collection
        /// </summary>
        Add,

        /// <summary>
        /// One or more items were removed from the collection
        /// </summary>
        Remove,

        /// <summary>
        /// One or more items were replaced in the collection
        /// </summary>
        Replace,

        /// <summary>
        /// One or more items were moved within the collection
        /// </summary>
        Move,

        /// <summary>
        /// The content of the collection changed dramatically
        /// </summary>
        Reset
    }
}