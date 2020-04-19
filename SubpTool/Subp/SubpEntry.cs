using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SubpTool.Subp
{
    [XmlType("Entry")]
    public class SubpEntry
    {
        private const short MagicNumber = 0x4C01;

        public SubpEntry()
        {
            Lines = new List<SubpLine>();
        }

        [XmlAttribute("Id")]
        public uint SubtitleId { get; set; }

        [XmlAttribute("Priority")]
        public byte SubtitlePriority { get; set; }

        [XmlAttribute("Flags")]
        public short Flags { get; set; }

        [XmlAttribute("CharacterId")]
        public short CharacterId { get; set; }

        [XmlAttribute("AdditionalLength")]
        public short AdditionalLength { get; set; }
        
        [XmlArray("Lines")]
        public List<SubpLine> Lines { get; set; }

        public static SubpEntry ReadSubpEntry(Stream input, Encoding encoding)
        {
            SubpEntry subpEntry = new SubpEntry();
            subpEntry.Read(input, encoding);
            return subpEntry;
        }

        private void Read(Stream input, Encoding encoding)
        {
            BinaryReader reader = new BinaryReader(input, encoding, true);
            short magicNumber = reader.ReadInt16();
            byte timingCount = reader.ReadByte();
            SubtitlePriority = reader.ReadByte();
            // TODO: Check if this is string length and (encoded) byte count.
            short stringLength1 = reader.ReadInt16();
            short stringLength2 = reader.ReadInt16();
            // TODO: Analyze what these values are used for
            AdditionalLength = Convert.ToInt16(stringLength2 - stringLength1);
            CharacterId = reader.ReadInt16();
            Flags = reader.ReadInt16();
            
            SubpTiming[] timings = new SubpTiming[timingCount];
            for (int i = 0; i < timingCount; i++)
            {
                timings[i] = SubpTiming.ReadSubpTiming(input);
            }

            byte[] data = reader.ReadBytes(stringLength1);
            string subtitles = encoding.GetString(data).TrimEnd('\0');
            
            // TODO: Check if the '$' literal can be escaped somehow
            // TODO: Check if Split('$').Count == lineCount
            string[] lines = subtitles.Split('$');
            for (int i = 0; i < timingCount; i++)
            {
                Lines.Add(new SubpLine(lines.Length > i ? lines[i] : "", timings[i]));
            }

            if (lines.Length > timingCount)
            {
                // More lines than timings => Append without timing
                for (int i = timingCount; i < lines.Length; i++)
                {
                    Lines.Add(new SubpLine(lines[i], null));
                }
            }
        }
        
        public SubpIndex GetIndex(Stream outputStream)
        {
            return new SubpIndex
            {
                SubtitleId = SubtitleId,
                Offset = (uint) outputStream.Position
            };
        }

        private string GetJoinedSubtitleLines()
        {
            return string.Join("$", Lines.Select(l => l.Text));
        }

        public void Write(Stream outputStream, Encoding encoding)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, encoding, true);
            writer.Write(MagicNumber);
            SubpTiming[] timings = Lines.Select(line => line.Timing).Where(timing => timing != null).ToArray();
            writer.Write((byte)timings.Length);
            writer.Write(SubtitlePriority);

            string subtitles = GetJoinedSubtitleLines() + '\0';
            byte[] encodedData = encoding.GetBytes(subtitles);

            writer.Write(Convert.ToInt16(encodedData.Length));
            writer.Write(Convert.ToInt16(encodedData.Length + AdditionalLength));
            writer.Write(CharacterId);
            writer.Write(Flags);

            foreach (var timing in timings)
            {
                timing.Write(outputStream);
            }

            writer.Write(encodedData);
        }
    }
}
