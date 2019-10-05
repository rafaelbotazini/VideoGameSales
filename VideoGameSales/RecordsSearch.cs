using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoGameSales
{
    public class RecordsSearch
    {
        /// <summary>
        /// Finds all records that matches the query string.
        /// </summary>
        /// <param name="name">The query string</param>
        /// <returns></returns>
        public static List<VideoGameSalesRecord> SearchSalesRecordsByName(string name)
        {
            var file = VideoGameSalesRandomAccessFile.Open(FileUtil.BinaryFilePath);
            var index = WordIndexRandomAccessFile.Open(FileUtil.WordIndexFilePath);
            var invertedIndex = InvertedIndexRandomAccessFile.Open(FileUtil.InvertedIndexFilePath);

            var words = StringUtil.GetGameNameWords(name);

            var results = new List<VideoGameSalesRecord>();

            foreach (var word in words)
            {
                int hash = StringUtil.GetWordHash(word);

                var wordIndex = index.ReadPosition(hash);

                if (wordIndex != null)
                {
                    InvertedIndex inverted;
                    do
                    {
                        inverted = invertedIndex.ReadPosition(wordIndex.InvertedIndexPosition);
                        if (inverted.Word == word)
                        {
                            var gamesFound = inverted.Records.Select(position => file.ReadPosition(position));

                            results.AddRange(gamesFound);

                            // break loop
                            inverted = null;
                        }
                        else
                        {
                            wordIndex = index.ReadPosition(wordIndex.Next);
                        }
                    } while (inverted != null);
                }
            }

            file.Close();
            index.Close();
            invertedIndex.Close();

            // Trying to get the games that matches all words
            var bestMatch = results
                .Where(result => Array
                    .TrueForAll(words, word => StringUtil.GetGameNameWords(result.Name).Contains(word)));

            IEnumerable<VideoGameSalesRecord> finalResult;
            
            if (bestMatch.Any())
            {
                finalResult = bestMatch;
            }
            else
            {
                finalResult = results;
            }

            return finalResult.OrderBy(record => record.Rank).Distinct(new RankComparer()).ToList();
        }

        class RankComparer : IEqualityComparer<VideoGameSalesRecord>
        {
            public bool Equals(VideoGameSalesRecord x, VideoGameSalesRecord y)
            {
                return x.Rank == y.Rank;
            }

            public int GetHashCode(VideoGameSalesRecord obj)
            {
                return 0;
            }
        }
    }
}
