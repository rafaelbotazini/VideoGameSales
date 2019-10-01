using System.IO;

namespace VideoGameSales
{
    public class VideoGameSalesRandomAccessFile : RandomAccessFile<VideoGameSalesRecord>
    {
        private VideoGameSalesRandomAccessFile(string path, int recordSize) : base(path, recordSize) { }

        public static VideoGameSalesRandomAccessFile Open(string path)
        {
            var recordSize = (SizeOf.Int32 * 2) + (SizeOf.String * 4) + (SizeOf.Double * 5);
            var randomAccessFile = new VideoGameSalesRandomAccessFile(path, recordSize);
            return randomAccessFile;
        }

        public override VideoGameSalesRecord ReadPosition(int position)
        {
            if (position >= TotalRecords)
            {
                return null;
            }

            SeekFromBegin(position);

            var record = new VideoGameSalesRecord();

            record.Rank = reader.ReadInt32();
            record.Name = ReadString(SizeOf.String);
            record.Genre = ReadString(SizeOf.String);
            record.Platform = ReadString(SizeOf.String);
            record.Publisher = ReadString(SizeOf.String);
            record.Year = reader.ReadInt32();
            record.NASales = reader.ReadDouble();
            record.JPSales = reader.ReadDouble();
            record.EUSales = reader.ReadDouble();
            record.GlobalSales = reader.ReadDouble();

            return record;
        }

        public override void Write(int position, VideoGameSalesRecord record)
        {
            SeekFromBegin(position);

            writer.Write(record.Rank);

            WriteString(record.Name, SizeOf.String);
            WriteString(record.Genre, SizeOf.String);
            WriteString(record.Platform, SizeOf.String);
            WriteString(record.Publisher, SizeOf.String);

            writer.Write(record.Year);
            writer.Write(record.NASales);
            writer.Write(record.JPSales);
            writer.Write(record.EUSales);
            writer.Write(record.GlobalSales);
        }
    }
}