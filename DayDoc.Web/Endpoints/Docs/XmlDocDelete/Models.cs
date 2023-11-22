using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class XmlDocDeleteRequest : ICommand
    {
        public int DocId { get; set; }
        public int Id { get; set; }
    }
}
