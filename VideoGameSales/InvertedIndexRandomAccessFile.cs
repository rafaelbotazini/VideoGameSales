using System;
using System.Collections.Generic;
using System.IO;

namespace VideoGameSales
{
    public class InvertedIndexRandomAccessFile : RandomAccessFile<InvertedIndex>
    {
        const int RecordListStringSize = 1024;

        private InvertedIndexRandomAccessFile(string path, int recordSize) : base(path, recordSize) { }

        public static InvertedIndexRandomAccessFile Open(string path)
        {
            var recordSize = SizeOf.String + RecordListStringSize;
            var randomAccessFile = new InvertedIndexRandomAccessFile(path, recordSize);
            return randomAccessFile;
        }

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

        public override void Write(int position, InvertedIndex record)
        {
            SeekFromBegin(position);
            WriteString(record.Word, SizeOf.String);
            WriteString(string.Join(',', record.Records), RecordListStringSize);
        }

        private IEnumerable<int> SplitRecords(string records)
        {
            string[] split = records.Split(',');
            int[] converted = Array.ConvertAll(split, int.Parse);
            return converted;
        }
    }
}