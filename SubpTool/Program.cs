using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SubpTool.Subp;

namespace SubpTool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Any() || args.Length > 2)
            {
                var encoding = GetEncodingFromArgument(args.Length == 2 ? args[0] : "");
                string path = args.Length == 2 ? args[1] : args[0];

                if (path.EndsWith(".subp"))
                {
                    UnpackSubp(path, encoding);
                    return;
                }
                if (path.EndsWith(".xml"))
                {
                    PackSubp(path, encoding);
                    return;
                }
            }
            Console.WriteLine("SubpTool by Atvaark\n" +
                              "Description\n" +
                              "  Converts Fox Engine subtitle pack (.subp) files to xml. \n" +
                              "Usage:\n" +
                              "  SubpTool.exe [options] filename.subp -Unpacks the subtitle pack file\n" +
                              "  SubpTool.exe [options] filename.xml -Packs the subtitle pack file \n" +
                              "Options:\n" +
                              "  -ara, -eng, -fre, -ger, -ita, -jpn, -por, -rus and -spa");
        }

        private static Encoding GetEncodingFromArgument(string encoding)
        {
            switch (encoding)
            {
                case "-rus":
                    return Encoding.GetEncoding("ISO-8859-5");
                case "-jpn":
                case "-ara":
                case "-por":
                    return Encoding.UTF8;
                case "-fre":
                case "-ger":
                case "-spa":
                case "-ita":
                case "-eng":
                default:
                    return Encoding.GetEncoding("ISO-8859-1");
            }
        }

        private static void UnpackSubp(string path, Encoding encoding)
        {
            string fileDirectory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string outputFileName = fileName + ".xml";
            string outputFilePath = Path.Combine(fileDirectory, outputFileName);


            using (FileStream inputStream = new FileStream(path, FileMode.Open))
            using (XmlWriter outputWriter = XmlWriter.Create(outputFilePath, new XmlWriterSettings
            {
                NewLineHandling = NewLineHandling.Entitize,
                Indent = true
            }))
            {
                SubpFile subpFile = SubpFile.ReadSubpFile(inputStream, encoding);
                // TODO: Change XML Encoding
                XmlSerializer serializer = new XmlSerializer(typeof(SubpFile));
                serializer.Serialize(outputWriter, subpFile);
            }
        }

        private static void PackSubp(string path, Encoding encoding)
        {
            string fileDirectory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string outputFileName = fileName + ".subp";
            string outputFilePath = Path.Combine(fileDirectory, outputFileName);

            using (FileStream inputStream = new FileStream(path, FileMode.Open))
            using (XmlReader xmlReader = XmlReader.Create(inputStream, CreateXmlReaderSettings<SubpFile>()))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SubpFile));
                SubpFile subpFile = serializer.Deserialize(xmlReader) as SubpFile;
                subpFile?.Write(outputStream, encoding);
            }
        }

        private static XmlReaderSettings CreateXmlReaderSettings<T>()
        {
            XmlSchemas schemas = new XmlSchemas();
            XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);
            XmlTypeMapping mapping = new XmlReflectionImporter().ImportTypeMapping(typeof(T));
            exporter.ExportTypeMapping(mapping);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            foreach (XmlSchema schema in schemas)
            {
                schemaSet.Add(schema);
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += HandleXmlReaderValidation;
            return settings;
        }

        private static void HandleXmlReaderValidation(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                Console.WriteLine($"{args.Severity} at line '{args.Exception?.LineNumber}' position '{args.Exception?.LinePosition}':\n{args.Message}");
            }
            else
            {
                throw args.Exception;
            }
        }
    }
}
