using System;
using System.Collections.Generic;
using System.IO;

namespace VideoGameSales
{
    public class FileUtil
    {

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

        public static void GenerateInvertedIndexFile(string binaryFilePath, string output)
        {
            throw new NotImplementedException();
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

    }
}
