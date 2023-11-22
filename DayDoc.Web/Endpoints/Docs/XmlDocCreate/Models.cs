using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{ 
    public class XmlDocCreateRequest : ICommand<XmlDocCreateResponse>
    {
        public int DocId { get; set; }     
    }

    public class XmlDocCreateResponse
    {
        public XmlDoc? XmlDoc { get; set; }
    }
}
