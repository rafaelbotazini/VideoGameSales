using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace VideoGameSales
{
    public class RandomAccessFile
    {
        public int TotalRecords { get; private set; }

        int recordSize = (SizeOf.Int32 * 2) + (SizeOf.String * 4) + (SizeOf.Double * 5);
        const int headSize = SizeOf.Int32;

        FileStream file;
        BinaryReader reader;
        BinaryWriter writer;

        private RandomAccessFile() { }

        private RandomAccessFile(string path)
        {
            bool isNewFile = !File.Exists(path);

            file = File.Open(path, FileMode.OpenOrCreate);

            reader = new BinaryReader(file);
            writer = new BinaryWriter(file, Encoding.UTF8);

            if (isNewFile)
            {
                InitHead();
            }
            else
            {
                SetHead();
            }
        }

        public static RandomAccessFile Open(string path)
        {
            return new RandomAccessFile(path);
        }


        public void Write(VideoGameSalesRecord record)
        {
            var position = TotalRecords * recordSize + headSize;

            file.Seek(position, SeekOrigin.Begin);

            writer.Write(record.Rank);

            writer.Write(record.Name);
            file.Seek(SizeOf.String - record.Name.Length, SeekOrigin.Current);

            writer.Write(record.Genre);
            file.Seek(SizeOf.String - record.Genre.Length, SeekOrigin.Current);

            writer.Write(record.Platform);
            file.Seek(SizeOf.String - record.Platform.Length, SeekOrigin.Current);

            writer.Write(record.Publisher);
            file.Seek(SizeOf.String - record.Publisher.Length, SeekOrigin.Current);

            writer.Write(record.Year);
            writer.Write(record.NASales);
            writer.Write(record.JPSales);
            writer.Write(record.EUSales);
            writer.Write(record.GlobalSales);

            file.Seek(0, SeekOrigin.Begin);
            writer.Write(++TotalRecords);
        }

        public VideoGameSalesRecord ReadPosition(int index)
        {
            if (index >= TotalRecords)
            {
                return default;
            }

            var position = index * recordSize + headSize;

            file.Seek(position, SeekOrigin.Begin);

            var record = new VideoGameSalesRecord();

            record.Rank = reader.ReadInt32();

            record.Name = reader.ReadString();
            file.Seek(SizeOf.String - record.Name.Length, SeekOrigin.Current);

            record.Genre = reader.ReadString();
            file.Seek(SizeOf.String - record.Genre.Length, SeekOrigin.Current);

            record.Platform = reader.ReadString();
            file.Seek(SizeOf.String - record.Platform.Length, SeekOrigin.Current);

            record.Publisher = reader.ReadString();
            file.Seek(SizeOf.String - record.Publisher.Length, SeekOrigin.Current);

            record.Year = reader.ReadInt32();
            record.NASales = reader.ReadDouble();
            record.JPSales = reader.ReadDouble();
            record.EUSales = reader.ReadDouble();
            record.GlobalSales = reader.ReadDouble();
            return record;
        }

        public void Close()
        {
            file.Close();
        }

        private void SetHead()
        {
            TotalRecords = reader.ReadInt32();
        }

        private void InitHead()
        {
            TotalRecords = 0;
            writer.Write(TotalRecords);
        }
    }
}
