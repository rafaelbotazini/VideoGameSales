using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameSales
{
    public class WordIndexRandomAccessFile : RandomAccessFile<WordIndex>
    {
        public WordIndexRandomAccessFile(string path, int recordSize) : base(path, recordSize)
        {
        }

        public static WordIndexRandomAccessFile Open(string path)
        {
            var randomAccessFile = new WordIndexRandomAccessFile(path, SizeOf.Int32 * 3);
            return randomAccessFile;
        }

        public override WordIndex ReadPosition(int position)
        {

            if (position >= TotalRecords || position == -1)
            {
                return null;
            }

            SeekFromBegin(position);

            WordIndex index = new WordIndex();

            index.StringHash = reader.ReadInt32();
            index.InvertedIndexPosition = reader.ReadInt32();
            index.Next = reader.ReadInt32();

            if (index.StringHash == 0 && index.StringHash == index.Next)
            {
                return null;
            }
            
            return index;
        }

        public void Add(int position, WordIndex record)
        {
            Write(position, record);
            if (TotalRecords < position)
            {
                UpdateHead(position + 1);
            }
            else
            {
                UpdateHead(TotalRecords + 1);
            }
        }

        public override void Write(int position, WordIndex record)
        {
            SeekFromBegin(position);
            writer.Write(record.StringHash);
            writer.Write(record.InvertedIndexPosition);
            writer.Write(record.Next);
        }
    }
}
