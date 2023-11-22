using DayDoc.Web.Models;
using FastEndpoints;

namespace DayDoc.Web.Endpoints.Models
{
    public class XmlDocGetFileRequest : ICommand<XmlDocGetFileInfoResponse>
    {
        public int Id { get; set; }     
    }

    public class XmlDocGetFileInfoResponse
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string ContentType { get; set; } = "application/xml";
    }
}
