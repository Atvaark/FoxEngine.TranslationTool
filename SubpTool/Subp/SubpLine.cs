using System.Xml.Serialization;

namespace SubpTool.Subp
{
    [XmlType("Line")]
    public class SubpLine
    {
        public SubpLine()
        {
        }

        public SubpLine(string text, SubpTiming timing)
        {
            Text = text;
            Timing = timing;
        }

        [XmlAttribute("Text")]
        public string Text { get; set; }

        [XmlElement("Timing", IsNullable = true)]
        public SubpTiming Timing { get; set; }

        public bool ShouldSerializeTiming()
        {
            return Timing != null;
        }
    }
}
