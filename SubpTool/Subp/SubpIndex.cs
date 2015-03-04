using System.IO;
using System.Text;

namespace SubpTool.Subp
{
    public class SubpIndex
    {
        public const int Size = 8;
        public uint SubtitleId { get; set; }
        public uint Offset { get; set; }

        public static SubpIndex ReadSubpIndex(Stream input)
        {
            SubpIndex subpIndex = new SubpIndex();
            subpIndex.Read(input);
            return subpIndex;
        }

        private void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);
            SubtitleId = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
        }

        public void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.Write(SubtitleId);
            writer.Write(Offset);
        }
    }
}
