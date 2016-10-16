using System;
using System.Collections.Generic;
using DataStructures;

namespace DataStructuresTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var data = new int[] {1,4,3,3,7,3,6,8,1,-2,3};

            foreach (var datum in data)
            {
                Console.Write(" {0}", datum);
            }
            Console.WriteLine();

            var maxHeap = new MaxHeap<int>(data);
            var minHeap = new MinHeap<int>(data);

            Console.WriteLine("Max\tMin");
            while (maxHeap.NumElems > 0)
            {
                Console.WriteLine("{0}\t{1}", maxHeap.Pop(), minHeap.Pop());
            }
        }
    }
}