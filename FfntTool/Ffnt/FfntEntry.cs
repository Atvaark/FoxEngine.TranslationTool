using System.IO;

namespace FfntTool.Ffnt
{
    public abstract class FfntEntry
    {
        public abstract FfntEntryHeader GetHeader(Stream outputStream);
        public abstract void Write(Stream outputStream);
    }
}
