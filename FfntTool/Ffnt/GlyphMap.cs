using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FfntTool.Ffnt
{
    [XmlType("GlyphMap")]
    public class GlyphMap : FfntEntry
    {
        public const string GlyphSignature = "GLYP";

        public GlyphMap()
        {
            Glyphs = new List<Glyph>();
        }

        [XmlArray("Glyphs")]
        public List<Glyph> Glyphs { get; set; }

        public int Unknown1 { get; set; }
        public short Unknown2 { get; set; }
        public int Unknown3 { get; set; }

        public static GlyphMap ReadGlyphMap(Stream inputStream)
        {
            GlyphMap glyphMap = new GlyphMap();
            glyphMap.Read(inputStream);
            return glyphMap;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.Default, true);
            Unknown1 = reader.ReadInt32();
            Unknown2 = reader.ReadInt16();
            short glyphCount = reader.ReadInt16();
            int size = reader.ReadInt32();
            Unknown3 = reader.ReadInt32();
            for (int i = 0; i < glyphCount; i++)
            {
                Glyphs.Add(Glyph.ReadGlyph(inputStream));
            }
        }

        public override void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write((short) Glyphs.Count);
            writer.Write(GetAlignedSize(0));
            writer.Write(Unknown3);
            foreach (var glyph in Glyphs.OrderBy(g => g.Character))
            {
                glyph.Write(outputStream);
            }
        }

        public override FfntEntryHeader GetHeader(Stream outputStream)
        {
            FfntEntryHeader header = new FfntEntryHeader
            {
                FfntEntrySignature = GlyphSignature,
                Offset = (int) outputStream.Position,
                Size = GetAlignedSize(16)
            };
            return header;
        }

        private int GetAlignedSize(int startSize)
        {
            int dataSize = Glyphs.Count*Glyph.GlyphSize + startSize;
            int alignmentRequired = dataSize%16;
            if (alignmentRequired > 0)
                dataSize += 16 - alignmentRequired;
            return dataSize;
        }
    }
}
