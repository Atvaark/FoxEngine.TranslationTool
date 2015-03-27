using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FfntTool.Ffnt
{
    [XmlType("Glyph")]
    public class Glyph
    {
        public const int GlyphSize = 20;

        [XmlAttribute("Character")]
        public string CharacterString
        {
            get { return Character.ToString(); }
            set { Character = value.Length == 1 ? value[0] : ' '; }
        }

        [XmlIgnore]
        public char Character { get; set; }

        [XmlAttribute]
        public short XOffset { get; set; }

        [XmlAttribute]
        public short YOffset { get; set; }

        [XmlAttribute]
        public byte Width { get; set; }

        [XmlAttribute]
        public byte Height { get; set; }

        [XmlAttribute]
        public byte Layer { get; set; }

        [XmlAttribute]
        public byte HorizontalSpace { get; set; }

        [XmlAttribute]
        public byte HorizontalShift { get; set; }

        [XmlAttribute]
        public byte VerticalShift { get; set; }

        [XmlAttribute]
        public short Unknown1 { get; set; }

        [XmlAttribute]
        public int Unknown2 { get; set; }

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
            Layer = reader.ReadByte();
            HorizontalSpace = reader.ReadByte();
            HorizontalShift = reader.ReadByte();
            VerticalShift = reader.ReadByte();
            Unknown1 = reader.ReadInt16();
            Unknown2 = reader.ReadInt32();
            // TODO: Check which unknown properties are constant zero
        }

        public void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.Write((int) Character);
            writer.Write(XOffset);
            writer.Write(YOffset);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Layer);
            writer.Write(HorizontalSpace);
            writer.Write(HorizontalShift);
            writer.Write(VerticalShift);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
        }
    }
}
