using System.IO;
using System.Linq;
using System.Text;

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

        internal static void AlignWrite(this BinaryWriter writer, int alignment, byte data)
        {
            long alignmentRequired = writer.BaseStream.Position % alignment;
            byte[] alignmentBytes = Enumerable.Repeat(data, (int)(alignment - alignmentRequired)).ToArray();
            writer.Write(alignmentBytes, 0, alignmentBytes.Length);
        }
    }
}
