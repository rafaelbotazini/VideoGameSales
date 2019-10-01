using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace VideoGameSales
{
    public abstract class RandomAccessFile<T>
    {
        // Constants
        private const int HeadSize = SizeOf.Int32;

        // Properties
        public int TotalRecords { get; private set; }

        // Fields
        private int currentPosition = 0;

        private readonly int recordSize;
        protected readonly FileStream file;
        protected readonly BinaryReader reader;
        protected readonly BinaryWriter writer;

        protected RandomAccessFile(string path, int recordSize)
        {
            this.recordSize = recordSize;

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

        protected int GetPosition(int position)
        {
            return position * recordSize + HeadSize;
        }

        protected string ReadString(int limit)
        {
            string str = reader.ReadString();
            file.Seek(limit - str.Length, SeekOrigin.Current);
            return str;
        }

        protected void WriteString(string str, int limit)
        {
            var length = str?.Length ?? 0;

            writer.Write(str ?? "");

            file.Seek(SizeOf.String - length, SeekOrigin.Current);
        }

        public T Read()
        {
            T record = ReadPosition(currentPosition);

            if (record != null)
            {
                currentPosition++;
            }
            else
            {
                currentPosition = 0;
            }

            return record;
        }

        public void Append(T record)
        {
            // Getting end position
            var position = GetPosition(TotalRecords);
            Write(position, record);
            file.Seek(0, SeekOrigin.Begin);
            writer.Write(++TotalRecords);
        }

        public abstract T ReadPosition(int position);

        public abstract void Write(int pos, T obj);
    }
}
