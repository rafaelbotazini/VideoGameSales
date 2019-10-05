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
        private readonly FileStream file;
        protected readonly BinaryReader reader;
        protected readonly BinaryWriter writer;

        /// <summary>
        /// Defines a class that represents a random access file of records of a type <see cref="T" />
        /// </summary>
        /// <param name="path">Binary file path</param>
        /// <param name="recordSize">Size of a single record, in bytes</param>
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

        /// <summary>
        /// Closes the binary file stream
        /// </summary>
        public void Close()
        {
            file.Close();
        }

        /// <summary>
        /// Updates Head property from file
        /// </summary>
        private void SetHead()
        {
            TotalRecords = reader.ReadInt32();
        }

        /// <summary>
        /// Init Head with default values
        /// </summary>
        private void InitHead()
        {
            UpdateHead(0);
        }

        /// <summary>
        /// Gets the absolute position on the file stream.
        /// </summary>
        /// <param name="position">The relative position</param>
        /// <returns></returns>
        protected int GetPosition(int position)
        {
            return position * recordSize + HeadSize;
        }

        /// <summary>
        /// Moves the file stream pointer to a relative position.
        /// </summary>
        /// <param name="position">Position to move</param>
        protected void SeekFromBegin(int position)
        {
            int offset = GetPosition(position);
            file.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Reads a string and moves the file stream pointer to a maximum given limit
        /// from the start of the string.
        /// </summary>
        /// <param name="limit">Max. string length</param>
        /// <returns></returns>
        protected string ReadString(int limit)
        {
            string str = reader.ReadString();
            file.Seek(file.Position - str.Length + limit, SeekOrigin.Begin);
            return str;
        }

        /// <summary>
        /// Writes a limited ammount of characters of a string to the stream and moves the
        /// stream pointer to the given limit.
        /// </summary>
        /// <param name="str">String to write</param>
        /// <param name="limit">Max. string length</param>
        protected void WriteString(string str, int limit)
        {
            var length = str?.Length ?? 0;

            if (length > limit)
            {
                throw new Exception("String " + str + " truncated to " + limit + " characters.");
            }

            writer.Write(str ?? "");

            file.Seek(file.Position - length + limit, SeekOrigin.Begin);
        }

        protected void UpdateHead(int total)
        {
            TotalRecords = total;
            file.Seek(0, SeekOrigin.Begin);
            writer.Write(TotalRecords);
        }

        /// <summary>
        /// Reads a record from the stream and moves the pointer to the next record.
        /// Will return null if no record is found.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Writes a record to the end of the file.
        /// </summary>
        /// <param name="record">Record to be written</param>
        public void Append(T record)
        {
            Write(TotalRecords, record);
            UpdateHead(TotalRecords + 1);
        }

        /// <summary>
        /// Gets a record from a given position.
        /// Should return null if no records are found.
        /// </summary>
        /// <param name="position">Position to seek the record</param>
        /// <returns></returns>
        public abstract T ReadPosition(int position);

        /// <summary>
        /// Writes a record on a given position.
        /// </summary>
        /// <param name="position">Position to write the record on</param>
        /// <param name="record">Record to be written</param>
        public abstract void Write(int position, T record);
    }
}
