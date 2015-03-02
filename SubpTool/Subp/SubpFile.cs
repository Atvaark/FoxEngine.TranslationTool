using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubpTool.Subp
{
    class SubpFile
    {
        private readonly List<SubpFileIndex> _indices;
        private const short MagicNumber = 0x4C01;

        public SubpFile()
        {
            _indices = new List<SubpFileIndex>();
        }

        public IEnumerable<SubpFileIndex> Indices
        {
            get { return _indices; }
        }

        public static SubpFile ReadSubpFile(Stream input, Encoding encoding)
        {
            SubpFile subpFile = new SubpFile();
            subpFile.ReadSubp(input, encoding);
            return subpFile;
        }

        public void ReadSubp(Stream input, Encoding encoding)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);
            short magicNumber = reader.ReadInt16();
            short entryCount = reader.ReadInt16();
            for (int i = 0; i < entryCount; i++)
            {
                SubpFileIndex index = SubpFileIndex.ReadSubpFileIndex(input);
                AddIndex(index);
            }

            foreach (var index in Indices)
            {
                input.Position = index.Offset;
                index.Entry = SubpFileEntry.ReadSubpFileEntry(input, encoding);
            }
        }

        private void AddIndex(SubpFileIndex index)
        {
            _indices.Add(index);
        }
    }
}
