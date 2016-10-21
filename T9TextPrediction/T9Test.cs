using System;
using T9TextPrediction;
using System.Collections.Generic;

namespace T9Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var test1 = new [] {2};
            var test2 = new [] {3};
            var test3 = new [] {8,3};
            var test4 = new [] {2,2,5};
            var test5 = new [] {2,3,3,8};
            var test6 = new [] {8,6};

            var predictor = new T9TextPredictor("./LargeDictionary.txt");

            PrintList(test1, predictor.GetWords(test1, 8));
            PrintList(test2, predictor.GetWords(test2, 8));
            PrintList(test3, predictor.GetWords(test3, 8));
            PrintList(test4, predictor.GetWords(test4, 8));
            PrintList(test5, predictor.GetWords(test5, 8));
            PrintList(test6, predictor.GetWords(test6, 8));
        }

        private static void PrintList(int[] code, List<string> words)
        {
            foreach (var elem in code)
            {
                Console.Write(" {0}", elem);
            }
            Console.WriteLine();
            foreach (var word in words)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine();
        }
    }
}