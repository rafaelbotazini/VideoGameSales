using System;
using System.Collections.Generic;
using System.IO;

namespace VideoGameSales
{
    public class InvertedIndexRandomAccessFile : RandomAccessFile<InvertedIndex>
    {
        const int RecordListStringSize = 1024;

        private InvertedIndexRandomAccessFile(string path, int recordSize) : base(path, recordSize) { }

        /// <summary>
        /// Opens a random access file with InvertedIndexRandomAccessFile records.
        /// If the file does not exist. It will be created.
        /// </summary>
        /// <param name="path">Path to the binary file</param>
        /// <returns></returns>
        public static InvertedIndexRandomAccessFile Open(string path)
        {
            var recordSize = SizeOf.String + RecordListStringSize;
            var randomAccessFile = new InvertedIndexRandomAccessFile(path, recordSize);
            return randomAccessFile;
        }

        /// <summary>
        /// Gets a record from a given position.
        /// Returns null if no record is found at the position.
        /// </summary>
        /// <param name="position">Position to seek the record</param>
        /// <returns></returns>
        public override InvertedIndex ReadPosition(int position)
        {
            if (position >= TotalRecords)
            {
                return null;
            }

            SeekFromBegin(position);

            var record = new InvertedIndex
            {
                Word = ReadString(SizeOf.String),
                Records = SplitRecords(ReadString(RecordListStringSize))
            };

            return record;
        }

        /// <summary>
        /// Writes a single <see cref="InvertedIndex" /> object to the file on a given position.
        /// </summary>
        /// <param name="position">Position to write the record on</param>
        /// <param name="record">Record to be written</param>
        public override void Write(int position, InvertedIndex record)
        {
            SeekFromBegin(position);
            WriteString(record.Word, SizeOf.String);
            WriteString(string.Join(',', record.Records), RecordListStringSize);
        }

        /// <summary>
        /// Splits and converts a string of comma separated integers to a collection of integers.
        /// </summary>
        /// <param name="records">Comma separated integer sequence</param>
        /// <returns></returns>
        private IEnumerable<int> SplitRecords(string records)
        {
            string[] split = records.Split(',');
            int[] converted = Array.ConvertAll(split, int.Parse);
            return converted;
        }
    }
}