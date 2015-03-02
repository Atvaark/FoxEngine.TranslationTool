using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangTool
{
    internal static class ExtensionMethods
    {
        internal static string ReadNullTerminatedString(this BinaryReader reader)
        {
            StringBuilder builder = new StringBuilder();
            char nextCharacter;
            while ((nextCharacter = reader.ReadChar()) != 0x00)
            {
                builder.Append(nextCharacter);
            }
            return builder.ToString();
        }


        internal static void WriteNullTerminatedString(this BinaryWriter writer, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text + '\0');
            writer.Write(data, 0, data.Length);
        }
    }
}
