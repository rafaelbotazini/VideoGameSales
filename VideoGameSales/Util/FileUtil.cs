using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VideoGameSales
{
    public class FileUtil
    {
        public static string InvertedIndexFilePath = "invertedIndex.bin";
        public static string WordIndexFilePath = "wordIndex.bin";
        public static string BinaryFilePath = "vgsales.bin";
        public static string CSVFilePath = "vgsales.csv";

        /// <summary>
        /// Creates a new binary file from a csv file.
        /// Any exisiting output file with the same name will be overwritten.
        /// </summary>
        /// <param name="path">Path to csv file</param>
        /// <param name="output">Path to output binary file</param>
        public static void GenerateVideoGameSalesBinaryFile(string path, string output)
        {
            using (var file = File.Open(path, FileMode.Open))
            using (var reader = new StreamReader(file))
            {
                Delete(output);
                Delete(InvertedIndexFilePath);
                Delete(WordIndexFilePath);

                string line;

                // Skip header
                reader.ReadLine();

                var binaryFile = VideoGameSalesRandomAccessFile.Open(output);
                var count = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] data = SplitLine(line);

                    var salesRecord = new VideoGameSalesRecord
                    {
                        Rank = (int)NumberUtil.ConvertDouble(data[0]),
                        Name = data[1],
                        Platform = data[2],
                        Year = (int)NumberUtil.ConvertDouble(data[3]),
                        Genre = data[4],
                        Publisher = data[5],
                        NASales = NumberUtil.ConvertDouble(data[6]),
                        EUSales = NumberUtil.ConvertDouble(data[7]),
                        JPSales = NumberUtil.ConvertDouble(data[8]),
                        OtherSales = NumberUtil.ConvertDouble(data[9]),
                        GlobalSales = NumberUtil.ConvertDouble(data[10]),
                    };

                    binaryFile.Append(salesRecord);

                    Console.Write("\r{0} records written", ++count);
                }

                binaryFile.Close();

                Console.WriteLine();

                var info = new FileInfo(output);

                Console.WriteLine("{0} bytes written to {1}", info.Length, info.FullName);
            }
        }

        public static void GenerateInvertedIndex(string gameName, int recordPosition)
        {
            foreach (string word in StringUtil.GetGameNameWords(gameName))
            {
                AddWordToInvertedIndex(word, recordPosition);
            }
        }

        private static void AddWordToInvertedIndex(string word, int recordPosition)
        {
            int hash = StringUtil.GetWordHash(word);

            var invertedIndexFile = InvertedIndexRandomAccessFile.Open(InvertedIndexFilePath);
            var wordIndexFile = WordIndexRandomAccessFile.Open(WordIndexFilePath);

            AddWordToInvertedIndex(word, recordPosition, invertedIndexFile, wordIndexFile, hash);

            invertedIndexFile.Close();
            wordIndexFile.Close();
        }

        private static void AddWordToInvertedIndex(string word, int recordPosition, InvertedIndexRandomAccessFile invertedIndexFile, WordIndexRandomAccessFile wordIndexFile, int hash)
        {
            WordIndex index = wordIndexFile.ReadPosition(hash);
            // new word
            if (index == null)
            {
                // add word to inverted index
                invertedIndexFile.Append(new InvertedIndex
                {
                    Word = word,
                    Records = new int[] { recordPosition },
                });

                // write new 
                int positionCreated = invertedIndexFile.TotalRecords - 1;

                wordIndexFile.Add(hash, new WordIndex
                {
                    InvertedIndexPosition = positionCreated,
                    StringHash = hash,
                    Next = -1
                });
            }
            else
            {
                InvertedIndex invertedIndex = invertedIndexFile.ReadPosition(index.InvertedIndexPosition);

                // word matches
                if (word == invertedIndex.Word)
                {
                    if (!invertedIndex.Records.Any(r => r == recordPosition))
                    {
                        // update index adding new position
                        invertedIndex.Records = invertedIndex.Records.Concat(new int[] { recordPosition });

                        invertedIndexFile.Write(index.InvertedIndexPosition, invertedIndex);
                    }
                }
                else
                {
                    if (index.Next == -1)
                    {
                        index.Next = wordIndexFile.TotalRecords;
                        wordIndexFile.Write(hash, index);
                    }
                    // go to next
                    AddWordToInvertedIndex(word, recordPosition, invertedIndexFile, wordIndexFile, index.Next);
                }
            }
        }

        /// <summary>
        /// Removes a file from disk.
        /// </summary>
        /// <param name="path">Path to the file</param>
        public static void Delete(string path)
        {
            var info = new FileInfo(path);
            if (File.Exists(path))
            {
                File.Delete(info.FullName);
            }
        }

        /// <summary>
        /// Utility method to split a csv line into an array of strings.
        /// </summary>
        /// <param name="line">CSV line</param>
        /// <returns></returns>
        static string[] SplitLine(string line)
        {
            var data = line.Split(',');

            var result = new List<string>();

            foreach (var part in data)
            {
                int lastIndex = result.Count - 1;

                // Merge and restore double quote escaped parts
                if (result.Count > 0 && result[lastIndex].StartsWith('"') && part.EndsWith('"'))
                {
                    result.Add((result[lastIndex] + "," + part).Replace("\"", ""));
                    result.RemoveAt(lastIndex);
                }
                else
                {
                    result.Add(part);
                }
            }

            return result.ToArray();
        }

        internal static void GenerateVideoGameSalesBinaryFile(object cSVFilePath, object binaryFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
