using System;
using System.Collections.Generic;

namespace CacheStuff
{
    public class LRUCache
    {
        private Dictionary<int, DoublyLinkedListNode> _hashedNodes;
        
        private DoublyLinkedListNode _leastRecent;
        private DoublyLinkedListNode _mostRecent;
        private int _capacity;
        private int _currentCount = 0;
        
        public LRUCache(int capacity)
        {
            if (capacity < 1) throw new ArgumentException("capacity must be greater than zero.");
            
            _capacity = capacity;
            _hashedNodes = new Dictionary<int, DoublyLinkedListNode>(_capacity);
        }

        public int Get(int key)
        {
            if (!_hashedNodes.ContainsKey(key)) return -1;
            
            // Grab the selected Node
            var selectedNode = _hashedNodes[key];
            
            // Let's re-link the surrounding nodes if necessary
            var lessRecentNode = selectedNode.LessRecent;
            var moreRecentNode = selectedNode.MoreRecent;

            // If this is the most recent node then return the value
            if (moreRecentNode == null) return selectedNode.Value;

            // If this is the least recent node (and not also the most recent) then re-link _leastRecent
            if (lessRecentNode == null) _leastRecent = moreRecentNode;

            // Link neighboring nodes
            moreRecentNode.LessRecent = lessRecentNode;
            if (lessRecentNode != null) lessRecentNode.MoreRecent = moreRecentNode;
            
            // Link the selected Node to the most recent
            selectedNode.LessRecent = _mostRecent;
            selectedNode.MoreRecent = null;
            _mostRecent.MoreRecent = selectedNode;
            
            // Make the most recent node point to the selected Node
            _mostRecent = selectedNode;
            
            return selectedNode.Value;
        }

        public void Set(int key, int value)
        {
            if (_hashedNodes.ContainsKey(key))
            {
                // So we have this key already.
                // Let's make it the most recently used.
                Get(key);
                
                // Now let's set the value (in case it's changing) and exit.
                _hashedNodes[key].Value = value;
                return;
            }
            
            // OK, we don't have this key.  Let's add it.
            var newNode = new DoublyLinkedListNode();
            newNode.Key = key;
            newNode.Value = value;
            newNode.MoreRecent = null;
            newNode.LessRecent = _mostRecent;

            _hashedNodes[key] = newNode;
            
            // Now let's make it the most recent
            if (_mostRecent != null) _mostRecent.MoreRecent = newNode;
            _mostRecent = newNode;
            
            // If this is the first element then we need to make it the least recent as well
            if (_leastRecent == null) _leastRecent = newNode;
            
            // if we're not at capacity then increment count and exit
            if (_currentCount < _capacity)
            {
                _currentCount++;
                return;
            }
            
            // We're at capacity so we need to remove an element.
            // Let's remove the least recent node from the hashed nodes
            _hashedNodes.Remove(_leastRecent.Key);

            // Now let's remove this node from the list
            // NOTE:  There are at lease two elements in this list
            // therefore _leastRecent.MoreRecent is not null
            _leastRecent.MoreRecent.LessRecent = null;
            _leastRecent = _leastRecent.MoreRecent;
            
            // Done!
        }
        
        private class DoublyLinkedListNode
        {
            public int Key;
            public int Value;
            public DoublyLinkedListNode LessRecent;
            public DoublyLinkedListNode MoreRecent;
        }
    }
}