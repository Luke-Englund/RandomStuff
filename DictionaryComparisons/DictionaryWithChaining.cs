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
        
    }
}