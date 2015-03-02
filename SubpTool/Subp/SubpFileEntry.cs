using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubpTool.Subp
{
    class SubpFileEntry
    {
        private const short MagicNumber = 0x4C01;

        private readonly List<SubpFileLineTiming> _lineTimings;
        public string Subtitles { get; set; }
        public byte SubtitlePriority { get; set; }


        public SubpFileEntry()
        {
            _lineTimings = new List<SubpFileLineTiming>();
        }

        public IEnumerable<SubpFileLineTiming> LineTimings
        {
            get { return _lineTimings; }
        }

        public static SubpFileEntry ReadSubpFileEntry(Stream input, Encoding encoding)
        {
            SubpFileEntry subpFileEntry = new SubpFileEntry();
            subpFileEntry.Read(input, encoding);
            return subpFileEntry;
        }

        private void Read(Stream input, Encoding encoding)
        {
            BinaryReader reader = new BinaryReader(input, encoding, true);
            short magicNumber = reader.ReadInt16();
            byte lineCount = reader.ReadByte();
            SubtitlePriority = reader.ReadByte();
            short stringLength1 = reader.ReadInt16();
            short stringLength2 = reader.ReadInt16();
            short unknown3 = reader.ReadInt16();
            short flags = reader.ReadInt16();
            for (int i = 0; i < lineCount; i++)
            {
                SubpFileLineTiming subpFileLineTiming = SubpFileLineTiming.ReadSubpFileLineTiming(input);
                AddLineTiming(subpFileLineTiming);
            }
            byte[] data = reader.ReadBytes(stringLength1);
            Subtitles = encoding.GetString(data);
        }

        private void AddLineTiming(SubpFileLineTiming lineTiming)
        {
            _lineTimings.Add(lineTiming);
        }
    }
}
