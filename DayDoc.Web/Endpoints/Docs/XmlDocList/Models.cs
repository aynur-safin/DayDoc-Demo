using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class XmlDocListRequest : ICommand<XmlDocListResponse>
    {
        public int DocId { get; set; }     
    }

    public class XmlDocListResponse
    {
        public List<XmlDoc>? XmlDocs { get; set; }
    }
}
