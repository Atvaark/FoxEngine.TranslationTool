using System;
using System.Collections.Generic;
using System.IO;
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


        public static GlyphMap ReadGlyphMap(Stream inputStream)
        {
            GlyphMap glyphMap = new GlyphMap();
            glyphMap.Read(inputStream);
            return glyphMap;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.Default, true);
            reader.Skip(6);
            short glyphCount = reader.ReadInt16();
            int size = reader.ReadInt32();
            reader.Skip(4);
            for (int i = 0; i < glyphCount; i++)
            {
                Glyphs.Add(Glyph.ReadGlyph(inputStream));
            }
        }

        public override void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.WriteZeros(6);
            writer.Write((short)Glyphs.Count);
            writer.Write(Glyphs.Count*Glyph.GlyphSize);
            writer.WriteZeros(4);
            foreach (var glyph in Glyphs)
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
                Size = Glyphs.Count * Glyph.GlyphSize + 24
            };
            return header;
        }
    }
}
