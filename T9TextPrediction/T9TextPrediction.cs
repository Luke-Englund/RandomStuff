using System;
using System.IO;
using System.Collections.Generic;

namespace T9TextPrediction
{
    public class T9TextPredictor
    {
        private readonly WeightedDictionaryElem[] _sortedDictionary;
        private TrieNode _t9TrieRoot = new TrieNode();

        public T9TextPredictor(string filepath)
        {
            _sortedDictionary = ReadInDictionary(filepath);

            WeightedDictionaryElem currElem;
            for (var i = 0; i < _sortedDictionary.Length; i++)
            {
                currElem = _sortedDictionary[i];
                _t9TrieRoot.AddWord(i, GetT9Code(currElem.Word), 0);
            }
        }

        public List<string> GetWords(int[] t9CodePrefix, int numWords)
        {
            var indices = _t9TrieRoot.GetWords(t9CodePrefix, 0, numWords);

            var wordList = new List<string>();

            foreach (var index in indices)
            {
                wordList.Add(_sortedDictionary[index].Word);
            }

            return wordList;
        }

        // Converter:                      a b c d e f g h i j k l m n o p q r s t u v w x y z
        private int[] _converter = new [] {2,2,2,3,3,3,4,4,4,5,5,5,6,6,6,7,7,7,7,8,8,8,9,9,9,9};
        private int[] GetT9Code(string word)
        {
            word = word.ToLower();

            var t9Code = new int[word.Length];

            for (var i = 0; i < word.Length; i++)
            {
                t9Code[i] = _converter[(int)(word[i] - 'a')];
            }

            return t9Code;
        }

        private WeightedDictionaryElem[] ReadInDictionary(string filepath)
        {
            if (!File.Exists(filepath)) throw new Exception("File path does not exist.");
            
            var dict = new List<WeightedDictionaryElem>();
            using(var sr = new StreamReader(filepath))
            {
                string line;
                string word;
                int weight;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    var elems = line.Split(' ');
                    if (elems.Length != 2) throw new Exception("Invalid Format");

                    word = elems[0];
                    if (!int.TryParse(elems[1], out weight)) throw new Exception("Invalid Format");
                    dict.Add(new WeightedDictionaryElem(word, weight));
                }
            }

            dict.Sort((x, y) => y.Word.Length.CompareTo(x.Word.Length));
            dict.Sort((x, y) => y.Weight.CompareTo(x.Weight));

            return dict.ToArray();
        }
    }

    internal class WeightedDictionaryElem
    {
        public readonly string Word;
        public readonly int Weight;

        public WeightedDictionaryElem(string word, int weight)
        {
            Word = word;
            Weight = weight;
        }
    }

    internal class TrieNode
    {
        private const int NumT9Buttons = 8;
        private const int T9Start = 2;

        private readonly List<int> _dictionaryLocations = new List<int>();
        private readonly TrieNode[] _children = new TrieNode[NumT9Buttons];

        public void AddWord(int dictIndex, int[] t9Code, int codeIndex)
        {
            _dictionaryLocations.Add(dictIndex);

            // Check to see if this is the last node we need to visit.
            if (codeIndex == t9Code.Length) return;

            // We need to visit the next child indicated by the t9Code.
            var code = t9Code[codeIndex] - T9Start;
            if (code < 0 || code >= _children.Length) throw new Exception("Invalid T9 Code");

            // Create the node if neccesary and recurse.
            if (_children[code] == null) _children[code] = new TrieNode();
            _children[code].AddWord(dictIndex, t9Code, codeIndex + 1);
        }

        public List<int> GetWords(int[] t9Code, int codeIndex, int numWords)
        {
            // If this is the final node then return the dictionary locations.
            if (codeIndex == t9Code.Length)
            {
                var numElems = Math.Min(numWords, _dictionaryLocations.Count);
                var retList = new List<int>();
                for (var i = 0; i < numElems; i++) retList.Add(_dictionaryLocations[i]);
                return retList;
            }

            // We need to visit the next child indicated by the t9Code.
            var code = t9Code[codeIndex] - T9Start;
            if (code < 0 || code >= _children.Length) throw new Exception("Invalid T9 Code");

            // If we've hit a dead end then return an empty list.
            if (_children[code] == null) return new List<int>();

            // Return the child's response.
            return _children[code].GetWords(t9Code, codeIndex + 1, numWords);
        }
    }
}