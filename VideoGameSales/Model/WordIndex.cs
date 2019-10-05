using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameSales
{
    public class WordIndex
    {
        public int StringHash { get; set; }
        public int InvertedIndexPosition { get; set; }
        public int Next { get; set; }

        public override string ToString()
        {
            return $"Hash: {StringHash}, InvertedIndex: {InvertedIndexPosition}, Next: {Next}";
        }
    }
}
