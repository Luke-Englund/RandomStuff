using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DictionaryCreator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dict = new Dictionary<string, int>();

            using(var sr = new StreamReader("./Hamlet.txt"))
            {
                string line;
                string[] elems;
                string temp;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    elems = line.Split(' ');
                    foreach(var elem in elems)
                    {
                        if (elem.Length == 0) continue;
                        if (!Regex.IsMatch(elem, @"^[a-zA-Z]+$")) continue;

                        temp = elem.ToLower();
                        if (!dict.ContainsKey(temp)) dict[temp] = 0;
                        dict[temp]++;
                    }
                }

                using (var sw = new StreamWriter(@"./LargeDictionary.txt"))
                {
                    foreach (var keyVal in dict)
                    {
                        sw.WriteLine("{0} {1}", keyVal.Key, keyVal.Value);
                    }
                }
            }
        }
    }
}