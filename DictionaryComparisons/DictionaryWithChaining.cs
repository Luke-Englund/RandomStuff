using System;

namespace DictionaryComparisons.Implementation
{
    public struct KeyValuePair<TKey, TVal>
    {
        public TKey Key { get; set; }
        public TVal Value { get; set; }
    }

    public class DictionaryWithChaining<TKey, TVal>
    {
        // The initial size of the array 
        const int INITIAL_SIZE = 16;
        // The loading percentage before resizing
        const float LOADING_LIMIT_PRCNT = 0.90f;

        // Table that holds the lists of key/value pairs
        private List<KeyValuePair<TKey, TVal>>[] _table;

        private int _arraySize = -1;

        public DictionaryWithChaining()
        {
            _arraySize = INITIAL_SIZE;
            
            // Check that key type is IEquatable and Hashable
            Init();
        }

        private void Init()
        {
            table = new List<KeyValuePair<TKey, TVal>>[_arraySize];
        }

        // Takes a key and provides array position
        private int GetArrayPosition(TKey key)
        {
            return 
        }
    }
}