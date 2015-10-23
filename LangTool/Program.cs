using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using LangTool.Lang;
using LangTool.Utility;

namespace LangTool
{
    internal class Program
    {
        const string DefaultDictionaryPath = "lang_dictionary.txt";

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
            if (String.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
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
            else if (String.Equals(extension, ".lng", StringComparison.OrdinalIgnoreCase)
                     || String.Equals(extension, ".lng2", StringComparison.OrdinalIgnoreCase))
            {
                var dictionary = GetDictionary(DefaultDictionaryPath);
                using (FileStream inputStream = new FileStream(path, FileMode.Open))
                using (FileStream outputStream = new FileStream(path + ".xml", FileMode.Create))
                using (StreamWriter xmlWriter = new StreamWriter(outputStream, Encoding.UTF8))
                {
                    LangFile file = LangFile.ReadLangFile(inputStream, dictionary);
                    XmlSerializer serializer = new XmlSerializer(typeof (LangFile));
                    serializer.Serialize(xmlWriter, file);
                }
            }
            else
            {
                ShowUsageInfo();
            }
        }
        
        private static Dictionary<uint, string> GetDictionary(string path)
        {
            var dictionary = new Dictionary<uint, string>();
            try
            {
                var values = File.ReadAllLines(path);
                foreach (var value in values)
                {
                    var code = Fox.GetStrCode32(value);
                    DebugCheckCollision(dictionary, code, value);
                    dictionary[code] = value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to read the dictionary " + e);
            }

            return dictionary;
        }

        [Conditional("DEBUG")]
        private static void DebugCheckCollision(Dictionary<uint, string> dictionary, uint code, string newValue)
        {
            string originalValue;
            if (dictionary.TryGetValue(code, out originalValue))
            {
                Debug.WriteLine("StrCode32 collision detected ({0}). Overwriting '{1}' with '{2}'", code, originalValue, newValue);
            }
        }

        private static void ShowUsageInfo()
        {
            Console.WriteLine("LangTool by Atvaark\n" +
                              "  A tool for converting between Fox Engine .lng/.lng2 files and xml.\n" +
                              "Usage:\n" +
                              "  LangTool file_path.lng|file_path.lng2|file_path.xml\n" +
                              "Examples:\n" +
                              "  LangTool gz_cassette.eng.lng     - Converts the lng file to xml\n" +
                              "  LangTool gz_cassette.eng.lng.xml - Converts the xml file to lng");
        }
    }
}
