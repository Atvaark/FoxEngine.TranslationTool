using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubpTool.Subp
{
    class SubpFileLineTiming
    {
        public ushort Start { get; set; }
        public ushort End { get; set; }

        public static SubpFileLineTiming ReadSubpFileLineTiming(Stream input)
        {
            SubpFileLineTiming subpFileLineTiming = new SubpFileLineTiming();
            subpFileLineTiming.Read(input);
            return subpFileLineTiming;
        }

        private void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);
            Start = reader.ReadUInt16();
            End = reader.ReadUInt16();
        }
    }
}
