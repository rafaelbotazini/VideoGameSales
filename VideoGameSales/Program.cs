using System;
using System.IO;

namespace VideoGameSales
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = RandomAccessFile.Open("C:/users/rafael/desktop/vgsales.bin");

            for (int i = 0; i < file.TotalRecords; i++)
            {
                Console.WriteLine(file.ReadPosition(i));
            }

            file.Close();
        }

        static void GenerateFiles(string path)
        {
            using (var file = File.Open(path, FileMode.Open))
            using (var reader = new StreamReader(file))
            {
                string line;

                // Remove headerline
                reader.ReadLine();

                var binaryFile = RandomAccessFile.Open("C:/users/rafael/desktop/vgsales.bin");
                var count = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] data = SplitLine(line);

                    var salesRecord = new VideoGameSalesRecord
                    {
                        Rank = (int)ConvertDouble(data[0]),
                        Name = data[1],
                        Platform = data[2],
                        Year = (int)ConvertDouble(data[3]),
                        Genre = data[4],
                        Publisher = data[5],
                        NASales = ConvertDouble(data[6]),
                        EUSales = ConvertDouble(data[7]),
                        JPSales = ConvertDouble(data[8]),
                        OtherSales = ConvertDouble(data[9]),
                        GlobalSales = ConvertDouble(data[10]),
                    };

                    binaryFile.Write(salesRecord);
                    Console.Write("\r{0} records written", ++count);
                }

                Console.WriteLine("");

                binaryFile.Close();
            }
        }

        static double ConvertDouble(string str)
        {
            double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double result);
            return result;
        }

        static string[] SplitLine(string line)
        {
            var data = line.Split(',');

            var result = new List<string>();

            foreach (var part in data)
            {
                int lastIndex = result.Count - 1;

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
