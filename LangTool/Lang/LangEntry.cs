using System.Xml.Serialization;

namespace LangTool.Lang
{
    [XmlType("Entry")]
    public class LangEntry
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlIgnore]
        public int KeyOffset { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        [XmlIgnore]
        public int ValueOffset { get; set; }

    }
}
