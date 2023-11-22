namespace DayDoc.Web.Models
{
    public class XmlDoc
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public DateTime DateAndTime { get; set; }
        public string FileName { get; set; } = "";

        public int DocId { get; set; }
        public virtual Doc? Doc { get; set; }
    }
}
