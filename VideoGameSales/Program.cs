using System;

namespace VideoGameSales
{
    class Program
    {
        static string BinaryFilePath = "vgsales.bin";
        static string CSVFilePath = "vgsales.csv";

        static void Main(string[] args)
        {
            FileUtil.GenerateVideoGameSalesBinaryFile(CSVFilePath, BinaryFilePath);
            ReadAll(BinaryFilePath);
        }

        static void ReadAll(string path)
        {
            var file = VideoGameSalesRandomAccessFile.Open(path);

            VideoGameSalesRecord record;

            while ((record = file.Read()) != null)
            {
                Console.WriteLine(record);
            }

            file.Close();
        }
    }
}
