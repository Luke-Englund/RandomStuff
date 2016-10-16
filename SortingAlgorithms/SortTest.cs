using System;
using System.IO;
using SortingAlgos;

namespace SortTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var testArr = new[]{5,2,3,6,1};

            MergeSort.Sort(testArr);

            foreach (var elem in testArr)
            {
                Console.WriteLine("{0}", elem);
            }
        }
    }
}