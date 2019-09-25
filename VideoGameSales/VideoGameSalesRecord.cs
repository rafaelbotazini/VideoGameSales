using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameSales
{
    public class VideoGameSalesRecord
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        public double NASales { get; set; }
        public double EUSales { get; set; }
        public double JPSales { get; set; }
        public double OtherSales { get; set; }
        public double GlobalSales { get; set; }

        public override string ToString()
        {
            return $"{Rank}\t{Name}\t{Year}\t{Platform}\t{Genre}\t{Publisher}\t{NASales}\t{EUSales}\t{JPSales}\t{OtherSales}\t{GlobalSales}";
        }
    }
}
