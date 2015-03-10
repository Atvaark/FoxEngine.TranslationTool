using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FfntTool.Ffnt
{
    [XmlType("FontData")]
    public class FontData : FfntEntry
    {
        public const string FontSignature = "FTDT";

        [XmlAttribute("Unknown1")]
        public int Unknown1 { get; set; }

        [XmlAttribute("Unknown2")]
        public int Unknown2 { get; set; }

        [XmlAttribute("Unknown3")]
        public int Unknown3 { get; set; }

        [XmlIgnore]
        public byte[] Data { get; set; }

        public static FontData ReadFontData(Stream inputStream)
        {
            FontData fontData = new FontData();
            fontData.Read(inputStream);
            return fontData;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.Default, true);
            Unknown1 = reader.ReadInt32();
            int size = reader.ReadInt32();
            Unknown2 = reader.ReadInt32();
            Unknown3 = reader.ReadInt32();
            Data = reader.ReadBytes(size);
            // TODO: Check which unknown properties are constant zero
        }

        public override void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.AlignWrite(16, 0x00);
            writer.Write(Unknown1);
            writer.Write(Data.Length);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(Data);
        }

        public override FfntEntryHeader GetHeader(Stream outputStream)
        {
            FfntEntryHeader header = new FfntEntryHeader
            {
                FfntEntrySignature = FontSignature,
                Offset = (int) outputStream.Position,
                Size = Data.Length + 16
            };
            return header;
        }
    }
}
