using System;
using System.Collections.Generic;

namespace DataStructures
{
    public abstract class Heap<T>
    {
        private const int DefaultHeapSize = 8;
        private const int Root = 1;

        // _minOrMax = 1 for maxHeap and -1 for minHeap.  Default is maxHeap.
        protected readonly int _minOrMax = 1;

        // An array that will hold the elements in the heap
        private T[] _heap;
        public int NumElems { get; private set; }

        // A comparison operation for the elements in the heap
        // NOTE: IComparer.Compare(x, y) returns <0 if x preceeds y, 0 if x equals y, and >0 if y preceeds x
        private readonly IComparer<T> _cmp;

        // A constructor that initializes the heap size and assigns the default comparison operation
        public Heap(int minOrMax)
        {
            _minOrMax = minOrMax;
            _heap = new T[DefaultHeapSize];
            _cmp = Comparer<T>.Default;
        }

        // A constructor that initializes the heap size and assigns a custom comparison operation
        public Heap(IComparer<T> cmp, int minOrMax)
        {
            _minOrMax = minOrMax;
            _heap = new T[DefaultHeapSize];
            _cmp = cmp;
        }

        public Heap(IList<T> data, int minOrMax)
        {
            _minOrMax = minOrMax;
            _cmp = Comparer<T>.Default;

            var size = 2;
            while (size < data.Count + 1) size *= 2;

            _heap = new T[size];
            foreach (var datum in data)
            {
                this.Insert(datum);
            }
        }

        // Peaks at the root of the tree.
        public T Peek()
        {
            if (NumElems < 1) throw new InvalidOperationException("No elements in Heap.");

            return _heap[Root];
        }

        // Pops the tree root off and restructures;
        public T Pop()
        {
            if (NumElems < 1) throw new InvalidOperationException("No elements in Heap.");

            var retVal = _heap[Root];

            _heap[Root] = _heap[NumElems--];
            if (NumElems > 1) BubbleDown(Root);

            return retVal;
        }

        // Inserts element at end of heap and restructures
        public void Insert(T elem)
        {
            if (NumElems >= _heap.Length - 1) GrowHeap();

            _heap[++NumElems] = elem;
            BubbleUp(NumElems);
        }

        private void GrowHeap()
        {
            var temp = new T[_heap.Length * 2];
            _heap.CopyTo(temp, 0);
            _heap = temp;
        }

        #region "Methods for restructuring Heap"
        private void BubbleUp(int index)
        {
            if (index <= 0 || index > NumElems) throw new IndexOutOfRangeException("Index out of range.");

            // Exit early if we are at the root of the tree.
            if (index == 1) return;

            // Compare the the parent.  Exit early if relationship is valid.
            var parent = Parent(index);
            if (_minOrMax * _cmp.Compare(_heap[parent], _heap[index]) > 0) return;

            // Relationship is invalid.  Swap and recurse.
            SwapElems(index, parent);
            BubbleUp(parent);
        }

        private void BubbleDown(int index)
        {
            if (index <= 0 || index > NumElems) throw new IndexOutOfRangeException("Index out of range.");

            // Grab the children indices
            var leftChild = LeftChild(index);
            var rightChild = RightChild(index);

            // Determine which of the three elements should be the local root
            var swapVal = _heap[index];
            var swapIndex = index;

            if (leftChild < NumElems && (_minOrMax * _cmp.Compare(_heap[leftChild], swapVal) > 0))
            {
                swapVal = _heap[leftChild];
                swapIndex = leftChild;
            }

            if (rightChild < NumElems && (_minOrMax * _cmp.Compare(_heap[rightChild], swapVal) > 0))
            {
                swapVal = _heap[rightChild];
                swapIndex = rightChild;
            }

            // Exit early if relationship with children is valid.
            if (swapIndex == index) return;

            // Relationship is invalid. Swap and recurse with appropriate child.
            SwapElems(index, swapIndex);
            BubbleDown(swapIndex);
        }

        private void SwapElems(int index1, int index2)
        {
            var temp = _heap[index1];
            _heap[index1] = _heap[index2];
            _heap[index2] = temp;
        }
        #endregion

        #region "Methods for traversing tree structure"
        private int Parent(int index)
        {
            if (index <= 0 || index > NumElems) throw new IndexOutOfRangeException("Index out of range.");

            return index / 2;
        }

        private int LeftChild(int index)
        {
            if (index <= 0 || index > NumElems) throw new IndexOutOfRangeException("Index out of range.");

            return index * 2;
        }

        private int RightChild(int index)
        {
            if (index <= 0 || index > NumElems) throw new IndexOutOfRangeException("Index out of range.");

            return index * 2 + 1;
        }
        #endregion
    }

    public class MaxHeap<T> : Heap<T>
    {
        public MaxHeap() : base(1) { }

        public MaxHeap(IComparer<T> cmp) : base(cmp, 1) { }

        public MaxHeap(IList<T> data) : base(data, 1) { }
    }

    public class MinHeap<T> : Heap<T>
    {
        public MinHeap() : base(-1) { }

        public MinHeap(IComparer<T> cmp) : base(cmp, -1) { }

        public MinHeap(IList<T> data) : base(data, -1) { }
    }
}