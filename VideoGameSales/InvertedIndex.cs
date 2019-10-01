using System.Collections.Generic;
using System.IO;

namespace VideoGameSales
{
    public class InvertedIndex
    {
        public string Word { get; set; }
        public IEnumerable<int> Records { get; set; }
    }
}