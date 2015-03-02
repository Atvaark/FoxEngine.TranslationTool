using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubpTool.Subp;

namespace SubpTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Any() || args.Length > 2)
            {
                var encoding = GetEncodingFromArgument(args.Length== 2 ? args[0] : "");
                string path = args.Length == 2 ? args[1] : args[0];
                
                if(path.EndsWith(".subp"))
                {
                    UnpackSubp(path, encoding);
                    return;
                }

            }
            Console.WriteLine("SubpTool by Atvaark\n" +
                              "Description\n" +
                              "  Converts Fox Engine subtitle pack (.subp) files to text (.txt)\n" +
                              "Usage:\n" +
                              "  SubpTool.exe [options] filename.subp -Unpacks the subtitle pack file\n" +
                              "Options:\n" +
                              "  -jpn Use UTF8 encoding for japanese characters\n" +
                              "  -rus Use ISO 8859-5 encoding for cyrillic characters\n" +
                              "  -ger Use ISO-8859-1 encoding for german characters");
        }

        static Encoding GetEncodingFromArgument(string encoding)
        {
            switch (encoding)
            {
                case "-rus":
                    return Encoding.GetEncoding("ISO-8859-5");
                case "-jpn":
                    return Encoding.UTF8;
                case "-ger":
                    return Encoding.GetEncoding("ISO-8859-1");
                default:
                    return Encoding.ASCII;
            }
        }

        private static void UnpackSubp(string path, Encoding encoding)
        {
            string fileDirectory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string outputFileName = String.Format("{0}.txt", fileName);
            string outputFilePath = Path.Combine(fileDirectory, outputFileName);


            using (FileStream input = new FileStream(path, FileMode.Open))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create))
            using (StreamWriter output = new StreamWriter(outputStream, encoding))
            {
                var psubFile = SubpFile.ReadSubpFile(input, encoding);

                foreach (var index in psubFile.Indices)
                {
                    output.WriteLine("ID: {0}", index.SubId);
                    output.WriteLine("Timings:");
                    foreach (var timing in index.Entry.LineTimings)
                    {
                        output.WriteLine("{0} {1}", timing.Start, timing.End);
                    }
                    output.WriteLine("Subtitles:");
                    foreach (var subtitle in index.Entry.Subtitles.Replace("\0", "").Split('$'))
                    {
                        output.WriteLine(subtitle);
                    }
                    output.WriteLine("");

                }
            }
        }
    }
}
