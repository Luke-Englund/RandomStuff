using System;
using System.Collections.Generic;

namespace DictionaryComparisons.Implementation
{
    public struct KeyValuePair<TKey, TVal>
    {
        public TKey Key { get; set; }
        public TVal Val { get; set; }
    }

    public class DictionaryWithChaining<TKey, TVal>
    {
        // The threshold for the dictionary's load
        private const float LOAD_THRESHOLD = 0.85f;

        // The storage capacity before resizing
        private int _capacity = 14;

        // The number of elements currently stored
        private int _numElems = 0;

        // The central storage data structure
        private List<KeyValuePair<TKey, TVal>>[] _dataStore;

        // A constructor that uses all default initialization parameters
        public DictionaryWithChaining()
        {
            Init();
        }

        // A constructor that uses a user determined initial storage size
        public DictionaryWithChaining(int capacity)
        {
            if (capacity < 1) throw new ArgumentNullException("capacity");

            _capacity = capacity;
            Init();
        }

        // Initializes the data structure
        private void Init()
        {
            var storageSize = (int)(_capacity / LOAD_THRESHOLD) + 1;
            _dataStore = new List<KeyValuePair<TKey, TVal>>[storageSize];
        }

        // Insert a key value pair
        public void Insert(TKey key, TVal val)
        {
            // Null check the key
            if (key == null) throw new ArgumentNullException("key");

            // Hash the key to retrieve the index
            var index = key.GetHashCode() % _capacity;

            // Ensure there's a list there to add to
            _dataStore[index] = _dataStore[index] ?? new List<KeyValuePair<TKey,TVal>>();

            // Need to ensure that the key hasn't already been inserted.
            foreach(var elem in _dataStore[index])
            {
                if (elem.Key.Equals(key)) throw new ArgumentException("An element with that key already exists in this set.");
            }

            // Insert the key
            _dataStore[index].Add(new KeyValuePair<TKey, TVal>() { Key = key, Val = val });

            // Increment the number of elements and resize if necessary.
            if (++_numElems > _capacity) Resize();
        }

        // Delete a key and its corresponding value if it exists
        public bool Delete(TKey key)
        {
            // Null check the key
            if (key == null) throw new ArgumentNullException("key");

            // Hash the key to retrieve the index
            var index = key.GetHashCode() % _capacity;

            // If nothing has been added to this bucket then key has not been added
            if (_dataStore[index] == null) return false;

            // Otherwise search for the corresponding key/value pair
            var elemFound = _dataStore[index].Find(x => x.Key.Equals(key));

            // If you found it then remove it
            if (!elemFound.Equals(default(KeyValuePair<TKey, TVal>)))
            {
                _dataStore[index].Remove(elemFound);
                return true;
            }

            // Uh oh, you didn't find it.  Let the user know.
            return false;
        }

        // Lookup a value based upon its key
        public TVal Lookup(TKey key)
        {
            // Null check the key
            if (key == null) throw new ArgumentNullException("key");

            // Hash the key to retrieve the index
            var index = key.GetHashCode() % _capacity;

            // If nothing has been added to this bucket then key has not been added
            if (_dataStore[index] == null) throw new ArgumentException("There is no element corresponding to this key.");

            // Otherwise search for the corresponding key/value pair
            foreach (var elem in _dataStore[index])
            {
                if (elem.Key.Equals(key)) return elem.Val;
            }

            // Uh oh, you didn't find it.  Let the user know.
            throw new ArgumentException("There is no element corresponding to this key.");
        }

        // Determines if a key exists in the current set
        public bool ContainsKey(TKey key)
        {
            // Null check the key
            if (key == null) throw new ArgumentNullException("key");

            // Hask the key to retreive index
            var index = key.GetHashCode() % _capacity;

            // If nothing has been added to this bucket then key has not been added
            if (_dataStore[index] == null) return false;

            // Otherwise search for the corresponding key/value pair
            foreach (var elem in _dataStore[index])
            {
                if (elem.Key.Equals(key)) return true;
            }

            // Uh oh, you didn't find it.
            return false;
        }

        // A get/set operator
        public TVal this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        // When the load gets too large increase size.
        // Ignoring potential space decreasing opportunities.
        private void Resize()
        {
            var newMinCapacity = _capacity << 1;

            var newStorageSize = (int)(newMinCapacity / LOAD_THRESHOLD) + 1;
            _capacity = (int)((newStorageSize - 1) * LOAD_THRESHOLD);

            var newDataStore = new List<KeyValuePair<TKey, TVal>>[newStorageSize];
        }

        // Returns the next greatest prime for storage size
        private int GetNextPrime(int val)
        {
            return val++;
        }
    }
}