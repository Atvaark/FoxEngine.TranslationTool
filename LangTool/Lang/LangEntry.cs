using System.Xml.Serialization;

namespace LangTool.Lang
{
    [XmlType("Entry")]
    public class LangEntry
    {
        [XmlAttribute]
        public uint Key { get; set; }

        [XmlIgnore]
        public int Offset { get; set; }

        [XmlAttribute]
        public short Color { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }
}
