using System.IO;
using System.Text;

namespace SubpTool.Subp
{
    class SubpFileIndex
    {
        public uint SubId { get; set; }
        public uint Offset { get; set; }
        public SubpFileEntry Entry { get; set; }

        public static SubpFileIndex ReadSubpFileIndex(Stream input)
        {
            SubpFileIndex subpFileIndex = new SubpFileIndex();
            subpFileIndex.Read(input);
            return subpFileIndex;
        }

        private void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);
            SubId = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
        }
    }
}
