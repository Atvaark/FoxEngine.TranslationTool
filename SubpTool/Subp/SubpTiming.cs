using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SubpTool.Subp
{
    public class SubpTiming
    {
        [XmlAttribute("Start")]
        public ushort Start { get; set; }

        [XmlAttribute("End")]
        public ushort End { get; set; }

        public static SubpTiming ReadSubpTiming(Stream input)
        {
            SubpTiming subpTiming = new SubpTiming();
            subpTiming.Read(input);
            return subpTiming;
        }

        private void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);
            Start = reader.ReadUInt16();
            End = reader.ReadUInt16();
        }

        public void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.Write(Start);
            writer.Write(End);
        }
    }
}
