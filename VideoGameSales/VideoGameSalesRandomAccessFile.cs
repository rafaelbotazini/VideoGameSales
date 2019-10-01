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

            var offset = GetPosition(position);

            file.Seek(offset, SeekOrigin.Begin);

            var record = new VideoGameSalesRecord
            {
                Rank = reader.ReadInt32(),
                Name = ReadString(SizeOf.String),
                Genre = ReadString(SizeOf.String),
                Platform = ReadString(SizeOf.String),
                Publisher = ReadString(SizeOf.String),
                Year = reader.ReadInt32(),
                NASales = reader.ReadDouble(),
                JPSales = reader.ReadDouble(),
                EUSales = reader.ReadDouble(),
                GlobalSales = reader.ReadDouble(),
            };

            return record;
        }

        public override void Write(int position, VideoGameSalesRecord record)
        {
            file.Seek(position, SeekOrigin.Begin);

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