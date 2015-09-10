using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using LangTool.Lang;

namespace LangTool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                ShowUsageInfo();
                return;
            }

            string path = args[0];

            if (File.Exists(path) == false)
            {
                ShowUsageInfo();
                return;
            }
            string extension = Path.GetExtension(path);
            if (string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream inputStream = new FileStream(path, FileMode.Open))
                using (StreamReader xmlReader = new StreamReader(inputStream, Encoding.UTF8))
                using (FileStream outputStream = new FileStream(path.Substring(0, path.Length - 4), FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof (LangFile));
                    LangFile file = serializer.Deserialize(xmlReader) as LangFile;
                    if (file == null)
                    {
                        Console.WriteLine("XML was not not a valid LangFile");
                        return;
                    }
                    file.Write(outputStream);
                }
            }
            else if (string.Equals(extension, ".lng", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream inputStream = new FileStream(path, FileMode.Open))
                using (FileStream outputStream = new FileStream(path + ".xml", FileMode.Create))
                using (StreamWriter xmlWriter = new StreamWriter(outputStream, Encoding.UTF8))
                {
                    LangFile file = LangFile.ReadLangFile(inputStream);
                    XmlSerializer serializer = new XmlSerializer(typeof (LangFile));
                    serializer.Serialize(xmlWriter, file);
                }
            }
            else
            {
                ShowUsageInfo();
            }
        }

        private static void ShowUsageInfo()
        {
            Console.WriteLine("LangTool by Atvaark\n" +
                              "  A tool for converting between Fox Engine .lng files and xml.\n" +
                              "Usage:\n" +
                              "  LangTool file_path.lng|file_path.xml\n" +
                              "Examples:\n" +
                              "  LangTool gz_cassette.eng.lng     - Converts the lng file to xml\n" +
                              "  LangTool gz_cassette.eng.lng.xml - Converts the xml file to lng");
        }
    }
}
