using System.IO;

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
