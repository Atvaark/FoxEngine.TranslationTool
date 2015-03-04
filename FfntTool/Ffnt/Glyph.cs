using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FfntTool.Ffnt
{
    [XmlType("Glyph")]
    public class Glyph
    {
        public const int GlyphSize = 20;

        [XmlElement("Character")]
        public string CharacterString
        {
            get { return Character.ToString(); }
            set { Character = value.Length == 1 ? value[0] : ' '; }
        }

        [XmlIgnore]
        public char Character { get; set; }

        public short XOffset { get; set; }
        public short YOffset { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte VerticalSpace { get; set; }
        public byte HorizontalSpace { get; set; }
        public byte HorizontalShift { get; set; }
        public byte VerticalShift { get; set; }
        // TODO: Add a layer property


        public static Glyph ReadGlyph(Stream inputStream)
        {
            Glyph glyph = new Glyph();
            glyph.Read(inputStream);
            return glyph;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.Default, true);
            Character = (char) reader.ReadInt32();
            XOffset = reader.ReadInt16();
            YOffset = reader.ReadInt16();
            Width = reader.ReadByte();
            Height = reader.ReadByte();
            VerticalSpace = reader.ReadByte();
            HorizontalSpace = reader.ReadByte();
            HorizontalShift = reader.ReadByte();
            VerticalShift = reader.ReadByte();
            reader.Skip(6);
        }

        public void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.Write((int) Character);
            writer.Write(XOffset);
            writer.Write(YOffset);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(VerticalSpace);
            writer.Write(HorizontalSpace);
            writer.Write(HorizontalShift);
            writer.Write(VerticalShift);
            writer.WriteZeros(6);
        }
    }
}
