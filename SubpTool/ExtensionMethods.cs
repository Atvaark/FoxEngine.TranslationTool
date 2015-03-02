using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubpTool
{
    internal static class ExtensionMethods
    {
        internal static string ReadString(this BinaryReader binaryReader, int count)
        {
            return new string(binaryReader.ReadChars(count));
        }

    }
}
